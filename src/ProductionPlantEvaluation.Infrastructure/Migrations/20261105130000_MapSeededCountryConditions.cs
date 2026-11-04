using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProductionPlantEvaluation.Infrastructure.Repositories;

namespace ProductionPlantEvaluation.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261105130000_MapSeededCountryConditions")]
public partial class MapSeededCountryConditions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            UPDATE c
            SET [Field] = N'IsoCountry'
            FROM [Conditions] AS c
            INNER JOIN [Rules] AS r ON r.[RuleId] = c.[RuleId]
            INNER JOIN [Rulesets] AS rs ON rs.[RSId] = r.[RulesetRSId]
            WHERE rs.[Name] = N'Ruleset Two'
              AND c.[Field] = N'productionPlant'
              AND ((r.[Name] = N'Rule 1' AND c.[Value] = N'US')
                OR (r.[Name] = N'Rule 2 - Canada' AND c.[Value] = N'CA'));
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            UPDATE c
            SET [Field] = N'productionPlant'
            FROM [Conditions] AS c
            INNER JOIN [Rules] AS r ON r.[RuleId] = c.[RuleId]
            INNER JOIN [Rulesets] AS rs ON rs.[RSId] = r.[RulesetRSId]
            WHERE rs.[Name] = N'Ruleset Two'
              AND c.[Field] = N'IsoCountry'
              AND ((r.[Name] = N'Rule 1' AND c.[Value] = N'US')
                OR (r.[Name] = N'Rule 2 - Canada' AND c.[Value] = N'CA'));
            """);
    }
}
