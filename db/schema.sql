
IF DB_ID(N'ProductionPlantEvaluation') IS NULL
BEGIN
    CREATE DATABASE [ProductionPlantEvaluation];
END;
GO

USE [ProductionPlantEvaluation];
GO

SET XACT_ABORT ON;
GO


IF OBJECT_ID(N'dbo.Rulesets', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Rulesets]
    (
        [RSId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Priority] int NOT NULL,
        [IsEnabled] bit NOT NULL,
        CONSTRAINT [PK_Rulesets] PRIMARY KEY ([RSId])
    );
END;
GO


IF OBJECT_ID(N'dbo.Rules', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Rules]
    (
        [RuleId] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Sequence] int NOT NULL,
        [ProductionPlant] nvarchar(max) NOT NULL,
        [RulesetRSId] int NULL,
        CONSTRAINT [PK_Rules] PRIMARY KEY ([RuleId])
    );
END;
GO


IF COL_LENGTH(N'dbo.Rules', N'RulesetRSId') IS NULL
   AND COL_LENGTH(N'dbo.Rules', N'RSId') IS NOT NULL
BEGIN
    EXEC sys.sp_rename N'dbo.Rules.RSId', N'RulesetRSId', N'COLUMN';
END;
GO


IF OBJECT_ID(N'dbo.Conditions', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Conditions]
    (
        [ConditionId] int IDENTITY(1,1) NOT NULL,
        [Field] nvarchar(max) NOT NULL,
        [RulesCon] int NOT NULL,
        [Value] nvarchar(max) NOT NULL,
        [Sequence] int NOT NULL,
        [RuleId] int NULL,
        [RulesetRSId] int NULL,
        CONSTRAINT [PK_Conditions] PRIMARY KEY ([ConditionId])
    );
END;
GO


IF COL_LENGTH(N'dbo.Conditions', N'RulesetRSId') IS NULL
   AND COL_LENGTH(N'dbo.Conditions', N'RSId') IS NOT NULL
BEGIN
    EXEC sys.sp_rename N'dbo.Conditions.RSId', N'RulesetRSId', N'COLUMN';
END;
GO


IF OBJECT_ID(N'dbo.EvaluationLogs', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[EvaluationLogs]
    (
        [EvaluationLogId] bigint IDENTITY(1,1) NOT NULL,
        [OrderId] nvarchar(max) NOT NULL,
        [InputJson] nvarchar(max) NOT NULL,
        [Matched] bit NOT NULL,
        [MatchedRuleset] nvarchar(max) NULL,
        [MatchedRule] nvarchar(max) NULL,
        [ProductionPlant] nvarchar(max) NULL,
        [Reason] nvarchar(max) NOT NULL,
        [ConditionTraceJson] nvarchar(max) NOT NULL,
        [EvaluatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_EvaluationLogs] PRIMARY KEY ([EvaluationLogId])
    );
END;
GO


IF EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Rules_Rulesets_RSId'
      AND parent_object_id = OBJECT_ID(N'dbo.Rules')
)
    ALTER TABLE [dbo].[Rules] DROP CONSTRAINT [FK_Rules_Rulesets_RSId];
GO

IF EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Conditions_Rulesets_RSId'
      AND parent_object_id = OBJECT_ID(N'dbo.Conditions')
)
    ALTER TABLE [dbo].[Conditions] DROP CONSTRAINT [FK_Conditions_Rulesets_RSId];
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Rules_RSId'
      AND object_id = OBJECT_ID(N'dbo.Rules')
)
    DROP INDEX [IX_Rules_RSId] ON [dbo].[Rules];
GO

IF EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Conditions_RSId'
      AND object_id = OBJECT_ID(N'dbo.Conditions')
)
    DROP INDEX [IX_Conditions_RSId] ON [dbo].[Conditions];
GO


IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Rules_Rulesets_RulesetRSId'
      AND parent_object_id = OBJECT_ID(N'dbo.Rules')
)
BEGIN
    ALTER TABLE [dbo].[Rules] WITH CHECK
    ADD CONSTRAINT [FK_Rules_Rulesets_RulesetRSId]
        FOREIGN KEY ([RulesetRSId]) REFERENCES [dbo].[Rulesets] ([RSId]);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Conditions_Rules_RuleId'
      AND parent_object_id = OBJECT_ID(N'dbo.Conditions')
)
BEGIN
    ALTER TABLE [dbo].[Conditions] WITH CHECK
    ADD CONSTRAINT [FK_Conditions_Rules_RuleId]
        FOREIGN KEY ([RuleId]) REFERENCES [dbo].[Rules] ([RuleId]);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Conditions_Rulesets_RulesetRSId'
      AND parent_object_id = OBJECT_ID(N'dbo.Conditions')
)
BEGIN
    ALTER TABLE [dbo].[Conditions] WITH CHECK
    ADD CONSTRAINT [FK_Conditions_Rulesets_RulesetRSId]
        FOREIGN KEY ([RulesetRSId]) REFERENCES [dbo].[Rulesets] ([RSId]);
END;
GO


IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Rules_RulesetRSId'
      AND object_id = OBJECT_ID(N'dbo.Rules')
)
    CREATE INDEX [IX_Rules_RulesetRSId] ON [dbo].[Rules] ([RulesetRSId]);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Conditions_RuleId'
      AND object_id = OBJECT_ID(N'dbo.Conditions')
)
    CREATE INDEX [IX_Conditions_RuleId] ON [dbo].[Conditions] ([RuleId]);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Conditions_RulesetRSId'
      AND object_id = OBJECT_ID(N'dbo.Conditions')
)
    CREATE INDEX [IX_Conditions_RulesetRSId] ON [dbo].[Conditions] ([RulesetRSId]);
GO
