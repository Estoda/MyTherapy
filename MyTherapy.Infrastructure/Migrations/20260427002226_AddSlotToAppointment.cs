using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTherapy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments",
                column: "SlotId",
                principalTable: "AvailabilitySlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AvailabilitySlots_SlotId",
                table: "Appointments",
                column: "SlotId",
                principalTable: "AvailabilitySlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
