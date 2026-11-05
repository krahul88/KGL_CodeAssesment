using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProductionPlantEvaluation.Infrastructure.Repositories;

namespace ProductionPlantEvaluation.Infrastructure.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261106120000_UpdateRulesetConfiguration")]
public partial class UpdateRulesetConfiguration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DECLARE @RulesetOneId int;
            DECLARE @RulesetTwoId int;

            SELECT @RulesetOneId = [RSId]
            FROM [dbo].[Rulesets]
            WHERE [Name] = N'Ruleset One';

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

            SELECT @RulesetTwoId = [RSId]
            FROM [dbo].[Rulesets]
            WHERE [Name] = N'Ruleset Two';

            IF @RulesetTwoId IS NULL
            BEGIN
                INSERT INTO [dbo].[Rulesets] ([Name], [Priority], [IsEnabled])
                VALUES (N'Ruleset Two', 2, 1);
                SET @RulesetTwoId = CONVERT(int, SCOPE_IDENTITY());
            END
            ELSE
            BEGIN
                UPDATE [dbo].[Rulesets]
                SET [Priority] = 2, [IsEnabled] = 1
                WHERE [RSId] = @RulesetTwoId;
            END;

            DELETE c
            FROM [dbo].[Conditions] AS c
            WHERE c.[RulesetRSId] IN (@RulesetOneId, @RulesetTwoId)
               OR EXISTS
               (
                   SELECT 1
                   FROM [dbo].[Rules] AS r
                   WHERE r.[RuleId] = c.[RuleId]
                     AND r.[RulesetRSId] IN (@RulesetOneId, @RulesetTwoId)
               );

            DELETE FROM [dbo].[Rules]
            WHERE [RulesetRSId] IN (@RulesetOneId, @RulesetTwoId);

            INSERT INTO [dbo].[Conditions]
                ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES
                (N'PublisherNumber', 0, N'99990', 1, NULL, @RulesetOneId),
                (N'OrderMethod', 0, N'POD', 2, NULL, @RulesetOneId),
                (N'PublisherNumber', 0, N'99999', 1, NULL, @RulesetTwoId),
                (N'OrderMethod', 0, N'POD', 2, NULL, @RulesetTwoId);

            DECLARE @RuleOneId int;
            INSERT INTO [dbo].[Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
            VALUES (N'Rule 1', 1, N'US', @RulesetOneId);
            SET @RuleOneId = CONVERT(int, SCOPE_IDENTITY());

            INSERT INTO [dbo].[Conditions]
                ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES
                (N'BindTypeCode', 0, N'PB', 1, @RuleOneId, NULL),
                (N'IsoCountry', 0, N'US', 2, @RuleOneId, NULL),
                (N'PrintQuantity', 1, N'20', 3, @RuleOneId, NULL);

            DECLARE @RuleTwoId int;
            INSERT INTO [dbo].[Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
            VALUES (N'Rule 2', 1, N'UK', @RulesetTwoId);
            SET @RuleTwoId = CONVERT(int, SCOPE_IDENTITY());

            INSERT INTO [dbo].[Conditions]
                ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES
                (N'BindTypeCode', 0, N'CV', 1, @RuleTwoId, NULL),
                (N'IsoCountry', 0, N'UK', 2, @RuleTwoId, NULL),
                (N'PrintQuantity', 1, N'20', 3, @RuleTwoId, NULL);

            DECLARE @RuleThreeId int;
            INSERT INTO [dbo].[Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
            VALUES (N'Rule 3', 2, N'KGL', @RulesetTwoId);
            SET @RuleThreeId = CONVERT(int, SCOPE_IDENTITY());

            INSERT INTO [dbo].[Conditions]
                ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES
                (N'BindTypeCode', 0, N'PB', 1, @RuleThreeId, NULL),
                (N'IsoCountry', 0, N'US', 2, @RuleThreeId, NULL),
                (N'PrintQuantity', 2, N'20', 3, @RuleThreeId, NULL);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            DECLARE @RulesetOneId int;
            DECLARE @RulesetTwoId int;

            SELECT @RulesetOneId = [RSId]
            FROM [dbo].[Rulesets]
            WHERE [Name] = N'Ruleset One';

            SELECT @RulesetTwoId = [RSId]
            FROM [dbo].[Rulesets]
            WHERE [Name] = N'Ruleset Two';

            DELETE c
            FROM [dbo].[Conditions] AS c
            WHERE c.[RulesetRSId] IN (@RulesetOneId, @RulesetTwoId)
               OR EXISTS
               (
                   SELECT 1
                   FROM [dbo].[Rules] AS r
                   WHERE r.[RuleId] = c.[RuleId]
                     AND r.[RulesetRSId] IN (@RulesetOneId, @RulesetTwoId)
               );

            DELETE FROM [dbo].[Rules]
            WHERE [RulesetRSId] IN (@RulesetOneId, @RulesetTwoId);

            DELETE FROM [dbo].[Rulesets]
            WHERE [RSId] = @RulesetOneId;

            UPDATE [dbo].[Rulesets]
            SET [Priority] = 2, [IsEnabled] = 1
            WHERE [RSId] = @RulesetTwoId;

            DECLARE @UsRuleId int;
            INSERT INTO [dbo].[Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
            VALUES (N'Rule 1', 1, N'US', @RulesetTwoId);
            SET @UsRuleId = CONVERT(int, SCOPE_IDENTITY());

            INSERT INTO [dbo].[Conditions]
                ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES (N'IsoCountry', 0, N'US', 1, @UsRuleId, NULL);

            DECLARE @CanadaRuleId int;
            INSERT INTO [dbo].[Rules] ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
            VALUES (N'Rule 2 - Canada', 2, N'CA', @RulesetTwoId);
            SET @CanadaRuleId = CONVERT(int, SCOPE_IDENTITY());

            INSERT INTO [dbo].[Conditions]
                ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
            VALUES (N'IsoCountry', 0, N'CA', 1, @CanadaRuleId, NULL);
            """);
    }
}
