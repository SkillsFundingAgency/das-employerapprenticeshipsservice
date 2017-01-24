﻿CREATE TABLE [employer_financial].[PaymentMetaData]
(
	[Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [ProviderName] NVARCHAR(MAX) NULL,
    [StandardCode] BIGINT NULL, 
    [FrameworkCode] INT NULL, 
    [ProgrammeType] INT NULL, 
    [PathwayCode] INT NULL, 
    [ApprenticeshipCourseName] NVARCHAR(MAX) NULL
)
