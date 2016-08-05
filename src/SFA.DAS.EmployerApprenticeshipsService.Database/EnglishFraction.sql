CREATE TABLE [dbo].[EnglishFraction]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [DateCalculated] DATETIME NULL, 
    [Amount] DECIMAL(18, 4) NULL, 
    [EmpRef] NCHAR(50) NULL
)
