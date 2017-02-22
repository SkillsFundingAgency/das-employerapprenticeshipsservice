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


-- Role seed data
SET IDENTITY_INSERT  [employer_account].[Role] ON 
IF (NOT EXISTS(SELECT * FROM [employer_account].[Role] WHERE Id = 1
    AND Name = 'Owner'))
BEGIN 
    INSERT INTO [employer_account].[Role](Id, Name) 
    VALUES(1, 'Owner') 
END 
ELSE 
BEGIN 
    UPDATE [employer_account].[Role] 
    SET Name = 'Owner'
    WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [employer_account].[Role] WHERE Id = 2
    AND Name = 'Transactor'))
BEGIN 
    INSERT INTO [employer_account].[Role](Id, Name) 
    VALUES(2, 'Transactor') 
END 
ELSE 
BEGIN 
    UPDATE [employer_account].[Role] 
    SET Name = 'Transactor'
    WHERE Id = 2
END 
IF (NOT EXISTS(SELECT * FROM [employer_account].[Role] WHERE Id = 3
    AND Name = 'Viewer'))
BEGIN 
    INSERT INTO [employer_account].[Role](Id, Name) 
    VALUES(3, 'Viewer') 
END 
ELSE 
BEGIN 
    UPDATE [employer_account].[Role] 
    SET Name = 'Viewer'
    WHERE Id = 3
END 
SET IDENTITY_INSERT  [employer_account].[Role] OFF



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