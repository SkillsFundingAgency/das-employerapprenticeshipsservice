﻿/*
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


-- EmployerAgreement Status
SET IDENTITY_INSERT  [employer_account].[EmployerAgreementStatus] ON 
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementStatus] WHERE Id = 1
	AND Name = 'Pending'))
BEGIN 
	INSERT INTO [employer_account].[EmployerAgreementStatus](Id, Name) 
	VALUES(1, 'Pending') 
END 
ELSE 
BEGIN 
	UPDATE [employer_account].[EmployerAgreementStatus] 
	SET Name = 'Pending'
	WHERE Id = 1
END 
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementStatus] WHERE Id = 2
	AND Name = 'Signed'))
BEGIN 
	INSERT INTO [employer_account].[EmployerAgreementStatus](Id, Name) 
	VALUES(2, 'Signed') 
END 
ELSE 
BEGIN 
	UPDATE [employer_account].[EmployerAgreementStatus] 
	SET Name = 'Signed'
	WHERE Id = 2
END 
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementStatus] WHERE Id = 3
	AND Name = 'Expired'))
BEGIN 
	INSERT INTO [employer_account].[EmployerAgreementStatus](Id, Name) 
	VALUES(3, 'Expired') 
END 
ELSE 
BEGIN 
	UPDATE [employer_account].[EmployerAgreementStatus] 
	SET Name = 'Expired'
	WHERE Id = 3
END 
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementStatus] WHERE Id = 4
	AND Name = 'Superceded'))
BEGIN 
	INSERT INTO [employer_account].[EmployerAgreementStatus](Id, Name) 
	VALUES(4, 'Superceded') 
END 
ELSE 
BEGIN 
	UPDATE [employer_account].[EmployerAgreementStatus] 
	SET Name = 'Superceded'
	WHERE Id = 4
END 
SET IDENTITY_INSERT  [employer_account].[EmployerAgreementStatus] OFF


-- EmployerAgreement Template
IF (NOT EXISTS(SELECT * FROM [employer_account].[EmployerAgreementTemplate] WHERE Id = 1
	AND [Ref] = 'SFA Employer Agreement V1.0BETA'))
BEGIN 
	INSERT INTO [employer_account].[EmployerAgreementTemplate](Ref, [Text], CreatedDate, ReleasedDate, ExpiryDays) 
	VALUES('SFA Employer Agreement V1.0BETA','Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce nunc eros, posuere at lectus sollicitudin, efficitur malesuada odio. Curabitur varius mauris sit amet fringilla consequat. Integer porta id augue eu pretium. Sed quis sem vitae orci tincidunt vehicula. Nam sit amet ante metus. Suspendisse et elit varius, euismod odio quis, elementum erat. Nam imperdiet at ipsum vitae molestie. Mauris nisl diam, congue ultrices pretium vitae, congue vitae magna.

Nam ut hendrerit velit. Nam vitae quam rhoncus, tempor eros nec, sagittis quam. Etiam aliquet lectus in varius sodales. Vestibulum volutpat orci eu dui faucibus rutrum ut sed ante. Etiam molestie, quam ac ultricies lacinia, erat nunc tempus enim, vitae elementum libero tortor vitae urna. Integer eu eros mattis, euismod ligula in, venenatis nisi. Praesent ultricies nulla sed enim tristique accumsan. Etiam bibendum nulla vel bibendum facilisis. Suspendisse et tellus in dui vehicula ullamcorper nec nec libero. Curabitur ac ex eget elit aliquet rutrum. Ut lacinia, tellus vitae mattis eleifend, metus arcu pharetra massa, vitae placerat velit sapien sit amet nulla. Fusce id mi egestas, facilisis justo in, scelerisque leo. Vivamus lorem nisl, egestas in pulvinar at, facilisis et leo.

Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nulla consequat ac justo eu accumsan. Duis viverra pharetra elit ut mollis. Ut ultricies viverra mollis. Praesent vel nunc nec magna scelerisque lobortis. Pellentesque tempor venenatis nisl ut scelerisque. Mauris pharetra sapien quis metus dictum suscipit. Morbi mattis velit ante, id ullamcorper sapien tempus vel. In consequat aliquam elit, in congue purus tempor non. Fusce sit amet ligula erat.

Nunc ut lectus vitae mi varius viverra porta at tortor. Fusce cursus nunc ac neque maximus, a sollicitudin lectus cursus. Phasellus in purus augue. Suspendisse egestas commodo dolor, tempor rutrum tortor gravida in. Ut feugiat in ligula nec malesuada. Pellentesque posuere imperdiet mi. Cras sed odio ante. Aliquam rutrum consectetur urna, eget dictum massa maximus vitae. Donec in pretium metus, sed porta tortor.

Mauris purus nisl, fermentum et est sit amet, dapibus pretium massa. Fusce ultrices massa vitae diam varius, eu pharetra felis gravida. Quisque interdum ac est vitae ultricies. Nunc efficitur nibh et urna varius, nec ullamcorper quam faucibus. Nunc vel lectus rhoncus, faucibus lectus ac, rhoncus nisi. Donec tempor id erat in condimentum. Sed volutpat elementum neque sed sagittis. Aliquam erat volutpat. Maecenas sit amet sapien ut mi ornare imperdiet nec ac diam. Nullam eleifend dictum tellus, luctus convallis ligula. Pellentesque in tortor a libero posuere pellentesque. Donec consectetur lacus at dignissim egestas. Fusce gravida rutrum ex a cursus.', GETDATE(), GETDATE(), 30) 
END 
ELSE 
BEGIN 
	UPDATE [employer_account].[EmployerAgreementTemplate] 
	SET [Text] = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce nunc eros, posuere at lectus sollicitudin, efficitur malesuada odio. Curabitur varius mauris sit amet fringilla consequat. Integer porta id augue eu pretium. Sed quis sem vitae orci tincidunt vehicula. Nam sit amet ante metus. Suspendisse et elit varius, euismod odio quis, elementum erat. Nam imperdiet at ipsum vitae molestie. Mauris nisl diam, congue ultrices pretium vitae, congue vitae magna.

Nam ut hendrerit velit. Nam vitae quam rhoncus, tempor eros nec, sagittis quam. Etiam aliquet lectus in varius sodales. Vestibulum volutpat orci eu dui faucibus rutrum ut sed ante. Etiam molestie, quam ac ultricies lacinia, erat nunc tempus enim, vitae elementum libero tortor vitae urna. Integer eu eros mattis, euismod ligula in, venenatis nisi. Praesent ultricies nulla sed enim tristique accumsan. Etiam bibendum nulla vel bibendum facilisis. Suspendisse et tellus in dui vehicula ullamcorper nec nec libero. Curabitur ac ex eget elit aliquet rutrum. Ut lacinia, tellus vitae mattis eleifend, metus arcu pharetra massa, vitae placerat velit sapien sit amet nulla. Fusce id mi egestas, facilisis justo in, scelerisque leo. Vivamus lorem nisl, egestas in pulvinar at, facilisis et leo.

Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nulla consequat ac justo eu accumsan. Duis viverra pharetra elit ut mollis. Ut ultricies viverra mollis. Praesent vel nunc nec magna scelerisque lobortis. Pellentesque tempor venenatis nisl ut scelerisque. Mauris pharetra sapien quis metus dictum suscipit. Morbi mattis velit ante, id ullamcorper sapien tempus vel. In consequat aliquam elit, in congue purus tempor non. Fusce sit amet ligula erat.

Nunc ut lectus vitae mi varius viverra porta at tortor. Fusce cursus nunc ac neque maximus, a sollicitudin lectus cursus. Phasellus in purus augue. Suspendisse egestas commodo dolor, tempor rutrum tortor gravida in. Ut feugiat in ligula nec malesuada. Pellentesque posuere imperdiet mi. Cras sed odio ante. Aliquam rutrum consectetur urna, eget dictum massa maximus vitae. Donec in pretium metus, sed porta tortor.

Mauris purus nisl, fermentum et est sit amet, dapibus pretium massa. Fusce ultrices massa vitae diam varius, eu pharetra felis gravida. Quisque interdum ac est vitae ultricies. Nunc efficitur nibh et urna varius, nec ullamcorper quam faucibus. Nunc vel lectus rhoncus, faucibus lectus ac, rhoncus nisi. Donec tempor id erat in condimentum. Sed volutpat elementum neque sed sagittis. Aliquam erat volutpat. Maecenas sit amet sapien ut mi ornare imperdiet nec ac diam. Nullam eleifend dictum tellus, luctus convallis ligula. Pellentesque in tortor a libero posuere pellentesque. Donec consectetur lacus at dignissim egestas. Fusce gravida rutrum ex a cursus.',
		CreatedDate = GETDATE(),
		Ref = 'T/1',
		ReleasedDate = GETDATE()
	WHERE Id = 1
END 



