using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apenew.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Pins_CapabilityId",
                table: "Pins",
                column: "CapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pins_Capabilities_CapabilityId",
                table: "Pins",
                column: "CapabilityId",
                principalTable: "Capabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pins_Capabilities_CapabilityId",
                table: "Pins");

            migrationBuilder.DropIndex(
                name: "IX_Pins_CapabilityId",
                table: "Pins");
        }
    }
}
