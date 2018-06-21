CREATE TABLE [employer_account].[AccountHistory]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1,1),
	[AccountId] BIGINT NOT NULL,
	[PayeRef] VARCHAR(20) NOT NULL,
	[AddedDate] DATETIME NOT NULL,
	[RemovedDate] DATETIME NULL,
	CONSTRAINT [FK_AccountHistory_Account] FOREIGN KEY (AccountId) REFERENCES [employer_account].[Account]([Id])
)
GO
CREATE INDEX [IDX_AccountHistory_Account] on [employer_account].[AccountHistory] (AccountId, PayeRef)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IDX_UNIQUE_AccountHistory_Account_Paye] on [employer_account].[AccountHistory] (AccountId, PayeRef)