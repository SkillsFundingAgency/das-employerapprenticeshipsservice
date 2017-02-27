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


-- EmployerAgreement Template
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementTemplate] WHERE Id = 1))
	BEGIN 
		INSERT INTO [employer_account].[EmployerAgreementTemplate](Id, PartialViewName, CreatedDate) 
		VALUES(1, '_Agreement_V1', GETDATE()) 
	END 
	ELSE 
	BEGIN 
		UPDATE [employer_account].[EmployerAgreementTemplate] 
		SET PartialViewName = '_Agreement_V1',
			CreatedDate = GETDATE()
		WHERE Id = 1
	END