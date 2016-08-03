CREATE PROCEDURE [dbo].[Cleardown]
AS
	DELETE FROM [dbo].[Invitation];
	DELETE FROM [dbo].[Membership];
	--DELETE FROM [dbo].[User];
	DELETE FROM [dbo].[LevyDeclaration];
	DELETE FROM [dbo].[EnglishFraction];
	DELETE FROM [dbo].[Paye];
	DELETE FROM [dbo].[Account];
	DELETE FROM [dbo].[Role];
RETURN 0
