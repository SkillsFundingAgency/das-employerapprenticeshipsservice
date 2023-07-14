SET NOCOUNT ON;

PRINT 'Initial set of NameConfirmed flag for existing accounts';

BEGIN TRAN;

UPDATE employer_account.Account
SET NameConfirmed = 1
WHERE NameConfirmed IS NULL
AND [Name] <> 'MY ACCOUNT'

COMMIT TRAN;