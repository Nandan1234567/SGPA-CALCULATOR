// Application/Services/VtuCreditResolver.cs
// ─────────────────────────────────────────────────────────────────────────────
// THE CORE CREDIT ENGINE — resolves credits for any VTU 22-scheme subject code.
//
// ┌─ BUG FIXED ──────────────────────────────────────────────────────────────┐
// │  Original regex:  ^B([A-Z]{2,3})(L?)(\d{3})([A-Z]?)$                   │
// │  Problem:  "BCSL305" matches as branch="CSL", isLab=false               │
// │            The greedy {2,3} consumes the 'L', leaving group 2 empty.    │
// │  Fixed with TWO separate patterns:                                       │
// │    Lab:    ^B([A-Z]{2})L(\d{3})([A-Z]?)$   → always 2-letter branch    │
// │    Theory: ^B([A-Z]{2,3})(\d{3})([A-Z]?)$  → 2-3 letter branch, no L  │
// │  Verified against: BCSL305 ✓  BCS301 ✓  BAIL504 ✓  BCS306A ✓          │
// └──────────────────────────────────────────────────────────────────────────┘
//
// LEARNING CONCEPT — Regex named groups:
//   A regex like ^B([A-Z]{2})L(\d{3})([A-Z]?)$
//   has numbered capture groups: group(1)=branch, group(2)=number, group(3)=suffix.
//   We can also name them: (?<branch>[A-Z]{2})  and access via match.Groups["branch"].
//   Named groups make the code self-documenting.
// ─────────────────────────────────────────────────────────────────────────────

using System.Text.RegularExpressions;
using SGPA_CALCULATOR.Infrastructure.Data;

namespace SGPA_CALCULATOR.Application.Services
{
    public class VtuCreditResolver
    {
        private readonly SgpaDbContext _db;

        public VtuCreditResolver(SgpaDbContext db)
        {
            _db = db;
        }

        // ── FIXED: Two patterns instead of one ambiguous pattern ─────────────
        //
        // Lab subject pattern:    B + [2 uppercase] + L + [3 digits] + [optional suffix]
        //   Matches: BCSL305, BAIL504, BCSL404, BCSL606
        //   BCSL305: branch=CS, num=305 → sem=3, pos=05 → 1 credit  ✓
        //
        // Theory subject pattern: B + [2-3 uppercase] + [3 digits] + [optional suffix]
        //   Matches: BCS301, BCS306A, BCS358A, BIS701, BCA786, BSCK307, BNSK359
        //   BCS301:  branch=CS,  num=301 → sem=3, pos=01 → 4 credits  ✓
        //   BSCK307: branch=SCK, num=307 → caught by _commonCodes dict first
        //   BNSK359: branch=NSK, num=359 → caught by _zeroCredit set first

        private static readonly Regex _labPattern = new(
            @"^B(?<branch>[A-Z]{2})L(?<num>\d{3})(?<suffix>[A-Z]?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        private static readonly Regex _theoryPattern = new(
            @"^B(?<branch>[A-Z]{2,3})(?<num>\d{3})(?<suffix>[A-Z]?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        // ── Zero-credit mandatory courses — always excluded from SGPA ────────
        private static readonly HashSet<string> _zeroCredit = new(StringComparer.OrdinalIgnoreCase)
        {
            "BNSK359", "BNSK459", "BNSK559", "BNSK658",   // NSS
            "BPEK359", "BPEK459", "BPEK559", "BPEK658",   // Physical Education
            "BYOK359", "BYOK459", "BYOK559", "BYOK658",   // Yoga
            "BIKS609",                                      // IKS (Sem 6)
        };

        // ── Common cross-branch codes (same code for ALL branches) ────────────
        private static readonly Dictionary<string, CreditInfo> _commonCodes =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "BSCK307", new CreditInfo("BSCK307", 1, true,  "Social Connect (UHV - excluded from SGPA)") },
            { "BBOC407", new CreditInfo("BBOC407", 2, false, "Biology for CS Engineers") },
            { "BUHK408", new CreditInfo("BUHK408", 1, false, "Universal Human Values") },
            { "BRMK557", new CreditInfo("BRMK557", 3, false, "Research Methodology & IPR") },
            { "BCS508",  new CreditInfo("BCS508",  2, false, "Environmental Studies") },
            { "BEC508",  new CreditInfo("BEC508",  2, false, "Environmental Studies") },
            { "BEE508",  new CreditInfo("BEE508",  2, false, "Environmental Studies") },
            { "BME508",  new CreditInfo("BME508",  2, false, "Environmental Studies") },
            { "BICK860", new CreditInfo("BICK860", 1, true,  "Industry Connect (non-credit)") },
        };

