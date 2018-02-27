CREATE TABLE [employer_account].[TransferConnectionInvitation]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[SenderUserId] BIGINT NOT NULL,
	[SenderAccountId] BIGINT NOT NULL,
	[ReceiverAccountId] BIGINT NOT NULL,
	[ApproverUserId] BIGINT NULL,
	[RejectorUserId] BIGINT NULL,
	[Status] INT NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[ModifiedDate] DATETIME NULL
	CONSTRAINT [FK_employer_account.TransferConnectionInvitation_employer_account.User_Id] FOREIGN KEY ([SenderUserId]) REFERENCES [employer_account].[User] ([Id]),
	CONSTRAINT [FK_employer_account.TransferConnectionInvitation_employer_account.Account_Id] FOREIGN KEY ([SenderAccountId]) REFERENCES [employer_account].[Account] ([Id]),
	CONSTRAINT [FK_employer_account.TransferConnectionInvitation_employer_account.AccountId] FOREIGN KEY ([ReceiverAccountId]) REFERENCES [employer_account].[Account] ([Id])
)
GO

CREATE UNIQUE INDEX [IX_SenderAccountId_ReceiverAccountId]
ON [employer_account].[TransferConnectionInvitation]([SenderAccountId] ASC, [ReceiverAccountId] ASC)
WHERE [Status] = 1
GO