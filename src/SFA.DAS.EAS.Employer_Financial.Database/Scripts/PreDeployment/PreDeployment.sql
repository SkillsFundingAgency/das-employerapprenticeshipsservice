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
-- ONLY DO THIS FOR DB UPGRADES - CHECK THE SCHEMA EXISTS!
IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'employer_financial')
BEGIN
	:r .\AML-2616-RemoveDuplicateLevyDeclarationsWithoutEmpRef.sql

END