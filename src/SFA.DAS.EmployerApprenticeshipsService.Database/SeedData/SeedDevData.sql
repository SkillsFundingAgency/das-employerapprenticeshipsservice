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
SET IDENTITY_INSERT  [account].[Role] ON 
IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 1
    AND Name = 'Owner'))
BEGIN 
    INSERT INTO [account].[Role](Id, Name) 
    VALUES(1, 'Owner') 
END 
ELSE 
BEGIN 
    UPDATE [account].[Role] 
    SET Name = 'Owner'
    WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 2
    AND Name = 'Transactor'))
BEGIN 
    INSERT INTO [account].[Role](Id, Name) 
    VALUES(2, 'Transactor') 
END 
ELSE 
BEGIN 
    UPDATE [account].[Role] 
    SET Name = 'Transactor'
    WHERE Id = 2
END 
IF (NOT EXISTS(SELECT * FROM [account].[Role] WHERE Id = 3
    AND Name = 'Viewer'))
BEGIN 
    INSERT INTO [account].[Role](Id, Name) 
    VALUES(3, 'Viewer') 
END 
ELSE 
BEGIN 
    UPDATE [account].[Role] 
    SET Name = 'Viewer'
    WHERE Id = 3
END 
SET IDENTITY_INSERT  [account].[Role] OFF



-- Account seed data
SET IDENTITY_INSERT  [account].[Account] ON
IF (NOT EXISTS(SELECT * FROM [account].[Account] WHERE Id = 1))
BEGIN 
    INSERT INTO [account].[Account](Id, Name, HashedId, CreatedDate) 
    VALUES(1, 'ACME LTD', 'KAKAKAKA', GETDATE()) 
END 
ELSE 
BEGIN 
    UPDATE [account].[Account] 
    SET Name = 'ACME LTD',
	HashedId = 'KAKAKAKA'
    WHERE Id = 1
END 
SET IDENTITY_INSERT  [account].[Account] OFF


-- User seed data 
SET IDENTITY_INSERT  [account].[User] ON
IF (NOT EXISTS(SELECT * FROM [account].[User] WHERE Id = 1
	AND PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B'
    AND Email = 'floyd.price@test.local'))
BEGIN 
    INSERT INTO [account].[User](Id, PireanKey, Email, FirstName, LastName) 
    VALUES(1,'758943A5-86AA-4579-86AF-FB3D4A05850B','floyd.price@test.local', 'Floyd', 'Price') 
END 
ELSE 
BEGIN 
    UPDATE [account].[User] 
    SET PireanKey = '758943A5-86AA-4579-86AF-FB3D4A05850B', Email = 'floyd.price@test.local', FirstName = 'Floyd', LastName = 'Price'
    WHERE Id = 1
END 


SET IDENTITY_INSERT  [account].[User] OFF


-- Membership seed data
IF (NOT EXISTS(SELECT * FROM [account].[Membership] WHERE RoleId = 1
	AND [UserId] = 1
    AND AccountId = 1))
BEGIN 
    INSERT INTO [account].[Membership](RoleId, UserId, AccountId) 
    VALUES(1,1,1) 
END  

-- EmployerAgreement Status
SET IDENTITY_INSERT  [account].[EmployerAgreementStatus] ON 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 1
	AND Name = 'Pending'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(1, 'Pending') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Pending'
	WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 2
	AND Name = 'Signed'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(2, 'Signed') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Signed'
	WHERE Id = 2
END 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 3
	AND Name = 'Expired'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(3, 'Expired') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Expired'
	WHERE Id = 3
END 
IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementStatus] WHERE Id = 4
	AND Name = 'Superceded'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementStatus](Id, Name) 
	VALUES(4, 'Superceded') 
END 
ELSE 
BEGIN 
	UPDATE [account].[EmployerAgreementStatus] 
	SET Name = 'Superceded'
	WHERE Id = 4
END 
SET IDENTITY_INSERT  [account].[EmployerAgreementStatus] OFF


IF (NOT EXISTS(SELECT * FROM [account].[EmployerAgreementTemplate] WHERE [Ref] = 'SFA Employer Agreement V1.0BETA'))
BEGIN 
	INSERT INTO [account].[EmployerAgreementTemplate]( [Text], CreatedDate, Ref, ReleasedDate) 
	VALUES('<p> 1 Introduction </p>', GETDATE(), 'SFA Employer Agreement V1.0BETA', GETDATE()) 
END 