using Microsoft.EntityFrameworkCore.Migrations;

namespace CodeCityCrew.Settings.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    EnvironmentName = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    AssemblyName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => new { x.Id, x.EnvironmentName });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Setting");
        }
    }
}
