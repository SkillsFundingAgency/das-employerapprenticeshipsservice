CREATE TABLE [employer_transactions].[TopUpPercentage]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [DateFrom] DATETIME NOT NULL, 
    [Amount] DECIMAL(18, 4) NULL 
)
