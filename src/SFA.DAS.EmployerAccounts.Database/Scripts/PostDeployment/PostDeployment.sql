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

:r .\MAC-95_Remove_transfer_connections.sql
:r .\CreateAgreementTemplates.sql
:r .\AML-3762-EOI-API.sql
:r .\UpdateAgreementTemplateV3.sql
-- Because of the way the script below is implemented it cancels execution of subsequent scripts under certain conditions.  Put any future scripts above this line...or fix the script below.
:r .\AML-2119-RestoreAgreementDetails.sql

IF (@@servername NOT LIKE '%pp%' AND @@servername NOT LIKE '%prd%' AND @@servername NOT LIKE '%mo%')
BEGIN
    :r .\SeedDevData.sql
END
