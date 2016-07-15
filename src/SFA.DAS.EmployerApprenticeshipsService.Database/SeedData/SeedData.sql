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
    INSERT INTO [dbo].[User](Id, PireanKey, Email) 
    VALUES(1,'758943A5-86AA-4579-86AF-FB3D4A05850B','floyd.price@test.local') 
END 
ELSE 
BEGIN 
    UPDATE [dbo].[User] 
    SET PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B', Email = 'floyd.price@test.local'
    WHERE Id = 1
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
