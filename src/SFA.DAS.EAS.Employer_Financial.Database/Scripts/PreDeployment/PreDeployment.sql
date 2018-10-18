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

IF EXISTS (SELECT * FROM sys.schemas WHERE name = 'employer_financial')
BEGIN
	:r .\DeleteHealthChecks.sql
	:r .\AML-2616-DeleteDuplicateLevyDeclarationsWithoutEmpRef.sql
	:r .\AML-2643-DeleteDuplicateTransactionLines.sql
	:r .\AML-2505-PopulateNullProviderNames.sql
	:r .\AML-2671-PopulateNullApprenticeshipCourseNames.sql
	:r .\AML-2395-DropDeprecatedView.sql
END