using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apenew.Migrations
{
    /// <inheritdoc />
    public partial class AddCrit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Criticality",
                table: "Assessments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Criticality",
                table: "Assessments");
        }
    }
}
