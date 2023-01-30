using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO AspNetRoles ([Id], [Name], [NormalizedName])
            VALUES ('2082366a-c161-41a0-a86b-823c03ed4dc9', 'Admin', 'Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM AspNetRoles WHERE [id] =  '2082366a-c161-41a0-a86b-823c03ed4dc9'");
        }
    }
}
