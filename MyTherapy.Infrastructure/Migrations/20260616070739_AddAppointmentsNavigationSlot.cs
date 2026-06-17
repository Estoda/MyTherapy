using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTherapy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentsNavigationSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LicenseDocumentPath",
                table: "Therapists",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "AvailabilitySlotId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AvailabilitySlotId",
                table: "Appointments",
                column: "AvailabilitySlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AvailabilitySlots_AvailabilitySlotId",
                table: "Appointments",
                column: "AvailabilitySlotId",
                principalTable: "AvailabilitySlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AvailabilitySlots_AvailabilitySlotId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AvailabilitySlotId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AvailabilitySlotId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseDocumentPath",
                table: "Therapists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
