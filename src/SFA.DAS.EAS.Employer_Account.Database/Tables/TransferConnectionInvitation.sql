CREATE TABLE [employer_account].[TransferConnectionInvitation]
(
	[Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	[SenderAccountId] BIGINT NOT NULL,
	[ReceiverAccountId] BIGINT NOT NULL,
	[Status] INT NOT NULL,
	[DeletedBySender] BIT NOT NULL,
	[DeletedByReceiver] BIT NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	CONSTRAINT [FK_TransferConnectionInvitation_SenderAccountId] FOREIGN KEY ([SenderAccountId]) REFERENCES [employer_account].[Account] ([Id]),
	CONSTRAINT [FK_TransferConnectionInvitation_ReceiverAccountId] FOREIGN KEY ([ReceiverAccountId]) REFERENCES [employer_account].[Account] ([Id])
)
GO

CREATE INDEX [IX_TransferConnectionInvitation_SenderAccountId_ReceiverAccountId_Status]
ON [employer_account].[TransferConnectionInvitation]([SenderAccountId] ASC, [ReceiverAccountId] ASC, [Status] ASC)
GO

CREATE UNIQUE INDEX [IX_TransferConnectionInvitation_SenderAccountId_ReceiverAccountId]
ON [employer_account].[TransferConnectionInvitation] (SenderAccountId, ReceiverAccountId)
WHERE [Status] <> 3
GO