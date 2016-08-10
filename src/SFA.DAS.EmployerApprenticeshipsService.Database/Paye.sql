CREATE TABLE [dbo].[Paye]
(
	[Ref] NVARCHAR(16) NOT NULL PRIMARY KEY, 
    [AccountId] BIGINT NULL,
	[LegalEntityId] BIGINT NOT NULL, 
	[AccessToken] UNIQUEIDENTIFIER NULL,
	[RefreshToken] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_Paye_Account] FOREIGN KEY (AccountId) REFERENCES [Account]([Id]),
    CONSTRAINT [FK_Paye_LegalEntity] FOREIGN KEY (LegalEntityId) REFERENCES [LegalEntity]([Id])
)
