CREATE TABLE [employer_financial].[EnglishFraction]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [DateCalculated] DATETIME NOT NULL, 
    [Amount] DECIMAL(18, 5) NULL, 
    [EmpRef] NVARCHAR(50) NULL, 
    [DateCreated] DATETIME NOT NULL DEFAULT GETDATE()
)

GO

CREATE INDEX [IX_EnglishFraction_EmpRef_DateCalculated] ON [employer_financial].[EnglishFraction] ([EmpRef], [DateCalculated]) WITH (ONLINE = ON)
