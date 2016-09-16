CREATE TABLE [account].[AccountHistory]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1,1),
	[AccountId] BIGINT NOT NULL,
	[PayeRef] VARCHAR(20) NOT NULL,
	[AddedDate] DATETIME NOT NULL,
	[RemovedDate] DATETIME NULL,
	CONSTRAINT [FK_AccountHistory_Account] FOREIGN KEY (AccountId) REFERENCES [account].[Account]([Id])
)
