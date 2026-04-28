using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGPA_CALCULATOR.Migrations
{
    /// <inheritdoc />
    public partial class upadtedatabase : Migration
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
                    { 10, 3, false, 1, "BCEDK103", "Workshop" },
                    { 11, 4, false, 1, "BMATE101", "Theory" },
                    { 12, 4, false, 1, "BPHYE102", "Integrated" },
                    { 13, 4, false, 1, "BEEE103", "Integrated" },
                    { 14, 3, false, 1, "BEMEM103", "Workshop" },
                    { 15, 3, false, 1, "BBEE103", "Workshop" },
                    { 16, 4, false, 1, "BMATC101", "Theory" },
                    { 17, 4, false, 1, "BPHYC102", "Integrated" },
                    { 18, 4, false, 1, "BCIVC103", "Integrated" },
                    { 19, 3, false, 1, "BESCK104A", "Theory" },
                    { 20, 3, false, 1, "BESCK104B", "Theory" },
                    { 21, 3, false, 1, "BESCK104C", "Theory" },
                    { 22, 3, false, 1, "BESCK104D", "Theory" },
                    { 23, 3, false, 1, "BESCK104E", "Theory" },
                    { 24, 3, false, 1, "BETCK105A", "Theory" },
                    { 25, 3, false, 1, "BETCK105B", "Theory" },
                    { 26, 3, false, 1, "BETCK105C", "Theory" },
                    { 27, 3, false, 1, "BETCK105D", "Theory" },
                    { 28, 3, false, 1, "BETCK105E", "Theory" },
                    { 29, 3, false, 1, "BETCK105F", "Theory" },
                    { 30, 3, false, 1, "BETCK105G", "Theory" },
                    { 31, 3, false, 1, "BETCK105H", "Theory" },
                    { 32, 3, false, 1, "BETCK105I", "Theory" },
                    { 33, 3, false, 1, "BETCK105J", "Theory" },
                    { 34, 3, false, 1, "BPLCK105A", "Theory" },
                    { 35, 3, false, 1, "BPLCK105B", "Theory" },
                    { 36, 3, false, 1, "BPLCK105C", "Theory" },
                    { 37, 3, false, 1, "BPLCK105D", "Theory" },
                    { 38, 3, false, 1, "BCEDK109", "Drawing" },
                    { 39, 1, false, 1, "BKSK105", "Language" },
                    { 40, 1, false, 1, "BIDTK158", "Workshop" },
                    { 41, 1, false, 1, "BENGK106", "Workshop" },
                    { 42, 1, false, 1, "BPWSK106", "Workshop" },
                    { 43, 1, false, 1, "BKSKK107", "Workshop" },
                    { 44, 1, false, 1, "BKBKK107", "Workshop" },
                    { 45, 1, false, 1, "BICOK107", "Workshop" },
                    { 46, 1, false, 1, "BSFH108", "Workshop" },
                    { 47, 1, false, 1, "BIDTK108", "Workshop" },
                    { 48, 1, false, 1, "BIDTK158", "Workshop" },
                    { 49, 1, false, 1, "BSFHK158", "Workshop" },
                    { 50, 4, false, 1, "BMATS201", "Theory" },
                    { 51, 4, false, 1, "BPHYS202", "Integrated" },
                    { 52, 4, false, 1, "BCHES202", "Integrated" },
                    { 53, 3, false, 1, "BCEDK203", "Workshop" },
                    { 54, 4, false, 1, "BMATM201", "Theory" },
                    { 55, 4, false, 1, "BPHYM202", "Integrated" },
                    { 56, 4, false, 1, "BCHEM202", "Integrated" },
                    { 57, 3, false, 1, "BEMEM203", "Workshop" },
                    { 58, 3, false, 1, "BCEDK203", "Workshop" },
                    { 59, 3, false, 1, "BEME203", "Workshop" },
                    { 60, 4, false, 1, "BMATE201", "Theory" },
                    { 61, 4, false, 1, "BPHYE202", "Integrated" },
                    { 62, 4, false, 1, "BEEE203", "Integrated" },
                    { 63, 3, false, 1, "BEMEM203", "Workshop" },
                    { 64, 3, false, 1, "BBEE203", "Workshop" },
                    { 65, 4, false, 1, "BMATC201", "Theory" },
                    { 66, 4, false, 1, "BPHYC202", "Integrated" },
                    { 67, 4, false, 1, "BCIVC203", "Integrated" },
                    { 68, 3, false, 1, "BESCK204A", "Theory" },
                    { 69, 3, false, 1, "BESCK204B", "Theory" },
                    { 70, 3, false, 1, "BESCK204C", "Theory" },
                    { 71, 3, false, 1, "BESCK204D", "Theory" },
                    { 72, 3, false, 1, "BESCK204E", "Theory" },
                    { 73, 3, false, 1, "BETCK205A", "Theory" },
                    { 74, 3, false, 1, "BETCK205B", "Theory" },
                    { 75, 3, false, 1, "BETCK205C", "Theory" },
                    { 76, 3, false, 1, "BETCK205D", "Theory" },
                    { 77, 3, false, 1, "BETCK205E", "Theory" },
                    { 78, 3, false, 1, "BETCK205F", "Theory" },
                    { 79, 3, false, 1, "BETCK205G", "Theory" },
                    { 80, 3, false, 1, "BETCK205H", "Theory" },
                    { 81, 3, false, 1, "BETCK205I", "Theory" },
                    { 82, 3, false, 1, "BETCK105J", "Theory" },
                    { 83, 3, false, 1, "22ETC15J", "Theory" },
                    { 84, 3, false, 1, "BPLCK205A", "Theory" },
                    { 85, 3, false, 1, "BPLCK205B", "Theory" },
                    { 86, 3, false, 1, "BPLCK205C", "Theory" },
                    { 87, 3, false, 1, "BPLCK205D", "Theory" },
                    { 88, 3, false, 1, "BCEDK209", "Drawing" },
                    { 89, 1, false, 1, "BKSK205", "Language" },
                    { 90, 1, false, 1, "BIDTK258", "Workshop" },
                    { 91, 1, false, 1, "BENGK206", "Workshop" },
                    { 92, 1, false, 1, "BPWSK206", "Workshop" },
                    { 93, 1, false, 1, "BKSKK207", "Workshop" },
                    { 94, 1, false, 1, "BKBKK207", "Workshop" },
                    { 95, 1, false, 1, "BICOK207", "Workshop" },
                    { 96, 1, false, 1, "BSFH208", "Workshop" },
                    { 97, 1, false, 1, "BIDTK208", "Workshop" },
                    { 98, 1, false, 1, "KIDTK258", "Workshop" },
                    { 99, 1, false, 1, "BSFHK258", "Workshop" }
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
