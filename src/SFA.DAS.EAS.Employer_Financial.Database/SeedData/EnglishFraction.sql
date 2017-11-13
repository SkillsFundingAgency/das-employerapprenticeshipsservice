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

--This script should update all the EF values only if the column has just been added, which is when the EF value is null
--it will never be null normally because processing levey declarations fills it in


	UPDATE
	tl
	SET
	tl.EnglishFraction = td.EnglishFraction
	FROM
	employer_financial.TransactionLine AS tl
	INNER JOIN employer_financial.GetLevyDeclarationAndTopUp AS td
	ON tl.EmpRef = td.EmpRef
	AND tl.AccountId = td.AccountId
	AND tl.SubmissionId = td.SubmissionId
	AND tl.EnglishFraction Is Null