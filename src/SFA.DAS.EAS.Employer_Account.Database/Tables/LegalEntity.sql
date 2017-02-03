CREATE TABLE [employer_account].[LegalEntity]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Code] NVARCHAR(50) NULL, 
    [RegisteredAddress] NVARCHAR(256) NULL, 
    [DateOfIncorporation] DATETIME NULL,
	[Status] NVARCHAR(50) NULL,
	[Source] TINYINT NOT NULL DEFAULT 1,
	[PublicSectorDataSource] TINYINT NULL,
	[Sector] NVARCHAR(100) NULL
)
