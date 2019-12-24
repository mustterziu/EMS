using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EMS.Migrations
{
    public partial class payments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payment",
                table: "Attendance",
                newName: "payment");

            migrationBuilder.AddColumn<string>(
                name: "Holiday",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Employee",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Attendance",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeRroga",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    Paga = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRroga", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeeId = table.Column<int>(nullable: true),
                    startDate = table.Column<DateTime>(nullable: false),
                    endDate = table.Column<DateTime>(nullable: false),
                    paymentNeto = table.Column<float>(nullable: false),
                    paymentBruto = table.Column<float>(nullable: false),
                    paid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Employee_employeeId",
                        column: x => x.employeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_PaymentId",
                table: "Attendance",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_employeeId",
                table: "Payment",
                column: "employeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Payment_PaymentId",
                table: "Attendance",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Payment_PaymentId",
                table: "Attendance");

            migrationBuilder.DropTable(
                name: "EmployeeRroga");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_PaymentId",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "Holiday",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Attendance");

            migrationBuilder.RenameColumn(
                name: "payment",
                table: "Attendance",
                newName: "Payment");
        }
    }
}
