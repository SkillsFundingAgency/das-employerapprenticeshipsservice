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


EXECUTE Cleardown;

-- Role seed data
SET IDENTITY_INSERT  [dbo].[Role] ON 
IF (NOT EXISTS(SELECT * FROM [dbo].[Role] WHERE Id = 1
    AND Name = 'Owner'))
BEGIN 
    INSERT INTO [dbo].[Role](Id, Name) 
    VALUES(1, 'Owner') 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[Role] 
    SET Name = 'Owner'
    WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [dbo].[Role] WHERE Id = 2
    AND Name = 'Transactor'))
BEGIN 
    INSERT INTO [dbo].[Role](Id, Name) 
    VALUES(2, 'Transactor') 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[Role] 
    SET Name = 'Transactor'
    WHERE Id = 2
END 
IF (NOT EXISTS(SELECT * FROM [dbo].[Role] WHERE Id = 3
    AND Name = 'Viewer'))
BEGIN 
    INSERT INTO [dbo].[Role](Id, Name) 
    VALUES(3, 'Viewer') 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[Role] 
    SET Name = 'Viewer'
    WHERE Id = 3
END 
SET IDENTITY_INSERT  [dbo].[Role] OFF
GO

-- Account seed data
SET IDENTITY_INSERT  [dbo].[Account] ON
IF (NOT EXISTS(SELECT * FROM [dbo].[Account] WHERE Id = 1
    AND Name = 'Floyd Price Ltd'))
BEGIN 
    INSERT INTO [dbo].[Account](Id, Name) 
    VALUES(1, 'Floyd Price Ltd') 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[Account] 
    SET Name = 'Floyd Price Ltd'
    WHERE Id = 1
END 
SET IDENTITY_INSERT  [dbo].[Account] OFF
GO

-- User seed data 
SET IDENTITY_INSERT  [dbo].[User] ON
IF (NOT EXISTS(SELECT * FROM [dbo].[User] WHERE Id = 1
	AND PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B'
    AND Email = 'floyd.price@test.local'))
BEGIN 
    INSERT INTO [dbo].[User](Id, PireanKey, Email, FirstName, LastName) 
    VALUES(1,'758943A5-86AA-4579-86AF-FB3D4A05850B','floyd.price@test.local', 'Floyd', 'Price') 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[User] 
    SET PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B', Email = 'floyd.price@test.local', FirstName = 'Floyd', LastName = 'Price'
    WHERE Id = 1
END 

IF (NOT EXISTS(SELECT * FROM [dbo].[User] WHERE Id = 2
	AND PireanKey = 'A0BBC02B-39A0-4DEC-8018-1A3A98A18A37'
    AND Email = 'ian.russell@test.local'))
BEGIN 
    INSERT INTO [dbo].[User](Id, PireanKey, Email, FirstName, LastName) 
    VALUES(2,'A0BBC02B-39A0-4DEC-8018-1A3A98A18A37','ian.russell@test.local', 'Ian', 'Russell') 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[User] 
    SET PireanKey = 'A0BBC02B-39A0-4DEC-8018-1A3A98A18A37', Email = 'ian.russell@test.local', FirstName = 'Ian', LastName = 'Russell'
    WHERE Id = 2
END 

SET IDENTITY_INSERT  [dbo].[User] OFF
GO

-- Membership seed data
IF (NOT EXISTS(SELECT * FROM [dbo].[Membership] WHERE RoleId = 1
	AND [UserId] = 1
    AND AccountId = 1))
BEGIN 
    INSERT INTO [dbo].[Membership](RoleId, UserId, AccountId) 
    VALUES(1,1,1) 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[Membership] 
    SET [UserId] = 1, AccountId = 1
    WHERE RoleId = 1
END 

-- EmployerAgreement Status
SET IDENTITY_INSERT  [dbo].[EmployerAgreementStatus] ON 
IF (NOT EXISTS(SELECT * FROM [dbo].[EmployerAgreementStatus] WHERE Id = 1
	AND Name = 'Pending'))
BEGIN 
	INSERT INTO [dbo].[EmployerAgreementStatus](Id, Name) 
	VALUES(1, 'Pending') 
END 
ELSE 
BEGIN 
	UPDATE [dbo].[EmployerAgreementStatus] 
	SET Name = 'Pending'
	WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [dbo].[EmployerAgreementStatus] WHERE Id = 2
	AND Name = 'Signed'))
BEGIN 
	INSERT INTO [dbo].[EmployerAgreementStatus](Id, Name) 
	VALUES(2, 'Signed') 
END 
ELSE 
BEGIN 
	UPDATE [dbo].[EmployerAgreementStatus] 
	SET Name = 'Signed'
	WHERE Id = 2
END 
SET IDENTITY_INSERT  [dbo].[EmployerAgreementStatus] OFF

-- EmployerAgreement Template
SET IDENTITY_INSERT  [dbo].[EmployerAgreementTemplate] ON 
IF (NOT EXISTS(SELECT * FROM [dbo].[EmployerAgreementTemplate] WHERE Id = 1
	AND [Text] = 'I am a template'))
BEGIN 
	INSERT INTO [dbo].[EmployerAgreementTemplate](Id, [Text]) 
	VALUES(1, 'I am a template') 
END 
ELSE 
BEGIN 
	UPDATE [dbo].[EmployerAgreementTemplate] 
	SET [Text] = 'I am a template'
	WHERE Id = 1
END 

SET IDENTITY_INSERT  [dbo].[EmployerAgreementTemplate] OFF
