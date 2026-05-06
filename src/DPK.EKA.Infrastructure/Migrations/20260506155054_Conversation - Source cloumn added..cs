using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPK.EKA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConversationSourcecloumnadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Conversations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Conversations");
        }
    }
}
