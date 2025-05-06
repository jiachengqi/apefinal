using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apenew.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTypeAndImpl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Control",
                table: "Capabilities");

            migrationBuilder.DropColumn(
                name: "DanskeBankImplementation",
                table: "Capabilities");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Capabilities");

            migrationBuilder.RenameColumn(
                name: "Field",
                table: "Capabilities",
                newName: "Cluster");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cluster",
                table: "Capabilities",
                newName: "Field");

            migrationBuilder.AddColumn<string>(
                name: "Control",
                table: "Capabilities",
                type: "text",
                nullable: true);

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
        }
    }
}
