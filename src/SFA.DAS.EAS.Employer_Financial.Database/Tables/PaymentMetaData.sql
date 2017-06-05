CREATE TABLE [employer_financial].[PaymentMetaData]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [ProviderName] VARCHAR(MAX) NULL,
    [StandardCode] BIGINT NULL, 
    [FrameworkCode] INT NULL, 	
    [ProgrammeType] INT NULL, 
    [PathwayCode] INT NULL, 
	[PathwayName] VARCHAR(MAX) NULL,
    [ApprenticeshipCourseName] VARCHAR(MAX) NULL, 
	[ApprenticeshipCourseStartDate] DATETIME NULL, 
	[ApprenticeshipCourseLevel] INT NULL, 
    [ApprenticeName] VARCHAR(MAX) NULL, 
    [ApprenticeNINumber] VARCHAR(MAX) NULL
)
