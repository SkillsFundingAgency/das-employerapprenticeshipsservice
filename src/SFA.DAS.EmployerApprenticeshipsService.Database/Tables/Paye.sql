CREATE TABLE [account].[Paye]
(
	[Ref] NVARCHAR(16) NOT NULL PRIMARY KEY, 
	[LegalEntityId] BIGINT NOT NULL, 
	[AccessToken] VARCHAR(50) NULL,
	[RefreshToken] VARCHAR(50) NULL,
    CONSTRAINT [FK_Paye_LegalEntity] FOREIGN KEY (LegalEntityId) REFERENCES [account].[LegalEntity]([Id])
)
