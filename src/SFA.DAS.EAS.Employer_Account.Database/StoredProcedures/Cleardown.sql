CREATE PROCEDURE [employer_account].[Cleardown]
	@includeUserTable BIT = 0
AS
	DELETE FROM [employer_account].[TransferRequest]
	DELETE FROM [employer_account].[TransferConnectionInvitationChange]
	DELETE FROM [employer_account].[TransferConnectionInvitation]
	DELETE FROM [employer_account].[UserAccountSettings]
	DELETE FROM [employer_account].[EmployerAgreement]
	DELETE FROM [employer_account].[Invitation]
	DELETE FROM [employer_account].[Membership]

	IF @includeUserTable = 1
	BEGIN
		DELETE FROM [employer_account].[User]
	END
		
	DELETE FROM [employer_account].[Paye]
	DELETE FROM [employer_account].[LegalEntity]
	DELETE FROM [employer_account].[AccountHistory]
	DELETE FROM [employer_account].[Account]
RETURN 0