using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResourceBooking.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceInfo_ResourceTypeInfo_ReourceTypeId",
                table: "ResourceInfo");

            migrationBuilder.DropIndex(
                name: "IX_ResourceInfo_ReourceTypeId",
                table: "ResourceInfo");

            migrationBuilder.DropColumn(
                name: "ReourceTypeId",
                table: "ResourceInfo");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceInfo_ResourceTypeId",
                table: "ResourceInfo",
                column: "ResourceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceInfo_ResourceTypeInfo_ResourceTypeId",
                table: "ResourceInfo",
                column: "ResourceTypeId",
                principalTable: "ResourceTypeInfo",
                principalColumn: "ResourceTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceInfo_ResourceTypeInfo_ResourceTypeId",
                table: "ResourceInfo");

            migrationBuilder.DropIndex(
                name: "IX_ResourceInfo_ResourceTypeId",
                table: "ResourceInfo");

            migrationBuilder.AddColumn<int>(
                name: "ReourceTypeId",
                table: "ResourceInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ResourceInfo_ReourceTypeId",
                table: "ResourceInfo",
                column: "ReourceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceInfo_ResourceTypeInfo_ReourceTypeId",
                table: "ResourceInfo",
                column: "ReourceTypeId",
                principalTable: "ResourceTypeInfo",
                principalColumn: "ResourceTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
