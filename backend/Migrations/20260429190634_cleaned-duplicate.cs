using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SGPA_CALCULATOR.Migrations
{
    /// <inheritdoc />
    public partial class cleanedduplicate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 4, "BMATE101", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BPHYE102", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 12,
                column: "SubjectCode",
                value: "BEEE103");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BBEE103", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 4, "BMATC101", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 4, "BPHYC102", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BCIVC103", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BESCK104A", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BESCK104B", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 19,
                column: "SubjectCode",
                value: "BESCK104C");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 20,
                column: "SubjectCode",
                value: "BESCK104D");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 21,
                column: "SubjectCode",
                value: "BESCK104E");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 22,
                column: "SubjectCode",
                value: "BETCK105A");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 23,
                column: "SubjectCode",
                value: "BETCK105B");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 24,
                column: "SubjectCode",
                value: "BETCK105C");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 25,
                column: "SubjectCode",
                value: "BETCK105D");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 26,
                column: "SubjectCode",
                value: "BETCK105E");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 27,
                column: "SubjectCode",
                value: "BETCK105F");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 28,
                column: "SubjectCode",
                value: "BETCK105G");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 29,
                column: "SubjectCode",
                value: "BETCK105H");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 30,
                column: "SubjectCode",
                value: "BETCK105I");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 31,
                column: "SubjectCode",
                value: "BETCK105J");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 32,
                column: "SubjectCode",
                value: "BPLCK105A");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 33,
                column: "SubjectCode",
                value: "BPLCK105B");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 34,
                column: "SubjectCode",
                value: "BPLCK105C");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 35,
                column: "SubjectCode",
                value: "BPLCK105D");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BCEDK109", "Drawing" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BKSK105", "Language" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BIDTK158", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BENGK106", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 40,
                column: "SubjectCode",
                value: "BPWSK106");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 41,
                column: "SubjectCode",
                value: "BKSKK107");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 42,
                column: "SubjectCode",
                value: "BKBKK107");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 43,
                column: "SubjectCode",
                value: "BICOK107");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 44,
                column: "SubjectCode",
                value: "BSFH108");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 45,
                column: "SubjectCode",
                value: "BIDTK108");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 46,
                column: "SubjectCode",
                value: "BSFHK158");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 2, "BMATS201", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 2, "BPHYS202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 2, "BCHES202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 2, "BCEDK203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 2, "BMATM201", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BPHYM202" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 2, "BCHEM202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 2, "BEMEM203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 2, "BEME203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 2, "BMATE201", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 2, "BPHYE202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 2, "BEEE203", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BBEE203" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BMATC201" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BPHYC202" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BCIVC203" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 2, "BESCK204A", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 2, "BESCK204B", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "Credits", "Semester", "SubjectCode" },
                values: new object[] { 3, 2, "BESCK204C" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 2, "BESCK204D", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 2, "BESCK204E", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205A" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205B" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205C" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205D" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205E" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205F" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205G" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205H" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BETCK205I" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "22ETC15J" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BPLCK205A" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BPLCK205B" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BPLCK205C" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BPLCK205D" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 2, "BCEDK209", "Drawing" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 2, "BKSK205", "Language" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 2, "BIDTK258", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 2, "BENGK206", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 2, "BPWSK206", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 2, "BKSKK207", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 2, "BKBKK207", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 2, "BICOK207", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BSFH208" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BIDTK208" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "KIDTK258" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 2, "BSFHK258" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BCEDK103", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BMATE101", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 12,
                column: "SubjectCode",
                value: "BPHYE102");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 4, "BEEE103", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BEMEM103", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BBEE103", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BMATC101", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 4, "BPHYC102", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 4, "BCIVC103", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 19,
                column: "SubjectCode",
                value: "BESCK104A");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 20,
                column: "SubjectCode",
                value: "BESCK104B");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 21,
                column: "SubjectCode",
                value: "BESCK104C");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 22,
                column: "SubjectCode",
                value: "BESCK104D");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 23,
                column: "SubjectCode",
                value: "BESCK104E");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 24,
                column: "SubjectCode",
                value: "BETCK105A");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 25,
                column: "SubjectCode",
                value: "BETCK105B");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 26,
                column: "SubjectCode",
                value: "BETCK105C");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 27,
                column: "SubjectCode",
                value: "BETCK105D");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 28,
                column: "SubjectCode",
                value: "BETCK105E");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 29,
                column: "SubjectCode",
                value: "BETCK105F");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 30,
                column: "SubjectCode",
                value: "BETCK105G");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 31,
                column: "SubjectCode",
                value: "BETCK105H");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 32,
                column: "SubjectCode",
                value: "BETCK105I");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 33,
                column: "SubjectCode",
                value: "BETCK105J");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 34,
                column: "SubjectCode",
                value: "BPLCK105A");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 35,
                column: "SubjectCode",
                value: "BPLCK105B");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BPLCK105C", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BPLCK105D", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Credits", "SubjectCode", "SubjectType" },
                values: new object[] { 3, "BCEDK109", "Drawing" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "SubjectCode", "SubjectType" },
                values: new object[] { "BKSK105", "Language" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 40,
                column: "SubjectCode",
                value: "BIDTK158");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 41,
                column: "SubjectCode",
                value: "BENGK106");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 42,
                column: "SubjectCode",
                value: "BPWSK106");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 43,
                column: "SubjectCode",
                value: "BKSKK107");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 44,
                column: "SubjectCode",
                value: "BKBKK107");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 45,
                column: "SubjectCode",
                value: "BICOK107");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 46,
                column: "SubjectCode",
                value: "BSFH108");

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 1, "BIDTK108", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 1, "BIDTK158", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, 1, "BSFHK158", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 1, "BMATS201", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BPHYS202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BCHES202" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BCEDK203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 1, "BMATM201", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 1, "BPHYM202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BCHEM202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BEMEM203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BCEDK203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BEME203" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BMATE201" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BPHYE202" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BEEE203" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BEMEM203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BBEE203", "Workshop" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "Credits", "Semester", "SubjectCode" },
                values: new object[] { 4, 1, "BMATC201" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 1, "BPHYC202", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 4, 1, "BCIVC203", "Integrated" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BESCK204A" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BESCK204B" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BESCK204C" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BESCK204D" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BESCK204E" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205A" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205B" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205C" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205D" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205E" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205F" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205G" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205H" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BETCK205I" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BETCK105J", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "22ETC15J", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BPLCK205A", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BPLCK205B", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BPLCK205C", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BPLCK205D", "Theory" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "Credits", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 3, 1, "BCEDK209", "Drawing" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 1, "BKSK205", "Language" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BIDTK258" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BENGK206" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BPWSK206" });

            migrationBuilder.UpdateData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "Semester", "SubjectCode" },
                values: new object[] { 1, "BKSKK207" });

            migrationBuilder.InsertData(
                table: "SubjectMasters",
                columns: new[] { "Id", "Credits", "IsNonCreditForSgpa", "Semester", "SubjectCode", "SubjectType" },
                values: new object[,]
                {
                    { 94, 1, false, 1, "BKBKK207", "Workshop" },
                    { 95, 1, false, 1, "BICOK207", "Workshop" },
                    { 96, 1, false, 1, "BSFH208", "Workshop" },
                    { 97, 1, false, 1, "BIDTK208", "Workshop" },
                    { 98, 1, false, 1, "KIDTK258", "Workshop" },
                    { 99, 1, false, 1, "BSFHK258", "Workshop" }
                });
        }
    }
}
