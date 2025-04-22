using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apenew.Migrations
{
    /// <inheritdoc />
    public partial class AddImageSizeToAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ImageHeight",
                table: "Assessments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ImageWidth",
                table: "Assessments",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageHeight",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "ImageWidth",
                table: "Assessments");
        }
    }
}
