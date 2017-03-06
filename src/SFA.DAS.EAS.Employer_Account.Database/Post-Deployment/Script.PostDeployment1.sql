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


--------------------------------------------------------------------------------------
-- Fix EmployerAgreement to have account id
--------------------------------------------------------------------------------------
DECLARE @AccountEntity TABLE
(
	LegalEntityId bigint,
	AccountId bigint
)
DECLARE @TemplateId int

INSERT INTO @AccountEntity
SELECT ea.LegalEntityId, aea.AccountId
FROM [employer_account].[EmployerAgreement] ea
INNER JOIN [employer_account].[AccountEmployerAgreement] aea
	ON ea.Id = aea.EmployerAgreementId

SET @TemplateId = (SELECT MAX([id]) FROM [employer_account].[EmployerAgreementTemplate])

DELETE FROM [employer_account].[AccountEmployerAgreement]
DELETE FROM [employer_account].[EmployerAgreement]

INSERT INTO [employer_account].[EmployerAgreement] (LegalEntityId, AccountId, TemplateId, StatusId)
SELECT LegalEntityId, AccountId, @TemplateId, 1 FROM @AccountEntity

INSERT INTO [employer_account].[AccountEmployerAgreement]
SELECT AccountId, Id FROM [employer_account].[EmployerAgreement]



--------------------------------------------------------------------------------------
-- Drop role table
--------------------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Role' AND TABLE_SCHEMA='employer_account')
	BEGIN
		DROP TABLE [employer_account].[Role]
	END