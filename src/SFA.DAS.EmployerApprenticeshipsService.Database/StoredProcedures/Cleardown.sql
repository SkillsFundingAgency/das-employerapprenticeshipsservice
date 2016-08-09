CREATE PROCEDURE [dbo].[Cleardown]
AS
	DELETE FROM [dbo].[AccountEmployerAgreement];
	DELETE FROM [dbo].[EmployerAgreement];
	DELETE FROM [dbo].[Invitation];
	DELETE FROM [dbo].[Membership];
	--DELETE FROM [dbo].[User];
	DELETE FROM [dbo].[LevyDeclaration];
	DELETE FROM [dbo].[EnglishFraction];
	DELETE FROM [dbo].[Paye];
	DELETE FROM [dbo].[LegalEntity];
	DELETE FROM [dbo].[Account];
	DELETE FROM [dbo].[Role];
RETURN 0
