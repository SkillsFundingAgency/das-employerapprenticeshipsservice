CREATE PROCEDURE [dbo].[Cleardown]
AS
	DELETE FROM [dbo].[Membership];
	DELETE FROM [dbo].[User];
	--DBCC CHECKIDENT("[dbo].[User]", RESEED, 0);
	DELETE FROM [dbo].[LevyDeclaration];
	DELETE FROM  [dbo].[EnglishFraction];

	DELETE FROM [dbo].[Paye];
	DELETE FROM [dbo].[Account];
	DELETE FROM [dbo].[Role];
	--DBCC CHECKIDENT("[dbo].[Account]", RESEED, 0);
RETURN 0
