CREATE TABLE [levy].[EnglishFraction]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [DateCalculated] DATETIME NOT NULL, 
    [Amount] DECIMAL(18, 4) NULL, 
    [EmpRef] NCHAR(50) NULL
)
