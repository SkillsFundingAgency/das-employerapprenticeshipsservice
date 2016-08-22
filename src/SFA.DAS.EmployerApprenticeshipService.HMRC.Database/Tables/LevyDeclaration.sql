CREATE TABLE [levy].[LevyDeclaration]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
	[AccountId] BIGINT NOT NULL DEFAULT 0,
    [empRef] NVARCHAR(50) NOT NULL, 
    [Amount] DECIMAL(18, 4) NULL, 
    [SubmissionDate] DATETIME NULL, 
    [SubmissionType] NCHAR(10) NULL, 
    [SubmissionId] NCHAR(255) NULL
)
