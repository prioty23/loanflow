using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanApplicationReadOnlyViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "LoanApplications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [LoanApplications]
                SET [CreatedAtUtc] = COALESCE([SubmittedAtUtc], [UpdatedAtUtc], SYSUTCDATETIME())
                WHERE [CreatedAtUtc] IS NULL;
                """);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "LoanApplications",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "LoanApplications");
        }
    }
}
