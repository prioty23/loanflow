using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanFlow.Infrastructure.Persistence.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260615070500_BackfillEmploymentAndFinancialData")]
public partial class BackfillEmploymentAndFinancialData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE [LoanApplicationFinancialProfiles]
            SET [HousingExpense] = [TotalMonthlyExpenses]
            WHERE [HousingExpense] = 0 AND [TotalMonthlyExpenses] > 0;
            """);

        migrationBuilder.Sql("""
            UPDATE [LoanApplicationFinancialProfiles]
            SET [ExistingMonthlyEmi] = [OtherLiabilities],
                [OtherLiabilities] = 0
            WHERE [ExistingMonthlyEmi] = 0 AND [OtherLiabilities] > 0;
            """);

        migrationBuilder.Sql("""
            UPDATE [LoanApplicationFinancialProfiles]
            SET [MonthlyNetSalary] = [TotalMonthlyIncome] - [OtherMonthlyIncome]
            WHERE [MonthlyNetSalary] = 0 AND [TotalMonthlyIncome] >= [OtherMonthlyIncome];
            """);

        migrationBuilder.Sql("""
            UPDATE [LoanApplicationFinancialProfiles]
            SET [EmploymentStartDate] = CAST(GETDATE() AS date)
            WHERE [EmploymentStartDate] = '0001-01-01';
            """);

        migrationBuilder.Sql("""
            UPDATE [LoanApplicationFinancialProfiles]
            SET [TotalMonthlyExpenses] = [HousingExpense] + [LivingExpense] + [ExistingMonthlyEmi] + [OtherLiabilities],
                [CalculatedDisposableIncome] = [TotalMonthlyIncome] - ([HousingExpense] + [LivingExpense] + [ExistingMonthlyEmi] + [OtherLiabilities]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
