CREATE TABLE [employer_transactions].[EnglishFraction]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [DateCalculated] DATETIME NOT NULL, 
    [Amount] DECIMAL(18, 5) NULL, 
    [EmpRef] NVARCHAR(50) NULL
)
