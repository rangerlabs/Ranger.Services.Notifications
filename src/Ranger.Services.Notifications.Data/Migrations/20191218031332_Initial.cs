using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ranger.Services.Notifications.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "data_protection_keys",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    friendly_name = table.Column<string>(nullable: true),
                    xml = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_data_protection_keys", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "frontend_notifications",
                columns: table => new
                {
                    backend_event_key = table.Column<string>(nullable: false),
                    operations_state = table.Column<int>(nullable: false),
                    pusher_event_name = table.Column<string>(nullable: false),
                    text = table.Column<string>(maxLength: 160, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_frontend_notifications", x => new { x.backend_event_key, x.operations_state });
                });

            migrationBuilder.CreateIndex(
                name: "IX_frontend_notifications_pusher_event_name",
                table: "frontend_notifications",
                column: "pusher_event_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "data_protection_keys");

            migrationBuilder.DropTable(
                name: "frontend_notifications");
        }
    }
}
