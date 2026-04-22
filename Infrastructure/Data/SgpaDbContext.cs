using Microsoft.EntityFrameworkCore;

namespace SGPA_CALCULATOR.Infrastructure.Data
{
    public class SgpaDbContext : DbContext
    {
        public DbSet<SubjectMaster> SubjectMasters { get; set; }

        public SgpaDbContext(DbContextOptions<SgpaDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder m)
        {
            base.OnModelCreating(m);

            // ── Only Sem 1 & 2 codes (common to ALL VTU branches) ──
            // These cannot be inferred by pattern — they use special prefixes
            // like BMATS, BPHYS, BCHES, BESCK, BCEDK, BKSK, BIDTK.
            // Total: 21 rows. That's it.

            // ── Only Sem 1 & 2 codes (common to ALL VTU branches) ──
            // These cannot be inferred by pattern — they use special prefixes
            // like BMATS, BPHYS, BCHES, BESCK, BCEDK, BKSK, BIDTK.
            // Total: 21 rows. That's it.
            m.Entity<SubjectMaster>().HasData(

                // ──────────── SEMESTER 1 ────────────
                // Verification: 4+4+4+3+3+1+1 = 20 credits ✓
                new SubjectMaster { Id = 1, SubjectCode = "BMATS101", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 2, SubjectCode = "BPHYS102", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 3, SubjectCode = "BCHES102", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 4, SubjectCode = "BPOPS103", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 5, SubjectCode = "BCEDK103", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                //mech
                new SubjectMaster { Id = 6, SubjectCode = "BMATM101", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 7, SubjectCode = "BPHYM102", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 8, SubjectCode = "BCHEM102", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 9, SubjectCode = "BEMEM103", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 10, SubjectCode = "BCEDK103", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },

                //eee
                new SubjectMaster { Id = 11, SubjectCode = "BMATE101", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 12, SubjectCode = "BPHYE102", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 13, SubjectCode = "BEEE103", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 14, SubjectCode = "BEMEM103", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 15, SubjectCode = "BBEE103", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },

                //cv
                new SubjectMaster { Id = 16, SubjectCode = "BMATC101", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 17, SubjectCode = "BPHYC102", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 18, SubjectCode = "BCIVC103", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 19, SubjectCode = "BESCK104A", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 20, SubjectCode = "BESCK104B", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 21, SubjectCode = "BESCK104C", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 22, SubjectCode = "BESCK104D", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 23, SubjectCode = "BESCK104E", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },

                //emerging tech course
                new SubjectMaster { Id = 24, SubjectCode = "BETCK105A", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 25, SubjectCode = "BETCK105B", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 26, SubjectCode = "BETCK105C", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 27, SubjectCode = "BETCK105D", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 28, SubjectCode = "BETCK105E", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 29, SubjectCode = "BETCK105F", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 30, SubjectCode = "BETCK105G", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 31, SubjectCode = "BETCK105H", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 32, SubjectCode = "BETCK105I", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 33, SubjectCode = "BETCK105J", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 34, SubjectCode = "BPLCK105A", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 35, SubjectCode = "BPLCK105B", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 36, SubjectCode = "BPLCK105C", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 37, SubjectCode = "BPLCK105D", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 38, SubjectCode = "BCEDK109", Credits = 3, SubjectType = "Drawing", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 39, SubjectCode = "BKSK105", Credits = 1, SubjectType = "Language", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 40, SubjectCode = "BIDTK158", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 41, SubjectCode = "BENGK106", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 42, SubjectCode = "BPWSK106", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 43, SubjectCode = "BKSKK107", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 44, SubjectCode = "BKBKK107", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 45, SubjectCode = "BICOK107", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },

                new SubjectMaster { Id = 46, SubjectCode = "BSFH108", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 47, SubjectCode = "BIDTK108", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 48, SubjectCode = "BIDTK158", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 49, SubjectCode = "BSFHK158", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },


                // ──────────── SEMESTER 2  ────────────
                // Verification: 4+4+4+3+3+1+1 = 20 credits ✓
                new SubjectMaster { Id = 50, SubjectCode = "BMATS201", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 51, SubjectCode = "BPHYS202", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 52, SubjectCode = "BCHES202", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 53, SubjectCode = "BCEDK203", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },

                //mech
                new SubjectMaster { Id = 54, SubjectCode = "BMATM201", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 55, SubjectCode = "BPHYM202", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 56, SubjectCode = "BCHEM202", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 57, SubjectCode = "BEMEM203", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 58, SubjectCode = "BCEDK203", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 59, SubjectCode = "BEME203", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },

                //eee
                new SubjectMaster { Id = 60, SubjectCode = "BMATE201", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 61, SubjectCode = "BPHYE202", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 62, SubjectCode = "BEEE203", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 63, SubjectCode = "BEMEM203", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 64, SubjectCode = "BBEE203", Credits = 3, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },

                //cv
                new SubjectMaster { Id = 65, SubjectCode = "BMATC201", Credits = 4, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 66, SubjectCode = "BPHYC202", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 67, SubjectCode = "BCIVC203", Credits = 4, SubjectType = "Integrated", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 68, SubjectCode = "BESCK204A", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 69, SubjectCode = "BESCK204B", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 70, SubjectCode = "BESCK204C", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 71, SubjectCode = "BESCK204D", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 72, SubjectCode = "BESCK204E", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },

                //emerging tech course
                new SubjectMaster { Id = 73, SubjectCode = "BETCK205A", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 74, SubjectCode = "BETCK205B", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 75, SubjectCode = "BETCK205C", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 76, SubjectCode = "BETCK205D", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 77, SubjectCode = "BETCK205E", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 78, SubjectCode = "BETCK205F", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 79, SubjectCode = "BETCK205G", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 80, SubjectCode = "BETCK205H", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 81, SubjectCode = "BETCK205I", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 82, SubjectCode = "BETCK105J", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 83, SubjectCode = "22ETC15J", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },




                new SubjectMaster { Id = 84, SubjectCode = "BPLCK205A", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 85, SubjectCode = "BPLCK205B", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 86, SubjectCode = "BPLCK205C", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 87, SubjectCode = "BPLCK205D", Credits = 3, SubjectType = "Theory", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 88, SubjectCode = "BCEDK209", Credits = 3, SubjectType = "Drawing", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 89, SubjectCode = "BKSK205", Credits = 1, SubjectType = "Language", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 90, SubjectCode = "BIDTK258", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 91, SubjectCode = "BENGK206", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 92, SubjectCode = "BPWSK206", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 93, SubjectCode = "BKSKK207", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 94, SubjectCode = "BKBKK207", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 95, SubjectCode = "BICOK207", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },



                new SubjectMaster { Id = 96, SubjectCode = "BSFH208", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 97, SubjectCode = "BIDTK208", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },


                new SubjectMaster { Id = 98, SubjectCode = "KIDTK258", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false },
                new SubjectMaster { Id = 99, SubjectCode = "BSFHK258", Credits = 1, SubjectType = "Workshop", Semester = 1, IsNonCreditForSgpa = false }
                );



               
        }
    }
}
