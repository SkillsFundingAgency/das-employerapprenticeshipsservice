CREATE TABLE [dbo].[Paye]
(
	[Ref] NVARCHAR(16) NOT NULL PRIMARY KEY, 
    [AccountId] BIGINT NOT NULL,
	CONSTRAINT [FK_Paye_Account] FOREIGN KEY (AccountId) REFERENCES [Account]([Id])
)
