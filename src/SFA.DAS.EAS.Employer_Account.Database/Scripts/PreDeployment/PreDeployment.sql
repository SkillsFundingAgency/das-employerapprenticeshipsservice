/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'employer_account')
BEGIN
	:r .\DeleteHealthChecks.sql
	:r .\AML-2239-DeleteDuplicateAccountHistory.sql
	:r .\AML-2381-DeleteDuplicateUserAccountSettings.sql
	:r .\AML-2119-BackupAgreementDetails.sql
	:r .\AML-2434-DeleteOrphanedUserAccountSettings.sql
END