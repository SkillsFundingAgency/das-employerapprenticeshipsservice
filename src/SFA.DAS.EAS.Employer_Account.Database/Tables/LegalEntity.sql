CREATE TABLE [employer_account].[LegalEntity]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Code] NVARCHAR(50) NULL, 
    [DateOfIncorporation] DATETIME NULL,
	[Status] NVARCHAR(50) NULL,
	[Source] SMALLINT NOT NULL DEFAULT 1,
	[PublicSectorDataSource] TINYINT NULL,
	[Sector] NVARCHAR(100) NULL
)
