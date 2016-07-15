CREATE TABLE [dbo].[LevyDeclaration]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [empRef] NVARCHAR(50) NOT NULL, 
    [Amount] DECIMAL(18, 4) NULL, 
    [SubmissionDate] DATETIME NULL, 
    [SubmissionType] NCHAR(10) NULL, 
    [SubmissionId] NCHAR(255) NULL
)
