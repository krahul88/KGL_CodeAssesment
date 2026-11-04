using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductionPlantEvaluation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationLogs",
                columns: table => new
                {
                    EvaluationLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Matched = table.Column<bool>(type: "bit", nullable: false),
                    MatchedRuleset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatchedRule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionPlant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConditionTraceJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EvaluatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationLogs", x => x.EvaluationLogId);
                });

            migrationBuilder.CreateTable(
                name: "Rulesets",
                columns: table => new
                {
                    RSId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rulesets", x => x.RSId);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    ProductionPlant = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RulesetRSId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_Rules_Rulesets_RulesetRSId",
                        column: x => x.RulesetRSId,
                        principalTable: "Rulesets",
                        principalColumn: "RSId");
                });

            migrationBuilder.CreateTable(
                name: "Conditions",
                columns: table => new
                {
                    ConditionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Field = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RulesCon = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    RuleId = table.Column<int>(type: "int", nullable: true),
                    RulesetRSId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conditions", x => x.ConditionId);
                    table.ForeignKey(
                        name: "FK_Conditions_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "RuleId");
                    table.ForeignKey(
                        name: "FK_Conditions_Rulesets_RulesetRSId",
                        column: x => x.RulesetRSId,
                        principalTable: "Rulesets",
                        principalColumn: "RSId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conditions_RuleId",
                table: "Conditions",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Conditions_RulesetRSId",
                table: "Conditions",
                column: "RulesetRSId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RulesetRSId",
                table: "Rules",
                column: "RulesetRSId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conditions");

            migrationBuilder.DropTable(
                name: "EvaluationLogs");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Rulesets");
        }
    }
}
