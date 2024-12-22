using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DistributedLoggingSystem.EF.Migrations
{
    /// <inheritdoc />
    public partial class addPathFileField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogFilePath",
                table: "LogEntries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogFilePath",
                table: "LogEntries");
        }
    }
}
