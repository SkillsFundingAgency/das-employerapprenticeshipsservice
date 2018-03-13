CREATE TABLE [employer_financial].[AccountTransfers]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [SenderAccountId] BIGINT NOT NULL, 
    [RecieverAccountId] BIGINT NOT NULL, 
    [CommitmentId] BIGINT NOT NULL, 
    [Amount] DECIMAL NOT NULL, 
    [Type] SMALLINT NOT NULL, 
    [TransferDate] DATETIME NOT NULL
)
