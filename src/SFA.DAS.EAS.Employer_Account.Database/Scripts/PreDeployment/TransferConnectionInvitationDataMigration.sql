IF EXISTS (
	SELECT 1
	FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_SCHEMA = 'employer_account'
	AND TABLE_NAME = 'TransferConnectionInvitation'
) AND NOT EXISTS (
	SELECT 1
	FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_SCHEMA = 'employer_account'
	AND TABLE_NAME = 'TransferConnectionInvitationChange'
)
BEGIN
	SELECT * INTO [employer_account].[TransferConnectionInvitationV1] FROM [employer_account].[TransferConnectionInvitation]
	TRUNCATE TABLE [employer_account].[TransferConnectionInvitation]
END