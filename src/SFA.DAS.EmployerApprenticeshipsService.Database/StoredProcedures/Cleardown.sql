CREATE PROCEDURE [dbo].[Cleardown]
	@INCLUDEUSERTABLE TINYINT = 0
AS
	DELETE FROM [dbo].[AccountEmployerAgreement];
	DELETE FROM [dbo].[EmployerAgreement];
	DELETE FROM [dbo].[Invitation];
	DELETE FROM [dbo].[Membership];

	IF @INCLUDEUSERTABLE = 1
	BEGIN
		DELETE FROM [dbo].[User];
	END

	DELETE FROM [dbo].[LevyDeclaration];
	DELETE FROM [dbo].[EnglishFraction];
	DELETE FROM [dbo].[Paye];
	DELETE FROM [dbo].[LegalEntity];
	DELETE FROM [dbo].[Account];
	DELETE FROM [dbo].[Role];
RETURN 0
