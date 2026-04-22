using SGPA_CALCULATOR.Infrastructure.Data;
using System.Text.RegularExpressions;

namespace SGPA_CALCULATOR.Application.Services
{

    // ─────────────────────────────────────────────────────────────────────────────
    // FILE 3: Application/Services/VtuCreditResolver.cs
    // THE CORE ENGINE — resolves credits for any VTU subject code.
    //
    // Resolution priority:
    //   1. Zero-credit mandatory courses (NSS, PE, Yoga, IKS)
    //   2. Sem 1 & 2 fixed codes (DB lookup — 21 rows total)
    //   3. Common cross-branch codes (in-memory dict: BSCK307, BBOC407, etc.)
    //   4. Pattern-based inference for all branch-specific codes (Sem 3–8)
    //
    // Pattern rule: B[branch 2-3 chars][optional L][3-digit num][optional suffix]
    //   BCS301  → branch=CS,  lab=false, num=301, sem=3, pos=01 → 4 credits


    /// </summary>
    public class VtuCreditResolver
    {
        private readonly SgpaDbContext _dbContext;

        public VtuCreditResolver(SgpaDbContext db)
        {
            _dbContext = db;
        }

        // ── Regex: B + [2-3 uppercase letters] + [optional L] + [3 digits] + [optional A-Z] ──
        private static readonly Regex _pattern = new(
            @"^B([A-Z]{2,3})(L?)(\d{3})([A-Z]?)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        // ── Zero-credit mandatory courses — always excluded from SGPA ──
        private static readonly HashSet<string> _zeroCredit = new(StringComparer.OrdinalIgnoreCase)
        {
            // NSS (Sem 3–6)
            "BNSK359", "BNSK459", "BNSK559", "BNSK658",
            // Physical Education (Sem 3–6)
            "BPEK359", "BPEK459", "BPEK559", "BPEK658",
            // Yoga (Sem 3–6)
            "BYOK359", "BYOK459", "BYOK559", "BYOK658",
            // Indian Knowledge System (Sem 6, MC type)
            "BIKS609",
        };

        // ── Common cross-branch codes (Sem 3–8) — same code for ALL branches ──
        //    These cannot be inferred by pattern (non-standard prefixes like SCK, BOC, RMK).
        //    NOTE: Credits verified against official VTU 22 Scheme documents.
        //    BSCK307: UHV type, 1 credit — VTU explicitly excludes UHV from SGPA in some
        //    regulations. Set IsNonCreditForSgpa=true to be safe; verify against your VTU circular.
        private static readonly Dictionary<string, CreditInfo> _commonCodes =
            new(StringComparer.OrdinalIgnoreCase)
        {
            // Sem 3
            { "BSCK307", new CreditInfo("BSCK307", 1, false,  "Social Connect (UHV - excluded)") },

            // biology with same subject code sometimes 3 and sometimes 2 it should be modiferd by users

            // Sem 4
            { "BBOC407", new CreditInfo("BBOC407", 2, false, "Biology for CS Engineers (BSC)") },
            { "BBOK407", new CreditInfo("BBOK407", 3, false, "Biology for CS Engineers (BSC)") },
            { "BMATEC301", new CreditInfo("BMATEC301", 3, false, "AV Mathematics-III for EC Engineering") },
            { "BEE301", new CreditInfo("BEE301", 3, false, "Engineering Mathematics for EEE") },
            { "BME301", new CreditInfo("BME301", 3, false, "Mechanics of Materials") },
            { "BUHK408", new CreditInfo("BUHK408", 1, false, "Universal Human Values (UHV)") },

            // Sem 5
            { "BRMK557", new CreditInfo("BRMK557", 3, false, "Research Methodology & IPR") },
            // Environmental Studies — common code used across CS family branches in Sem 5
            //{ "BCS508",  new CreditInfo("BCS508",  2, false, "Environmental Studies") },
            { "BEC508",  new CreditInfo("BEC508",  2, false, "Environmental Studies") },
            { "BEE508",  new CreditInfo("BEE508",  2, false, "Environmental Studies") },
            { "BME508",  new CreditInfo("BME508",  2, false, "Environmental Studies") },
            { "BME504L",  new CreditInfo("BME504L",  2, false, "CNC Programming and 3-D Printing lab") },
            { "BESK508",  new CreditInfo("BESK508",  2, false, "Environmental Studies") },

            // Sem 8
            { "BICK860", new CreditInfo("BICK860", 1, true,  "Industry Connect (non-credit)") },
        };

        /// <summary>
        /// Main entry point. Resolves any VTU subject code to credits + SGPA exclusion flag.
        /// </summary>
        public CreditInfo Resolve(string rawCode)
        {
            if (string.IsNullOrWhiteSpace(rawCode))
                return CreditInfo.Unknown(rawCode ?? "");

            var code = rawCode.Trim().ToUpperInvariant();

            // ── Priority 1: Zero-credit mandatory courses ──
            if (_zeroCredit.Contains(code))
                return new CreditInfo(code, 0, true, "Zero-credit mandatory (NSS/PE/Yoga/IKS)");

            // ── Priority 2: Sem 1 & 2 exact codes from DB ──
            var sem12 = _dbContext.SubjectMasters.FirstOrDefault(s => s.SubjectCode == code);
            if (sem12 != null)
                return new CreditInfo(code, sem12.Credits, sem12.IsNonCreditForSgpa, "Sem1/2 fixed code (DB)");

            // ── Priority 3: Common cross-branch codes (in-memory) ──
            if (_commonCodes.TryGetValue(code, out var common))
                return common;

            // ── Priority 4: Pattern-based inference ──
            return InferFromPattern(code);
        }

        private static CreditInfo InferFromPattern(string code)
        {
            var match = _pattern.Match(code);
            if (!match.Success)
                return CreditInfo.Unknown(code);

            string branch = match.Groups[1].Value;   // e.g. "CS", "IS", "CA", "EC"
            bool isLab = match.Groups[2].Value == "L";
            int num = int.Parse(match.Groups[3].Value);
            string suffix = match.Groups[4].Value;   // e.g. "A", "B", "" (empty)

            int sem = num / 100;   // 301 → 3,   701 → 7
            int pos = num % 100;   // 301 → 1,   358 → 58

            // ── Position → Credit map per semester ──
            // All entries verified against official VTU 22 Scheme PDFs for IS and AI branches.
            // The same pattern applies to CS, EC, EE, ME, CE, CV, BT, CH branches.
            var (credits, nonCredit, reason) = (sem, pos, isLab) switch
            {


                // here first true false is based on lab and 2nd true/false based on zerocredit or not
                // ═══════════════ SEMESTER 3 ═══════════════════════════════════
                // Total credit-bearing: ~20 credits
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

    /// <summary>
    /// Result of credit resolution for a single subject code.
    /// </summary>
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
