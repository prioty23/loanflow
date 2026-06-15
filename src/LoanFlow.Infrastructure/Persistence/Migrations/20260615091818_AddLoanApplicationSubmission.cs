using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanApplicationSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmissionReference",
                table: "LoanApplications",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicantSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    NationalIdNumber = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CurrentAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantSnapshots_LoanApplications_LoanApplicationId",
                        column: x => x.LoanApplicationId,
                        principalTable: "LoanApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_SubmissionReference",
                table: "LoanApplications",
                column: "SubmissionReference",
                unique: true,
                filter: "[SubmissionReference] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantSnapshots_LoanApplicationId",
                table: "ApplicantSnapshots",
                column: "LoanApplicationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_LoanApplications_SubmissionReference",
                table: "LoanApplications");

            migrationBuilder.DropColumn(
                name: "SubmissionReference",
                table: "LoanApplications");
        }
    }
}
