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


IF EXISTS(select 1 from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'EnglishFraction' and COLUMN_NAME = 'Amount' and NUMERIC_SCALE = 4)
BEGIN
	ALTER TABLE [employer_financial].[EnglishFraction] 
	ALTER COLUMN AMOUNT DECIMAL(18,5) NULL
END
IF EXISTS(select 1  from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'EnglishFraction' and COLUMN_NAME='EmpRef' AND DATA_TYPE = 'nchar')
BEGIN
	ALTER TABLE [employer_financial].[EnglishFraction] 
	ALTER COLUMN EmpRef NVARCHAR(50) NULL
END