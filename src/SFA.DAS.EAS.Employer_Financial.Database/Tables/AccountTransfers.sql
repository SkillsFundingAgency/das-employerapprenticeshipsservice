CREATE TABLE [employer_financial].[AccountTransfers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SenderAccountId] BIGINT NOT NULL, 
    [RecieverAccountId] BIGINT NOT NULL, 
    [CommitmentId] BIGINT NOT NULL, 
    [Amount] DECIMAL NOT NULL, 
    [Type] SMALLINT NOT NULL, 
    [TransferDate] DATETIME NOT NULL, 
    [CreatedDate] DATETIME NOT NULL
)
