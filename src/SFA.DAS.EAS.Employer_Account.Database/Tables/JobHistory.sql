﻿CREATE TABLE [dbo].[JobHistory]
(
	[Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
	[Job] NVARCHAR(50) NOT NULL, 
	[Ran] DATETIME2 NOT NULL
)