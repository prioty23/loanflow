using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmploymentAndFinancialDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MonthlyIncome",
                table: "LoanApplicationFinancialProfiles",
                newName: "TotalMonthlyIncome");

            migrationBuilder.RenameColumn(
                name: "MonthlyHousingExpense",
                table: "LoanApplicationFinancialProfiles",
                newName: "TotalMonthlyExpenses");

            migrationBuilder.RenameColumn(
                name: "MonthlyDebtPayments",
                table: "LoanApplicationFinancialProfiles",
                newName: "OtherLiabilities");

            migrationBuilder.RenameColumn(
                name: "EmployerName",
                table: "LoanApplicationFinancialProfiles",
                newName: "EmployerOrBusinessName");

            migrationBuilder.AddColumn<decimal>(
                name: "CalculatedDisposableIncome",
                table: "LoanApplicationFinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EmploymentStartDate",
                table: "LoanApplicationFinancialProfiles",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "ExistingMonthlyEmi",
                table: "LoanApplicationFinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HousingExpense",
                table: "LoanApplicationFinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "LoanApplicationFinancialProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "LivingExpense",
                table: "LoanApplicationFinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyNetSalary",
                table: "LoanApplicationFinancialProfiles",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculatedDisposableIncome",
                table: "LoanApplicationFinancialProfiles");

            migrationBuilder.DropColumn(
                name: "EmploymentStartDate",
                table: "LoanApplicationFinancialProfiles");

            migrationBuilder.DropColumn(
                name: "ExistingMonthlyEmi",
                table: "LoanApplicationFinancialProfiles");

            migrationBuilder.DropColumn(
                name: "HousingExpense",
                table: "LoanApplicationFinancialProfiles");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "LoanApplicationFinancialProfiles");

            migrationBuilder.DropColumn(
                name: "LivingExpense",
                table: "LoanApplicationFinancialProfiles");

            migrationBuilder.DropColumn(
                name: "MonthlyNetSalary",
                table: "LoanApplicationFinancialProfiles");

            migrationBuilder.RenameColumn(
                name: "TotalMonthlyIncome",
                table: "LoanApplicationFinancialProfiles",
                newName: "MonthlyIncome");

            migrationBuilder.RenameColumn(
                name: "TotalMonthlyExpenses",
                table: "LoanApplicationFinancialProfiles",
                newName: "MonthlyHousingExpense");

            migrationBuilder.RenameColumn(
                name: "OtherLiabilities",
                table: "LoanApplicationFinancialProfiles",
                newName: "MonthlyDebtPayments");

            migrationBuilder.RenameColumn(
                name: "EmployerOrBusinessName",
                table: "LoanApplicationFinancialProfiles",
                newName: "EmployerName");
        }
    }
}
