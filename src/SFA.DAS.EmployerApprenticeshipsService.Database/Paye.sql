CREATE TABLE [dbo].[Paye]
(
	[Ref] NVARCHAR(10) NOT NULL PRIMARY KEY, 
    [AccountId] INT NULL,
	CONSTRAINT [FK_Paye_Account] FOREIGN KEY (AccountId) REFERENCES [Account]([Id])
)
