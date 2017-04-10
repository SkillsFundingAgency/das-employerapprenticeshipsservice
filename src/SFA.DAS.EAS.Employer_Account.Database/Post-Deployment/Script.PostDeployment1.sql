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


--------------------------------------------------------------------------------------
-- Remove references to duplicate legal entities on agreements
--------------------------------------------------------------------------------------
DECLARE @legalEntityIds as Table
(
	LegalEntityID  BIGINT,
	Code  nVarchar(50)
)
insert into @legalEntityIds (LegalEntityId,code)
select Min(id),code as id from employer_account.legalentity
group by code
having count(1) > 1


DECLARE @legalEntityDuplicateIds as Table
(
	LegalEntityIDKeep  BIGINT,
	LegalEntityIDRemove  BIGINT,
	Code  nVarchar(50)
)

insert into @legalEntityDuplicateIds
select LegalEntityId as LEToKeep,le.Id as LEToRemove, le.code 
from @legalEntityIds dervx
inner join employer_account.legalentity le on le.code = dervx.code and dervx.LegalEntityID <> le.id

update employer_account.employeragreement SET legalentityID = derv.legalentityidkeep
from employer_account.employeragreement ea
inner join @legalEntityDuplicateIds derv on derv.LegalEntityIDRemove = ea.LegalEntityId
inner join employer_Account.LegalEntity le on le.id = ea.LegalEntityId

delete from employer_account.LegalEntity
where id not in
(select LegalEntityId from employer_account.employeragreement)


--------------------------------------------------------------------------------------
-- Rename PrieanKey Column for User table
--------------------------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'PrieanKey' AND TABLE_NAME ='User' AND TABLE_SCHEMA='employer_account')
	BEGIN
		EXEC sp_RENAME 'employer_account.User.PrieanKey', 'UserRef' , 'COLUMN'
	END