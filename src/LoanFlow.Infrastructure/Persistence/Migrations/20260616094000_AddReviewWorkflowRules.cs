using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanFlow.Infrastructure.Persistence.Migrations
{
    public partial class AddReviewWorkflowRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedLoanOfficerId",
                table: "LoanApplications",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoanApplicationDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplicationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanApplicationDocuments_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_AssignedLoanOfficerId",
                table: "LoanApplications",
                column: "AssignedLoanOfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicationDocuments_LoanApplicationId_DocumentType",
                table: "LoanApplicationDocuments",
                columns: new[] { "LoanApplicationId", "DocumentType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplications_AspNetUsers_AssignedLoanOfficerId",
                table: "LoanApplications",
                column: "AssignedLoanOfficerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplications_AspNetUsers_AssignedLoanOfficerId",
                table: "LoanApplications");

            migrationBuilder.DropTable(
                name: "LoanApplicationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_LoanApplications_AssignedLoanOfficerId",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "AssignedLoanOfficerId",
                table: "LoanApplications");
        }
    }
}
