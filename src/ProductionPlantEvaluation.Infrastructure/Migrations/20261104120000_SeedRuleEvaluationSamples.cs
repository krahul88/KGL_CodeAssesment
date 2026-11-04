using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProductionPlantEvaluation.Infrastructure.Repositories;

namespace ProductionPlantEvaluation.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261104120000_SeedRuleEvaluationSamples")]
public partial class SeedRuleEvaluationSamples : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DECLARE @RulesetId int;
            INSERT INTO [Rulesets] ([Name], [Priority], [IsEnabled])
            VALUES (N'Ruleset Two', 2, 1);
            SET @RulesetId = CONVERT(int, SCOPE_IDENTITY());

            DECLARE @UsRuleId int;
            INSERT INTO [Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
            VALUES (N'Rule 1', 1, N'US', @RulesetId);
            SET @UsRuleId = CONVERT(int, SCOPE_IDENTITY());
            INSERT INTO [Conditions] ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES (N'productionPlant', 0, N'US', 1, @UsRuleId, NULL);

            DECLARE @CaRuleId int;
            INSERT INTO [Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
            VALUES (N'Rule 2 - Canada', 2, N'CA', @RulesetId);
            SET @CaRuleId = CONVERT(int, SCOPE_IDENTITY());
            INSERT INTO [Conditions] ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES (N'productionPlant', 0, N'CA', 1, @CaRuleId, NULL);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DELETE c
            FROM [Conditions] AS c
            INNER JOIN [Rules] AS r ON r.[RuleId] = c.[RuleId]
            INNER JOIN [Rulesets] AS rs ON rs.[RSId] = r.[RulesetRSId]
            WHERE rs.[Name] = N'Ruleset Two'
              AND r.[Name] IN (N'Rule 1', N'Rule 2 - Canada');

            DELETE r
            FROM [Rules] AS r
            INNER JOIN [Rulesets] AS rs ON rs.[RSId] = r.[RulesetRSId]
            WHERE rs.[Name] = N'Ruleset Two'
              AND r.[Name] IN (N'Rule 1', N'Rule 2 - Canada');

            DELETE FROM [Rulesets]
            WHERE [Name] = N'Ruleset Two'
              AND [Priority] = 2
              AND [IsEnabled] = 1
              AND NOT EXISTS (
                  SELECT 1 FROM [Rules] AS r WHERE r.[RulesetRSId] = [Rulesets].[RSId]);
            """);
    }
}
