using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMonitoringApp.Data.Migrations
{
    public partial class UpdPropsNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BloodSugar",
                table: "BloodSugars",
                newName: "SugarValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SugarValue",
                table: "BloodSugars",
                newName: "BloodSugar");
        }
    }
}
