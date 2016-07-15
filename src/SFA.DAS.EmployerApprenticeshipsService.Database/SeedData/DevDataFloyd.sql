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

SET IDENTITY_INSERT  [dbo].[Role] ON 

INSERT INTO [dbo].[Role] (Id, Name) VALUES (1, 'Owner');
SET IDENTITY_INSERT [dbo].[Role] OFF 
GO 

SET IDENTITY_INSERT  [dbo].Account ON
INSERT INTO [dbo].[Account] (Id, Name) VALUES (1, 'Floyd Price Ltd')

SET IDENTITY_INSERT  [dbo].Account OFF
GO
 
SET IDENTITY_INSERT  [dbo].[User] ON
INSERT INTO [dbo].[User] (Id, PireanKey, Email) VALUES( 1, '758943A5-86AA-4579-86AF-FB3D4A05850B', 'floyd.price@test.local');
 
SET IDENTITY_INSERT  [dbo].[User] OFF
GO

INSERT INTO [dbo].Membership (RoleId, UserId, AccountId) VALUES (1, 1, 1);


INSERT INTO [dbo].[Paye] (AccountId, Ref) VALUES (1, '123-ab12345');
INSERT INTO [dbo].[Paye] (AccountId, Ref) VALUES (1, '456-cd45678');


Go


