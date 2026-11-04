
USE [ProductionPlantEvaluation];
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @RulesetId int;
DECLARE @UsRuleId int;
DECLARE @CanadaRuleId int;


SELECT @RulesetId = [RSId]
FROM [dbo].[Rulesets]
WHERE [Name] = N'Ruleset Two';

IF @RulesetId IS NULL
BEGIN
    INSERT INTO [dbo].[Rulesets] ([Name], [Priority], [IsEnabled])
    VALUES (N'Ruleset Two', 2, 1);

    SET @RulesetId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE [dbo].[Rulesets]
    SET [Priority] = 2,
        [IsEnabled] = 1
    WHERE [RSId] = @RulesetId;
END;


SELECT @UsRuleId = [RuleId]
FROM [dbo].[Rules]
WHERE [RulesetRSId] = @RulesetId
  AND [Name] = N'Rule 1';

IF @UsRuleId IS NULL
BEGIN
    INSERT INTO [dbo].[Rules]
        ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
    VALUES
        (N'Rule 1', 1, N'US', @RulesetId);

    SET @UsRuleId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE [dbo].[Rules]
    SET [Sequence] = 1,
        [ProductionPlant] = N'US'
    WHERE [RuleId] = @UsRuleId;
END;


IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[Conditions]
    WHERE [RuleId] = @UsRuleId
      AND [Field] = N'IsoCountry'
      AND [RulesCon] = 0
      AND [Value] = N'US'
      AND [Sequence] = 1
)
BEGIN
    INSERT INTO [dbo].[Conditions]
        ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
    VALUES
        (N'IsoCountry', 0, N'US', 1, @UsRuleId, NULL);
END;


SELECT @CanadaRuleId = [RuleId]
FROM [dbo].[Rules]
WHERE [RulesetRSId] = @RulesetId
  AND [Name] = N'Rule 2 - Canada';

IF @CanadaRuleId IS NULL
BEGIN
    INSERT INTO [dbo].[Rules]
        ([Name], [Sequence], [ProductionPlant], [RulesetRSId])
    VALUES
        (N'Rule 2 - Canada', 2, N'CA', @RulesetId);

    SET @CanadaRuleId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE [dbo].[Rules]
    SET [Sequence] = 2,
        [ProductionPlant] = N'CA'
    WHERE [RuleId] = @CanadaRuleId;
END;

IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[Conditions]
    WHERE [RuleId] = @CanadaRuleId
      AND [Field] = N'IsoCountry'
      AND [RulesCon] = 0
      AND [Value] = N'CA'
      AND [Sequence] = 1
)
BEGIN
    INSERT INTO [dbo].[Conditions]
        ([Field], [RulesCon], [Value], [Sequence], [RuleId], [RulesetRSId])
    VALUES
        (N'IsoCountry', 0, N'CA', 1, @CanadaRuleId, NULL);
END;

COMMIT TRANSACTION;
GO
