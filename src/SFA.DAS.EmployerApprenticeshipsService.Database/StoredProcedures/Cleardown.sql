CREATE PROCEDURE [account].[Cleardown]
	@INCLUDEUSERTABLE TINYINT = 0
AS
	DELETE FROM [account].[AccountEmployerAgreement];
	DELETE FROM [account].[EmployerAgreement];
	DELETE FROM [account].[Invitation];
	DELETE FROM [account].[Membership];

	IF @INCLUDEUSERTABLE = 1
	BEGIN
		DELETE FROM [account].[User];
	END

	DELETE FROM [account].[LevyDeclaration];
	DELETE FROM [account].[EnglishFraction];
	DELETE FROM [account].[Paye];
	DELETE FROM [account].[LegalEntity];
	DELETE FROM [account].[Account];
	DELETE FROM [account].[Role];
	DELETE FROM [account].[EmployerAgreementTemplate];
RETURN 0
