// Application/Services/VtuCreditResolver.cs
// ─────────────────────────────────────────────────────────────────────────────
// BUG FIXED — N+1 DB Query Problem
//
// OLD (broken):
//   foreach (subject) {
//       _db.SubjectMasters.FirstOrDefault(s => s.SubjectCode == code); // ← DB hit!
//   }
//   9 subjects = 9 DB round-trips. Visible in SQL Server Profiler.
//
// NEW (fixed):
//   Constructor loads ALL rows into a Dictionary<string, SubjectMaster> ONCE.
//   Then every lookup is O(1) in-memory. 1 DB hit per request, always.
//
// LEARNING — What is N+1?
//   It's when you make 1 query to get a list, then N more queries for each item.
//   Classic sign: "why is my page slow when there are many items?"
//   Fix pattern: always load collections with a single query, then look up locally.
//
// BUG ALSO FIXED — Regex (already in previous version, kept here)
//   Two patterns instead of one greedy pattern.
//   BCSL305: old → branch="CSL" isLab=false (wrong). New → branch="CS" isLab=true ✓
// ─────────────────────────────────────────────────────────────────────────────

using System.Text.RegularExpressions;
using SGPA_CALCULATOR.Infrastructure.Data;

namespace SGPA_CALCULATOR.Application.Services
{
    public class VtuCreditResolver
    {
        // ── N+1 FIX: one dictionary loaded once in constructor ────────────────
        // Key: subject code uppercase. Value: SubjectMaster from DB.
        // All Sem 1 & 2 codes live here. O(1) lookup, zero DB calls after init.
        private readonly Dictionary<string, SubjectMaster> _sem12Cache;

        public VtuCreditResolver(SgpaDbContext db)
        {
            // Fix: Group by SubjectCode and take the first occurrence to avoid duplicate key crash
            _sem12Cache = db.SubjectMasters
                .AsEnumerable() // Move to memory to handle string comparisons safely
                .GroupBy(s => s.SubjectCode.ToUpperInvariant())
                .ToDictionary(
                    g => g.Key,
                    g => g.First() // If duplicates exist, take the first one
                );
        }

        // ── TWO REGEX PATTERNS (N+1 fix was the new bug; these were fixed earlier) ──
        // Lab:    B + [2 letters] + L + [3 digits] + [optional suffix]
        // Theory: B + [2-3 letters]   + [3 digits] + [optional suffix]
        private static readonly Regex _labPattern = new(
            @"^B(?<branch>[A-Z]{2})L(?<num>\d{3})(?<suffix>[A-Z]?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );
        private static readonly Regex _theoryPattern = new(
            @"^B(?<branch>[A-Z]{2,3})(?<num>\d{3})(?<suffix>[A-Z]?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        // ── Zero-credit mandatory — always excluded from SGPA ─────────────────
        private static readonly HashSet<string> _zeroCredit =
            new(StringComparer.OrdinalIgnoreCase)
        {
            "BNSK359","BNSK459","BNSK559","BNSK658",  // NSS
            "BPEK359","BPEK459","BPEK559","BPEK658",  // Physical Education
            "BYOK359","BYOK459","BYOK559","BYOK658",  // Yoga
            "BIKS609",                                  // Indian Knowledge System (Sem 6)
        };

        // ── Common cross-branch codes ─────────────────────────────────────────
        private static readonly Dictionary<string, CreditInfo> _commonCodes =
            new(StringComparer.OrdinalIgnoreCase)
        {
            // Sem 3
            { "BSCK307", new CreditInfo("BSCK307", 1, true,  "Social Connect (excluded from SGPA)") },
            // Sem 4
            { "BBOC407", new CreditInfo("BBOC407", 2, false, "Biology for CS Engineers") },
            { "BBOK407", new CreditInfo("BBOK407", 3, false, "Biology for CS Engineers (3cr variant)") },
            { "BUHK408", new CreditInfo("BUHK408", 1, false, "Universal Human Values") },
            // Sem 5
            { "BRMK557", new CreditInfo("BRMK557", 3, false, "Research Methodology & IPR") },
            { "BCS508",  new CreditInfo("BCS508",  2, false, "Environmental Studies") },
            { "BEC508",  new CreditInfo("BEC508",  2, false, "Environmental Studies") },
            { "BEE508",  new CreditInfo("BEE508",  2, false, "Environmental Studies") },
            { "BME508",  new CreditInfo("BME508",  2, false, "Environmental Studies") },
            { "BESK508", new CreditInfo("BESK508", 2, false, "Environmental Studies") },
            // Sem 8
            { "BICK860", new CreditInfo("BICK860", 1, true,  "Industry Connect (excluded from SGPA)") },
        };

        /// <summary>
        /// Resolves any VTU 22-scheme subject code → credits + SGPA exclusion flag.
        ///
        /// Priority order:
        ///   1. Zero-credit mandatory (NSS, PE, Yoga, IKS)   — O(1) HashSet
        ///   2. Sem 1 & 2 codes from DB                       — O(1) Dictionary (N+1 fixed)
        ///   3. Common cross-branch codes (in-memory)          — O(1) Dictionary
        ///   4. Pattern-based inference (regex → switch)       — fast, no I/O
        /// </summary>
        public CreditInfo Resolve(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode))
                return CreditInfo.Unknown(rawCode ?? "");

            var code = rawCode.Trim().ToUpperInvariant();

            // Priority 1 — zero-credit mandatory
            if (_zeroCredit.Contains(code))
                return new CreditInfo(code, 0, true, "Zero-credit mandatory (NSS/PE/Yoga/IKS)");

            // Priority 2 — Sem 1 & 2 from in-memory cache (was N+1, now O(1))
            if (_sem12Cache.TryGetValue(code, out var cached))
                return new CreditInfo(code, cached.Credits, cached.IsNonCreditForSgpa, "Sem1/2 fixed code (DB cache)");

            // Priority 3 — common cross-branch codes
            if (_commonCodes.TryGetValue(code, out var common))
                return common;

            // Priority 4 — pattern inference
            return InferFromPattern(code);
        }

