CREATE TABLE [employer_financial].[TransferConnectionInvitationChange]
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
	CONSTRAINT [FK_TransferConnectionInvitationChange_TransferConnectionInvitationId] FOREIGN KEY ([TransferConnectionInvitationId]) REFERENCES [employer_financial].[TransferConnectionInvitation] ([Id]),
	CONSTRAINT [FK_TransferConnectionInvitationChange_SenderAccountId] FOREIGN KEY ([SenderAccountId]) REFERENCES [employer_financial].[Account] ([Id]),
	CONSTRAINT [FK_TransferConnectionInvitationChange_ReceiverAccountId] FOREIGN KEY ([ReceiverAccountId]) REFERENCES [employer_financial].[Account] ([Id]),
	CONSTRAINT [FK_TransferConnectionInvitationChange_UserId] FOREIGN KEY ([UserId]) REFERENCES [employer_financial].[User] ([Id])
)
GO

CREATE INDEX [IX_TransferConnectionInvitationChange_TransferConnectionInvitationId]
ON [employer_financial].[TransferConnectionInvitationChange]([TransferConnectionInvitationId] ASC)
GO