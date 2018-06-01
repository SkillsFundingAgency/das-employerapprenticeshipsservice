CREATE TYPE [employer_financial].[TransferTransactionsTable] AS TABLE
(
	[AccountId] BIGINT NOT NULL,
	[SenderAccountId] BIGINT NOT NULL,
	[SenderAccountName] NVARCHAR(100) NOT NULL,
	[ReceiverAccountId] BIGINT NOT NULL,
	[ReceiverAccountName] NVARCHAR(100) NOT NULL,
	[PeriodEnd] NVARCHAR(50) NOT NULL,
	[Amount] DECIMAL(18,4) NOT NULL,
	[TransactionType] SMALLINT NOT NULL,
	[TransactionDate] DATETIME NOT NULL	
)