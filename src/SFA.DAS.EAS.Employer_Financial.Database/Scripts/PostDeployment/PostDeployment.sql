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

:r .\CreateTopUpPercentages.sql
:r .\CreateTransactionLineTypes.sql
:r .\CreatePaymentTransactionTypes.sql

IF (@@servername NOT LIKE '%pp%' AND @@servername NOT LIKE '%prd%' AND @@servername NOT LIKE '%mo%')
BEGIN
:r .\SeedDevData.sql
END
GO

-- the following is a dirty hack to basically achieve the following...
-- IF (@@servername NOT LIKE '%pp%' AND @@servername NOT LIKE '%prd%' AND @@servername NOT LIKE '%mo%')
-- BEGIN
-- :r ..\Manual\GenerateTestData\DataGenFramework\AddDataGenerationFramework.sql
--END

IF (@@servername LIKE '%pp%' OR @@servername LIKE '%prd%' OR @@servername LIKE '%mo%')
BEGIN
	EXEC('CREATE SCHEMA DataGen')

	CREATE TYPE DataGen.PaymentGenerationSourceTable AS TABLE   
	(monthBeforeToDate int, amount decimal(18, 4), paymentsToGenerate int, createMonth datetime)

	CREATE TYPE DataGen.LevyGenerationSourceTable AS TABLE   
	(monthBeforeToDate int, amount decimal(18, 4), createMonth datetime, payrollYear varchar(5), payrollMonth int)

	SET NOEXEC ON;
END
GO

:r ..\Manual\GenerateTestData\DataGenFramework\AddDataGenerationFramework.sql

SET NOEXEC OFF;
GO

IF (@@servername LIKE '%pp%' OR @@servername LIKE '%prd%' OR @@servername LIKE '%mo%')
BEGIN
	DROP TYPE DataGen.PaymentGenerationSourceTable
	DROP TYPE DataGen.LevyGenerationSourceTable
	EXEC('DROP SCHEMA DataGen')
END
GO
