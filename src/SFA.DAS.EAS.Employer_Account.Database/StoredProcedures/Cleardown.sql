CREATE PROCEDURE [employer_account].[Cleardown]
	@INCLUDEUSERTABLE TINYINT = 0
AS
	DELETE FROM [employer_account].[AccountEmployerAgreement];
	DELETE FROM [employer_account].[EmployerAgreement];
	DELETE FROM [employer_account].[Invitation];
	DELETE FROM [employer_account].[Membership];

	IF @INCLUDEUSERTABLE = 1
	BEGIN
		DELETE FROM [employer_account].[User];
	END
		
	DELETE FROM [employer_account].[Paye];
	DELETE FROM [employer_account].[LegalEntity];
	DELETE FROM [employer_account].[AccountHistory]
	DELETE FROM [employer_account].[Account];	
	DELETE FROM [employer_account].[EmployerAgreementTemplate];
RETURN 0
