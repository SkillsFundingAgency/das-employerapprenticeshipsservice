CREATE TABLE [employer_financial].[EnglishFractionOverride]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [AccountId] BIGINT NOT NULL, 
    [EmpRef] NVARCHAR(50) NOT NULL, 
    [Amount] DECIMAL(18, 5) NOT NULL, 
    [CreatedOn] DATETIME NOT NULL DEFAULT GETDATE(), 
	[DateFrom] DATETIME NOT NULL, 
    [ApprovedBy] NVARCHAR(255) NOT NULL
)
