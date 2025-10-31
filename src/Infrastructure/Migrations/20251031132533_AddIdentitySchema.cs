using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "timestamp",
                table: "Cheeps",
                newName: "TimeStamp");

            migrationBuilder.RenameColumn(
                name: "text",
                table: "Cheeps",
                newName: "Text");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Authors",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Authors",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Cheeps",
                newName: "timestamp");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Cheeps",
                newName: "text");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Authors",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Authors",
                newName: "email");
        }
    }
}
