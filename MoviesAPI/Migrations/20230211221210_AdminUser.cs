using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'a7bf105a-6449-4a06-a58e-0fff957f1260', N'admin', N'ADMIN', N'admin@email.com', N'ADMIN@ADMIN.COM', 0, N'AQAAAAEAACcQAAAAEIX1fJU1ECQGbh8mXYj3AhFne/ywOvz7Q8Ata5SjAo5zybcZyJb0hPqA0jMsnPlUzQ==', N'Y5A76AU4QT5HGYPYNZYDKIETF3BIUU65', N'1cf8f3b7-9047-46ca-a38c-b293c1d180a0', NULL, 0, 0, NULL, 1, 0)
                GO
                SET IDENTITY_INSERT [dbo].[AspNetUserClaims] ON 
                GO
                INSERT [dbo].[AspNetUserClaims] ([Id], [UserId], [ClaimType], [ClaimValue]) VALUES (2, N'a7bf105a-6449-4a06-a58e-0fff957f1260', N'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', N'Admin')
                GO
                SET IDENTITY_INSERT [dbo].[AspNetUserClaims] OFF
                GO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM [dbo].[AspNetUsers] WHERE [Id] = '6613a578-2d7a-4c77-8767-2be921e2307c';
                DELETE FROM [dbo].[AspNetUserClaims] WHERE Id] = 2");
        }
    }
}
