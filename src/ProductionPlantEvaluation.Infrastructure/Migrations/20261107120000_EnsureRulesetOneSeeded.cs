using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProductionPlantEvaluation.Infrastructure.Repositories;

namespace ProductionPlantEvaluation.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261107120000_EnsureRulesetOneSeeded")]
public partial class EnsureRulesetOneSeeded : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DECLARE @RulesetOneId int;

            SELECT TOP (1) @RulesetOneId = [RSId]
            FROM [dbo].[Rulesets]
            WHERE [Name] = N'Ruleset One'
            ORDER BY [RSId];

            IF @RulesetOneId IS NULL
            BEGIN
                INSERT INTO [dbo].[Rulesets] ([Name], [Priority], [IsEnabled])
                VALUES (N'Ruleset One', 1, 1);
                SET @RulesetOneId = CONVERT(int, SCOPE_IDENTITY());
            END
            ELSE
            BEGIN
                UPDATE [dbo].[Rulesets]
                SET [Priority] = 1, [IsEnabled] = 1
                WHERE [RSId] = @RulesetOneId;
            END;

            UPDATE [dbo].[Conditions]
            SET [Field] = N'PublisherNumber', [RulesCon] = 0, [Value] = N'99990', [RuleId] = NULL
            WHERE [RulesetRSId] = @RulesetOneId AND [RuleId] IS NULL AND [Sequence] = 1;
            IF @@ROWCOUNT = 0
                INSERT INTO [dbo].[Conditions] ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
                VALUES (N'PublisherNumber', 0, N'99990', 1, NULL, @RulesetOneId);

            UPDATE [dbo].[Conditions]
            SET [Field] = N'OrderMethod', [RulesCon] = 0, [Value] = N'POD', [RuleId] = NULL
            WHERE [RulesetRSId] = @RulesetOneId AND [RuleId] IS NULL AND [Sequence] = 2;
            IF @@ROWCOUNT = 0
                INSERT INTO [dbo].[Conditions] ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
                VALUES (N'OrderMethod', 0, N'POD', 2, NULL, @RulesetOneId);

            DECLARE @RuleOneId int;

            SELECT TOP (1) @RuleOneId = [RuleId]
            FROM [dbo].[Rules]
            WHERE [RulesetRSId] = @RulesetOneId AND [Name] = N'Rule 1'
            ORDER BY [RuleId];

            IF @RuleOneId IS NULL
            BEGIN
                INSERT INTO [dbo].[Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
                VALUES (N'Rule 1', 1, N'US', @RulesetOneId);
                SET @RuleOneId = CONVERT(int, SCOPE_IDENTITY());
            END
            ELSE
            BEGIN
                UPDATE [dbo].[Rules]
                SET [Sequence] = 1, [ProductionPlant] = N'US'
                WHERE [RuleId] = @RuleOneId;
            END;

            UPDATE [dbo].[Conditions]
            SET [Field] = N'BindTypeCode', [RulesCon] = 0, [Value] = N'PB', [RulesetRSId] = NULL
            WHERE [RuleId] = @RuleOneId AND [Sequence] = 1;
            IF @@ROWCOUNT = 0
                INSERT INTO [dbo].[Conditions] ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
                VALUES (N'BindTypeCode', 0, N'PB', 1, @RuleOneId, NULL);

            UPDATE [dbo].[Conditions]
            SET [Field] = N'IsoCountry', [RulesCon] = 0, [Value] = N'US', [RulesetRSId] = NULL
            WHERE [RuleId] = @RuleOneId AND [Sequence] = 2;
            IF @@ROWCOUNT = 0
                INSERT INTO [dbo].[Conditions] ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
                VALUES (N'IsoCountry', 0, N'US', 2, @RuleOneId, NULL);

            UPDATE [dbo].[Conditions]
            SET [Field] = N'PrintQuantity', [RulesCon] = 1, [Value] = N'20', [RulesetRSId] = NULL
            WHERE [RuleId] = @RuleOneId AND [Sequence] = 3;
            IF @@ROWCOUNT = 0
                INSERT INTO [dbo].[Conditions] ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
                VALUES (N'PrintQuantity', 1, N'20', 3, @RuleOneId, NULL);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
