using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SGPA_CALCULATOR.Migrations
{
    /// <inheritdoc />
    public partial class FinalCleanDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SubjectMasters",
                columns: new[] { "Id", "Credits", "IsNonCreditForSgpa", "Semester", "SubjectCode", "SubjectType" },
                values: new object[] { 47, 4, false, 2, "BMATS201", "Theory" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SubjectMasters",
                keyColumn: "Id",
                keyValue: 47);
        }
    }
}
