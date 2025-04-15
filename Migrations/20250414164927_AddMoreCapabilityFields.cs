using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apenew.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreCapabilityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DanskeBankImplementation",
                table: "Capabilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Capabilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Field",
                table: "Capabilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "Capabilities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubcontrolDescription",
                table: "Capabilities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DanskeBankImplementation",
                table: "Capabilities");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Capabilities");

            migrationBuilder.DropColumn(
                name: "Field",
                table: "Capabilities");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "Capabilities");

            migrationBuilder.DropColumn(
                name: "SubcontrolDescription",
                table: "Capabilities");
        }
    }
}
