using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGPA_CALCULATOR.Migrations
{
    /// <inheritdoc />
    public partial class updatefinaldb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    SubjectType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    IsNonCreditForSgpa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectMasters", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SubjectMasters",
                columns: new[] { "Id", "Credits", "IsNonCreditForSgpa", "Semester", "SubjectCode", "SubjectType" },
                values: new object[,]
                {
                    { 1, 4, false, 1, "BMATS101", "Theory" },
                    { 2, 4, false, 1, "BPHYS102", "Integrated" },
                    { 3, 4, false, 1, "BCHES102", "Integrated" },
                    { 4, 3, false, 1, "BPOPS103", "Workshop" },
                    { 5, 3, false, 1, "BCEDK103", "Workshop" },
                    { 6, 4, false, 1, "BMATM101", "Theory" },
                    { 7, 4, false, 1, "BPHYM102", "Integrated" },
                    { 8, 4, false, 1, "BCHEM102", "Integrated" },
                    { 9, 3, false, 1, "BEMEM103", "Workshop" },
                    { 10, 4, false, 1, "BMATE101", "Theory" },
                    { 11, 4, false, 1, "BPHYE102", "Integrated" },
                    { 12, 4, false, 1, "BEEE103", "Integrated" },
                    { 13, 3, false, 1, "BBEE103", "Workshop" },
                    { 14, 4, false, 1, "BMATC101", "Theory" },
                    { 15, 4, false, 1, "BPHYC102", "Integrated" },
                    { 16, 4, false, 1, "BCIVC103", "Integrated" },
                    { 17, 3, false, 1, "BESCK104A", "Theory" },
                    { 18, 3, false, 1, "BESCK104B", "Theory" },
                    { 19, 3, false, 1, "BESCK104C", "Theory" },
                    { 20, 3, false, 1, "BESCK104D", "Theory" },
                    { 21, 3, false, 1, "BESCK104E", "Theory" },
                    { 22, 3, false, 1, "BETCK105A", "Theory" },
                    { 23, 3, false, 1, "BETCK105B", "Theory" },
                    { 24, 3, false, 1, "BETCK105C", "Theory" },
                    { 25, 3, false, 1, "BETCK105D", "Theory" },
                    { 26, 3, false, 1, "BETCK105E", "Theory" },
                    { 27, 3, false, 1, "BETCK105F", "Theory" },
                    { 28, 3, false, 1, "BETCK105G", "Theory" },
                    { 29, 3, false, 1, "BETCK105H", "Theory" },
                    { 30, 3, false, 1, "BETCK105I", "Theory" },
                    { 31, 3, false, 1, "BETCK105J", "Theory" },
                    { 32, 3, false, 1, "BPLCK105A", "Theory" },
                    { 33, 3, false, 1, "BPLCK105B", "Theory" },
                    { 34, 3, false, 1, "BPLCK105C", "Theory" },
                    { 35, 3, false, 1, "BPLCK105D", "Theory" },
                    { 36, 3, false, 1, "BCEDK109", "Drawing" },
                    { 37, 1, false, 1, "BKSK105", "Language" },
                    { 38, 1, false, 1, "BIDTK158", "Workshop" },
                    { 39, 1, false, 1, "BENGK106", "Workshop" },
                    { 40, 1, false, 1, "BPWSK106", "Workshop" },
                    { 41, 1, false, 1, "BKSKK107", "Workshop" },
                    { 42, 1, false, 1, "BKBKK107", "Workshop" },
                    { 43, 1, false, 1, "BICOK107", "Workshop" },
                    { 44, 1, false, 1, "BSFH108", "Workshop" },
                    { 45, 1, false, 1, "BIDTK108", "Workshop" },
                    { 46, 1, false, 1, "BSFHK158", "Workshop" },
                    { 47, 4, false, 2, "BMATS201", "Theory" },
                    { 48, 4, false, 2, "BPHYS202", "Integrated" },
                    { 49, 4, false, 2, "BCHES202", "Integrated" },
                    { 50, 3, false, 2, "BCEDK203", "Workshop" },
                    { 51, 4, false, 2, "BMATM201", "Theory" },
                    { 52, 4, false, 2, "BPHYM202", "Integrated" },
                    { 53, 4, false, 2, "BCHEM202", "Integrated" },
                    { 54, 3, false, 2, "BEMEM203", "Workshop" },
                    { 55, 3, false, 2, "BEME203", "Workshop" },
                    { 56, 4, false, 2, "BMATE201", "Theory" },
                    { 57, 4, false, 2, "BPHYE202", "Integrated" },
                    { 58, 4, false, 2, "BEEE203", "Integrated" },
                    { 59, 3, false, 2, "BBEE203", "Workshop" },
                    { 60, 4, false, 2, "BMATC201", "Theory" },
                    { 61, 4, false, 2, "BPHYC202", "Integrated" },
                    { 62, 4, false, 2, "BCIVC203", "Integrated" },
                    { 63, 3, false, 2, "BESCK204A", "Theory" },
                    { 64, 3, false, 2, "BESCK204B", "Theory" },
                    { 65, 3, false, 2, "BESCK204C", "Theory" },
                    { 66, 3, false, 2, "BESCK204D", "Theory" },
                    { 67, 3, false, 2, "BESCK204E", "Theory" },
                    { 68, 3, false, 2, "BETCK205A", "Theory" },
                    { 69, 3, false, 2, "BETCK205B", "Theory" },
                    { 70, 3, false, 2, "BETCK205C", "Theory" },
                    { 71, 3, false, 2, "BETCK205D", "Theory" },
                    { 72, 3, false, 2, "BETCK205E", "Theory" },
                    { 73, 3, false, 2, "BETCK205F", "Theory" },
                    { 74, 3, false, 2, "BETCK205G", "Theory" },
                    { 75, 3, false, 2, "BETCK205H", "Theory" },
                    { 76, 3, false, 2, "BETCK205I", "Theory" },
                    { 77, 3, false, 2, "22ETC15J", "Theory" },
                    { 78, 3, false, 2, "BPLCK205A", "Theory" },
                    { 79, 3, false, 2, "BPLCK205B", "Theory" },
                    { 80, 3, false, 2, "BPLCK205C", "Theory" },
                    { 81, 3, false, 2, "BPLCK205D", "Theory" },
                    { 82, 3, false, 2, "BCEDK209", "Drawing" },
                    { 83, 1, false, 2, "BKSK205", "Language" },
                    { 84, 1, false, 2, "BIDTK258", "Workshop" },
                    { 85, 1, false, 2, "BENGK206", "Workshop" },
                    { 86, 1, false, 2, "BPWSK206", "Workshop" },
                    { 87, 1, false, 2, "BKSKK207", "Workshop" },
                    { 88, 1, false, 2, "BKBKK207", "Workshop" },
                    { 89, 1, false, 2, "BICOK207", "Workshop" },
                    { 90, 1, false, 2, "BSFH208", "Workshop" },
                    { 91, 1, false, 2, "BIDTK208", "Workshop" },
                    { 92, 1, false, 2, "KIDTK258", "Workshop" },
                    { 93, 1, false, 2, "BSFHK258", "Workshop" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectMasters");
        }
    }
}
