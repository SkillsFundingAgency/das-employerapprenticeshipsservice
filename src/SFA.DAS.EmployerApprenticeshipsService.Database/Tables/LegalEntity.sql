CREATE TABLE [account].[LegalEntity]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Code] NVARCHAR(50) NULL, 
    [RegisteredAddress] NVARCHAR(256) NULL, 
    [DateOfIncorporation] DATETIME NULL
)
