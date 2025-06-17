using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apenew.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewerCommentToCapability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewerComment",
                table: "Capabilities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewerComment",
                table: "Capabilities");
        }
    }
}
