CREATE TABLE [employer_financial].[LevyOverride]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
	[AccountId] BIGINT NOT NULL,
	[IsLevyPayer] TINYINT NOT NULL DEFAULT 0,
	[DateAdded] DATETIME NOT NULL,
	[ChangedBy] VARCHAR(500) NOT NULL
)
GO
CREATE INDEX [IX_LevyOverride_AccountId_IsLevyPayer] ON [employer_financial].[LevyOverride] (AccountId, DateAdded DESC) INCLUDE (IsLevyPayer)
GO
