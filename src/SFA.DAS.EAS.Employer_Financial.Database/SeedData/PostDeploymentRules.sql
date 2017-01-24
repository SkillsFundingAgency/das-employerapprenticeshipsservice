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

IF (@@servername NOT LIKE '%pp%' AND @@servername NOT LIKE '%prd%')
	BEGIN
	   RAISERROR('Server %s is in development - seeding test data.',10,1,@@servername) WITH NOWAIT
	   :r .\SeedData.sql
	END
ELSE
	BEGIN
		RAISERROR('Server %s is managed - seeding referential data only.',10,1,@@servername) WITH NOWAIT
		:r .\SeedProdData.sql
	END