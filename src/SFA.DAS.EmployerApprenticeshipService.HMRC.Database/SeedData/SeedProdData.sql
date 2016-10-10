/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


IF (NOT EXISTS(SELECT * FROM [levy].[TopUpPercentage] WHERE Id = 1
	AND DateFrom = '2015-01-01 00:00:00.000'))
BEGIN 
	INSERT INTO [levy].[TopUpPercentage]
	(DateFrom,Amount)
	VALUES
	('2015-01-01 00:00:00.000',0.1)
END 