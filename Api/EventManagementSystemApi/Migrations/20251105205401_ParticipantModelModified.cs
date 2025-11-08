using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystemApi.Migrations
{
    /// <inheritdoc />
    public partial class ParticipantModelModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Participants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Participants");
        }
    }
}