        /// <summary>
        /// Resolves any VTU 22-scheme subject code to its credit count.
        /// Resolution priority:
        ///   1. Zero-credit mandatory (NSS, PE, Yoga, IKS)
        ///   2. Sem 1 & 2 fixed codes from DB
        ///   3. Common cross-branch codes (in-memory dict)
        ///   4. Pattern-based inference (lab or theory regex → switch)
        /// </summary>
        public CreditInfo Resolve(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode))
                return CreditInfo.Unknown(rawCode ?? "");

            var code = rawCode.Trim().ToUpperInvariant();

            // Priority 1 — zero-credit mandatory
            if (_zeroCredit.Contains(code))
                return new CreditInfo(code, 0, true, "Zero-credit mandatory (NSS/PE/Yoga/IKS)");

            // Priority 2 — Sem 1 & 2 exact codes from DB
            var sem12 = _db.SubjectMasters.FirstOrDefault(s => s.SubjectCode == code);
            if (sem12 != null)
                return new CreditInfo(code, sem12.Credits, sem12.IsNonCreditForSgpa, "Sem1/2 fixed code (DB)");

            // Priority 3 — common cross-branch codes
            if (_commonCodes.TryGetValue(code, out var common))
                return common;

            // Priority 4 — pattern inference
            return InferFromPattern(code);
        }

        private static CreditInfo InferFromPattern(string code)
        {
            // ── Try lab pattern first ─────────────────────────────────────────
            // Example: BCSL305 → branch=CS, num=305, isLab=true
            var labMatch = _labPattern.Match(code);
            if (labMatch.Success)
            {
                string branch = labMatch.Groups["branch"].Value;
                int num = int.Parse(labMatch.Groups["num"].Value);
                string suffix = labMatch.Groups["suffix"].Value;
                return Lookup(code, branch, true, num, suffix);
            }

            // ── Then try theory pattern ───────────────────────────────────────
            // Example: BCS306A → branch=CS, num=306, suffix=A, isLab=false
            var theorMatch = _theoryPattern.Match(code);
            if (theorMatch.Success)
            {
                string branch = theorMatch.Groups["branch"].Value;
                int num = int.Parse(theorMatch.Groups["num"].Value);
                string suffix = theorMatch.Groups["suffix"].Value;
                return Lookup(code, branch, false, num, suffix);
            }

            return CreditInfo.Unknown(code);
        }

        private static CreditInfo Lookup(string code, string branch, bool isLab, int num, string suffix)
        {
            int sem = num / 100;   // 301 → sem 3,  786 → sem 7
            int pos = num % 100;   // 301 → pos 01, 358 → pos 58

            // ── Position → Credit lookup ──────────────────────────────────────
            // Each tuple: (semester, position, isLab) → (credits, nonCreditForSgpa, reason)
            var (credits, nonCredit, reason) = (sem, pos, isLab) switch
            {  // Total credit-bearing: ~20 credits
                (3, 01, false) => (4, false, "Sem3 Core1 BSC/PCC (with Tutorial)"),  // BCS301, BEC301…
                (3, 02, false) => (4, false, "Sem3 Core2 IPCC (Theory+Lab)"),         // BCS302…
                (3, 03, false) => (4, false, "Sem3 Core3 IPCC"),                       // BCS303…
                (3, 04, false) => (3, false, "Sem3 Core4 PCC Theory"),                 // BCS304…
                (3, 05, true) => (1, false, "Sem3 Lab (BCSL305)"),                    // BCSL305
                (3, 06, false) => (3, false, "Sem3 ESC Elective (306A/B)"),            // BCS306A…
                (3, 58, false) => (1, false, "Sem3 Skill/AEC (358A/B/C/D)"),           // BCS358A…
                (3, 58, true) => (1, false, "Sem3 Skill/AEC Lab variant"),             // BCSL358A…

                // ═══════════════ SEMESTER 4 ═══════════════════════════════════
                // Total credit-bearing: ~19 credits
                (4, 01, false) => (3, false, "Sem4 Core1 PCC Theory"),                 // BCS401…
                (4, 02, false) => (4, false, "Sem4 Core2 IPCC"),                       // BIS402, BAI402…
                (4, 03, false) => (4, false, "Sem4 Core3 IPCC"),                       // BCS403…
                (4, 04, true) => (1, false, "Sem4 Lab (BCSL404)"),                    // BCSL404
                (4, 05, false) => (3, false, "Sem4 ESC Elective (405A/B/C/D)"),        // BCS405A…
                (4, 56, false) => (1, false, "Sem4 Skill/AEC (456A/B/C)"),             // BCS456A…
                (4, 56, true) => (1, false, "Sem4 Skill/AEC Lab (BCSL456D)"),         // BCSL456D

                // ═══════════════ SEMESTER 5 ═══════════════════════════════════
                // Total credit-bearing: ~22 credits
                (5, 01, false) => (4, false, "Sem5 Core1 PCC Theory"),                 // BCS501…
                (5, 02, false) => (4, false, "Sem5 Core2 IPCC"),                       // BCS502…
                (5, 03, false) => (4, false, "Sem5 Core3 PCC (with Tutorial)"),        // BCS503…
                (5, 04, true) => (1, false, "Sem5 Lab (BAIL504, BCSL504)"),           // BAIL504…
                (5, 04, false) => (1, false, "Sem5 Lab alternate code"),               // safety fallback
                (5, 08, false) => (1, false, "Sem5 Environmental Studies (BCS508)"),   // common, caught above usually
                (5, 15, false) => (3, false, "Sem5 Professional Elective (515A/B/C)"), // BCS515A, BAI515B…
                                                                                       //(5, 57, false) => (3, false, "Sem5 Skill/AEC (557A/B/C)"),             // BCS557A…
                                                                                       //(5, 57, true)  => (1, false, "Sem5 Skill/AEC Lab variant"),
                (5, 86, false) => (2, false, "Sem5 Mini Project (BIS586, BCA586)"),    // BIS586, BCA586

                // ═══════════════ SEMESTER 6 ═══════════════════════════════════
                // Total credit-bearing: ~18 credits
                (6, 01, false) => (4, false, "Sem6 Core1 IPCC"),                       // BIS601, BCO601…
                (6, 02, false) => (4, false, "Sem6 Core2 PCC Theory"),                 // BCS602…
                (6, 06, true) => (1, false, "Sem6 Lab (BCSL606)"),                    // BCSL606
                (6, 13, false) => (3, false, "Sem6 Professional Elective (613A/B/C)"), // BCS613A…
                (6, 54, false) => (3, false, "Sem6 Open Elective (654A/B/C)"),         // BCS654A…
                (6, 57, false) => (1, false, "Sem6 AEC/SDC (657A/B/C)"),               // BCS657A, BISL657A…
                (6, 57, true) => (1, false, "Sem6 AEC/SDC Lab variant"),
                (6, 85, false) => (2, false, "Sem6 Project Phase I (BIS685, BCA685)"), // BIS685…
                (6, 09, false) => (0, true, "Sem6 IKS mandatory (handled via dict)"), // safety

                // ═══════════════ SEMESTER 7 ═══════════════════════════════════
                // Total credit-bearing: ~24 credits
                (7, 01, false) => (4, false, "Sem7 Core1 IPCC"),                       // BIS701, BCA701…
                (7, 02, false) => (4, false, "Sem7 Core2 IPCC"),                       // BCS702, BCA702…
                (7, 03, false) => (4, false, "Sem7 Core3 PCC Theory"),                 // BIS703, BCS703…
                (7, 14, false) => (3, false, "Sem7 Professional Elective (714A/B/C)"), // BIS714A, BCA714A…
                (7, 55, false) => (3, false, "Sem7 Open Elective (755A/B/C)"),         // BIS755A, BCA755A…
                (7, 86, false) => (6, false, "Sem7 Major Project Phase II (BIS786)"),  // BIS786, BCA786

                // ═══════════════ SEMESTER 8 ═══════════════════════════════════
                // Total credit-bearing: ~16 credits
                (8, 01, false) => (3, false, "Sem8 PEC Online (801A/B/C/D)"),          // BIS801A, BCA801A…
                (8, 02, false) => (3, false, "Sem8 OEC Online (802A/B/C/D)"),          // BIS802A…
                (8, 03, false) => (10, false, "Sem8 Internship (BIS803, BCA803)"),     // BIS803…

                // ── Unknown position ──
                _ => (0, false, $"UNKNOWN: sem={sem} pos={pos} lab={isLab} branch={branch}")
            };

            if (credits == 0 && !nonCredit)
                return CreditInfo.Unknown(code, reason);

            return new CreditInfo(code, credits, nonCredit, reason);
        }
    }

    // ── Value object returned by Resolve() ────────────────────────────────────
    // record = immutable class with auto-generated Equals/GetHashCode/ToString
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