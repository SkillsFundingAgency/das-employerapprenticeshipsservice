CREATE TABLE [employer_account].[AccountHistory]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY,
	[AccountId] BIGINT NOT NULL,
	[PayeRef] VARCHAR(20) NOT NULL,
	[AddedDate] DATETIME NOT NULL,
	[RemovedDate] DATETIME NULL,
	CONSTRAINT [FK_AccountHistory_Account] FOREIGN KEY (AccountId) REFERENCES [employer_account].[Account]([Id])
)
GO

CREATE INDEX [IX_AccountId_PayeRef] ON [employer_account].[AccountHistory] (AccountId, PayeRef)
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_PayeRef_RemovedDate] ON [employer_account].[AccountHistory] (PayeRef) WHERE RemovedDate IS NULL