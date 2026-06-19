using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTherapy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiTaskIdToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiTaskId",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiTaskId",
                table: "Sessions");
        }
    }
}
