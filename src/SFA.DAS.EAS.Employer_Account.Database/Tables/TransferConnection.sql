CREATE TABLE [employer_account].[TransferConnection]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SenderUserId] BIGINT NOT NULL,
	[SenderAccountId] BIGINT NOT NULL,
	[ReceiverAccountId] BIGINT NOT NULL,
	[Status] TINYINT NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[ModifiedDate] DATETIME NULL,
	CONSTRAINT [FK_employer_account.TransferConnection_employer_account.User_Id] FOREIGN KEY ([SenderUserId]) REFERENCES [employer_account].[User] ([Id]),
	CONSTRAINT [FK_employer_account.TransferConnection_employer_account.Account_Id] FOREIGN KEY ([SenderAccountId]) REFERENCES [employer_account].[Account] ([Id]),
	CONSTRAINT [FK_employer_account.TransferConnection_employer_account.AccountId] FOREIGN KEY ([ReceiverAccountId]) REFERENCES [employer_account].[Account] ([Id])
)
