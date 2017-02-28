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

-- Account seed data
SET IDENTITY_INSERT  [employer_account].[Account] ON
IF (NOT EXISTS(SELECT * FROM [employer_account].[Account] WHERE Id = 1))
BEGIN 
    INSERT INTO [employer_account].[Account](Id, Name, HashedId, CreatedDate) 
    VALUES(1, 'ACME LTD', 'KAKAKAKA', GETDATE()) 
END 
ELSE 
BEGIN 
    UPDATE [employer_account].[Account] 
    SET Name = 'ACME LTD',
	HashedId = 'KAKAKAKA'
    WHERE Id = 1
END 
SET IDENTITY_INSERT  [employer_account].[Account] OFF


-- User seed data 
SET IDENTITY_INSERT  [employer_account].[User] ON
IF (NOT EXISTS(SELECT * FROM [employer_account].[User] WHERE Id = 1
	AND PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B'
    AND Email = 'floyd.price@test.local'))
BEGIN 
    INSERT INTO [employer_account].[User](Id, PireanKey, Email, FirstName, LastName) 
    VALUES(1,'758943A5-86AA-4579-86AF-FB3D4A05850B','floyd.price@test.local', 'Floyd', 'Price') 
END 
ELSE 
BEGIN 
    UPDATE [employer_account].[User] 
    SET PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B', Email = 'floyd.price@test.local', FirstName = 'Floyd', LastName = 'Price'
    WHERE Id = 1
END 


SET IDENTITY_INSERT  [employer_account].[User] OFF


-- Membership seed data
IF (NOT EXISTS(SELECT * FROM [employer_account].[Membership] WHERE RoleId = 1
	AND [UserId] = 1
    AND AccountId = 1))
BEGIN 
    INSERT INTO [employer_account].[Membership](RoleId, UserId, AccountId) 
    VALUES(1,1,1) 
END  


-- Template seed
SET IDENTITY_INSERT  [employer_account].[EmployerAgreementTemplate] ON
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementTemplate] WHERE PartialViewName = '_Agreement_V1'))
BEGIN 
	INSERT INTO [employer_account].[EmployerAgreementTemplate]( PartialViewName, CreatedDate) 
	VALUES('_Agreement_V1', GETDATE()) 
END 
SET IDENTITY_INSERT  [employer_account].[EmployerAgreementTemplate] OFF