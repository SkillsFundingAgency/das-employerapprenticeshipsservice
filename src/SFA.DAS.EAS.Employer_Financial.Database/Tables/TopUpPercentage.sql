CREATE TABLE [employer_financial].[TopUpPercentage]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [DateFrom] DATETIME NOT NULL, 
    [Amount] DECIMAL(18, 4) NULL 
)
