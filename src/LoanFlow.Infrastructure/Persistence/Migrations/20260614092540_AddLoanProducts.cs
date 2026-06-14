using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MinimumLoanAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaximumLoanAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AnnualInterestRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    MinimumTenureMonths = table.Column<int>(type: "int", nullable: false),
                    MaximumTenureMonths = table.Column<int>(type: "int", nullable: false),
                    MinimumMonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaximumDebtToIncomeRatio = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanProducts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_ProductCode",
                table: "LoanProducts",
                column: "ProductCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanProducts");
        }
    }
}
