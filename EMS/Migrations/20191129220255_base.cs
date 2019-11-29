using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EMS.Migrations
{
    public partial class @base : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Employee",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Employee",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Employee");
        }
    }
}
