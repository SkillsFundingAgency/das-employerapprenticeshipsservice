CREATE TABLE [employer_account].[TransferRequest]
(
	[CommitmentId] BIGINT NOT NULL PRIMARY KEY,
	[CommitmentHashedId] NVARCHAR(100) NOT NULL,
	[SenderAccountId] BIGINT NOT NULL,
	[ReceiverAccountId] BIGINT NOT NULL,
	[Status] INT NOT NULL,
	[TransferCost] DECIMAL(18,5) NOT NULL,
	[CreatedDate] DATETIME NOT NULL,
	[ModifiedDate] DATETIME NULL,
	CONSTRAINT [FK_TransferRequest_SenderAccountId] FOREIGN KEY ([SenderAccountId]) REFERENCES [employer_account].[Account] ([Id]),
	CONSTRAINT [FK_TransferRequest_ReceiverAccountId] FOREIGN KEY ([ReceiverAccountId]) REFERENCES [employer_account].[Account] ([Id])
)
GO

CREATE INDEX [IX_TransferRequest_SenderAccountId_ReceiverAccountId_Status]
ON [employer_account].[TransferRequest]([SenderAccountId] ASC, [ReceiverAccountId] ASC, [Status] ASC)
GO