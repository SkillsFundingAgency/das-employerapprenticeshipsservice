CREATE TABLE [employer_account].[TransferConnectionInvitationChange]
(
	[Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
	[TransferConnectionInvitationId] INT NOT NULL,
	[SenderAccountId] BIGINT NULL,
	[ReceiverAccountId] BIGINT NULL,
	[Status] INT NULL,
	[DeletedBySender] BIT NULL,
	[DeletedByReceiver] BIT NULL,
	[UserId] BIGINT NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	CONSTRAINT [FK_TransferConnectionInvitationChange_TransferConnectionInvitationId] FOREIGN KEY ([TransferConnectionInvitationId]) REFERENCES [employer_account].[TransferConnectionInvitation] ([Id]),
	CONSTRAINT [FK_TransferConnectionInvitationChange_SenderAccountId] FOREIGN KEY ([SenderAccountId]) REFERENCES [employer_account].[Account] ([Id]),
	CONSTRAINT [FK_TransferConnectionInvitationChange_ReceiverAccountId] FOREIGN KEY ([ReceiverAccountId]) REFERENCES [employer_account].[Account] ([Id]),
	CONSTRAINT [FK_TransferConnectionInvitationChange_UserId] FOREIGN KEY ([UserId]) REFERENCES [employer_account].[User] ([Id])
)
GO

CREATE INDEX [IX_TransferConnectionInvitationChange_TransferConnectionInvitationId]
ON [employer_account].[TransferConnectionInvitationChange]([TransferConnectionInvitationId] ASC)
GO