CREATE TABLE [employer_account].[Account]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
	[HashedId] NVARCHAR(100) NULL,
    [Name] NVARCHAR(100) NOT NULL ,
	[CreatedDate] DATETIME NOT NULL,
	[ModifiedDate] DATETIME NULL
)
GO

CREATE INDEX [IX_Account_HashedAccountId] ON [employer_account].[Account] ([HashedId])
