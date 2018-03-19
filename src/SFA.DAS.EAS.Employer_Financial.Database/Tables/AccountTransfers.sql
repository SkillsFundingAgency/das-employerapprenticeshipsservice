CREATE TABLE [employer_financial].[AccountTransfers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SenderAccountId] BIGINT NOT NULL, 
    [ReceiverAccountId] BIGINT NOT NULL, 
	[ReceiverAccountName] NVARCHAR(100) NOT NULL, 
    [ApprenticeshipId] BIGINT NOT NULL, 	
	[CourseName] VARCHAR(MAX) NOT NULL, 
	[PeriodEnd] nvarchar(20) NOT NULL,
    [Amount] DECIMAL(18, 5) NOT NULL, 
    [Type] SMALLINT NOT NULL, 
    [TransferDate] DATETIME NOT NULL, 
    [CreatedDate] DATETIME NOT NULL
)