        private static CreditInfo InferFromPattern(string code)
        {
            // Try lab pattern first: BCSL305, BCSL504, BAIL504, BCSL404, BCSL606
            var labMatch = _labPattern.Match(code);
            if (labMatch.Success)
            {
                int num = int.Parse(labMatch.Groups["num"].Value);
                string branch = labMatch.Groups["branch"].Value;
                string suffix = labMatch.Groups["suffix"].Value;
                return Lookup(code, branch, isLab: true, num, suffix);
            }

            // Then theory pattern: BCS301, BCS306A, BIS701, BCA786
            var theoryMatch = _theoryPattern.Match(code);
            if (theoryMatch.Success)
            {
                int num = int.Parse(theoryMatch.Groups["num"].Value);
                string branch = theoryMatch.Groups["branch"].Value;
                string suffix = theoryMatch.Groups["suffix"].Value;
                return Lookup(code, branch, isLab: false, num, suffix);
            }

            return CreditInfo.Unknown(code);
        }

        private static CreditInfo Lookup(string code, string branch, bool isLab, int num, string suffix)
        {
            int sem = num / 100;   // 301 → sem 3
            int pos = num % 100;   // 301 → pos 01

            var (credits, nonCredit, reason) = (sem, pos, isLab) switch
            {
                // ══ SEMESTER 3 ══════════════════════════════════════════════
                (3, 01, false) => (4, false, "Sem3 Core1 BSC/PCC"),      // BCS301
                (3, 02, false) => (4, false, "Sem3 Core2 IPCC"),          // BCS302
                (3, 03, false) => (4, false, "Sem3 Core3 IPCC"),          // BCS303
                (3, 04, false) => (3, false, "Sem3 Core4 PCC Theory"),    // BCS304
                (3, 05, true) => (1, false, "Sem3 Lab"),                 // BCSL305 ← was broken before fix
                (3, 06, false) => (3, false, "Sem3 ESC Elective"),        // BCS306A/B
                (3, 58, false) => (1, false, "Sem3 AEC/Skill"),           // BCS358A/B/C/D
                (3, 58, true) => (1, false, "Sem3 AEC Lab variant"),

                // ══ SEMESTER 4 ══════════════════════════════════════════════
                (4, 01, false) => (3, false, "Sem4 Core1 PCC Theory"),    // BCS401
                (4, 02, false) => (4, false, "Sem4 Core2 IPCC"),          // BAD402, BAI402
                (4, 03, false) => (4, false, "Sem4 Core3 IPCC"),          // BCS403
                (4, 04, true) => (1, false, "Sem4 Lab"),                 // BCSL404
                (4, 05, false) => (3, false, "Sem4 ESC Elective"),        // BCS405A/B/C/D
                (4, 56, false) => (1, false, "Sem4 AEC/Skill"),           // BCS456A/B/C
                (4, 56, true) => (1, false, "Sem4 AEC Lab"),             // BCSL456D / BDSL456B

                // ══ SEMESTER 5 ══════════════════════════════════════════════
                (5, 01, false) => (3, false, "Sem5 Core1 PCC Theory"),    // BCS501 = 3cr (NOT 4)
                (5, 02, false) => (4, false, "Sem5 Core2 IPCC"),          // BCS502
                (5, 03, false) => (4, false, "Sem5 Core3 PCC"),           // BCS503
                (5, 04, true) => (1, false, "Sem5 Lab"),                 // BCSL504, BAIL504
                (5, 04, false) => (1, false, "Sem5 Lab (no-L variant)"),
                (5, 15, false) => (3, false, "Sem5 Professional Elective"), // BCS515A/B/C/D
                (5, 57, false) => (1, false, "Sem5 AEC/Skill"),           // BCS557A/B/C
                (5, 57, true) => (1, false, "Sem5 AEC Lab variant"),
                (5, 86, false) => (2, false, "Sem5 Mini Project"),        // BCS586, BIS586

                // ══ SEMESTER 6 ══════════════════════════════════════════════
                (6, 01, false) => (4, false, "Sem6 Core1 IPCC"),
                (6, 02, false) => (4, false, "Sem6 Core2 PCC Theory"),
                (6, 06, true) => (1, false, "Sem6 Lab"),                 // BCSL606
                (6, 13, false) => (3, false, "Sem6 Professional Elective"),
                (6, 54, false) => (3, false, "Sem6 Open Elective"),
                (6, 57, false) => (1, false, "Sem6 AEC/SDC"),
                (6, 57, true) => (1, false, "Sem6 AEC Lab variant"),
                (6, 85, false) => (2, false, "Sem6 Project Phase I"),     // BIS685, BCA685
                (6, 09, false) => (0, true, "Sem6 IKS (safety catch)"),

                // ══ SEMESTER 7 ══════════════════════════════════════════════
                (7, 01, false) => (4, false, "Sem7 Core1 IPCC"),
                (7, 02, false) => (4, false, "Sem7 Core2 IPCC"),
                (7, 03, false) => (4, false, "Sem7 Core3 PCC Theory"),
                (7, 14, false) => (3, false, "Sem7 Professional Elective"),
                (7, 55, false) => (3, false, "Sem7 Open Elective"),
                (7, 86, false) => (6, false, "Sem7 Major Project Phase II"),

                // ══ SEMESTER 8 ══════════════════════════════════════════════
                (8, 01, false) => (3, false, "Sem8 PEC Online"),
                (8, 02, false) => (3, false, "Sem8 OEC Online"),
                (8, 03, false) => (10, false, "Sem8 Internship"),

                _ => (0, false, $"UNKNOWN: sem={sem} pos={pos} lab={isLab} branch={branch}")
            };

            if (credits == 0 && !nonCredit)
                return CreditInfo.Unknown(code, reason);

            return new CreditInfo(code, credits, nonCredit, reason);
        }
    }

    public record CreditInfo(
        string SubjectCode,
        int Credits,
        bool IsNonCreditForSgpa,
        string ResolutionMethod)
    {
        public bool IsResolved => Credits > 0 || IsNonCreditForSgpa;

        public static CreditInfo Unknown(string code, string? hint = null) =>
            new(code, 0, false,
                $"UNRESOLVED{(hint != null ? ": " + hint : "")} — check VTU scheme or override manually");
    }
}