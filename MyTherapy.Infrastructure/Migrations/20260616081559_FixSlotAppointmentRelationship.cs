using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTherapy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSlotAppointmentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
