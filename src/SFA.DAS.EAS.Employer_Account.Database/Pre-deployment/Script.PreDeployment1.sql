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


--Update for PAYE

IF EXISTS(select 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_NAME = 'FK_Paye_LegalEntity')
BEGIN
	alter table employer_account.paye drop constraint FK_Paye_LegalEntity	
END
GO

IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'PAYE' and COLUMN_NAME = 'legalentityid')
BEGIN 
	alter table employer_account.paye drop column legalentityid
END
GO

IF EXISTS(select 1  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'User' and COLUMN_NAME='Email' and CHARACTER_MAXIMUM_LENGTH = 50)
BEGIN
	ALTER TABLE [employer_account].[User]
	ALTER COLUMN Email NVARCHAR(255) NOT NULL
END

IF EXISTS(select 1  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Invitation' and COLUMN_NAME='Email' and CHARACTER_MAXIMUM_LENGTH = 100)
BEGIN
	ALTER TABLE [employer_account].[Invitation]
	ALTER COLUMN Email NVARCHAR(255) NOT NULL
END

IF EXISTS(Select 1 from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='ACCOUNT')
BEGIN
	IF NOT EXISTS(Select 1 from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Account' AND COlUMN_NAME='CreatedDate')
	BEGIN
		ALTER TABLE [employer_account].[Account]
		ADD CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
	END
END

IF EXISTS(Select 1 from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='LegalEntity')
BEGIN
	IF NOT EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LegalEntity' and COLUMN_NAME='Source')
	BEGIN
		ALTER TABLE [employer_account].[LegalEntity]
		ADD [Source] TINYINT NOT NULL DEFAULT 1
	END
END


IF EXISTS(Select 1 from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='Membership')
BEGIN
	IF NOT EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Membership' and COLUMN_NAME='CreatedDate')
	BEGIN
		ALTER TABLE [employer_account].[Membership]
		ADD [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
	END
END

IF EXISTS(Select 1 from INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='employer_account' AND TABLE_NAME='LegalEntity')
BEGIN
	UPDATE [employer_account].[LegalEntity] SET Code=LOWER(NEWID()) WHERE Code IS NULL OR Code = ''
END