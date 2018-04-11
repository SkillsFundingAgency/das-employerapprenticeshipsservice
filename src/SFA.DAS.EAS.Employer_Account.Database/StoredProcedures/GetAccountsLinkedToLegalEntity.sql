CREATE PROCEDURE [employer_account].[GetAccountsLinkedToLegalEntity]
	@legalEntityId AS bigint
AS
BEGIN
	SET NOCOUNT ON

	SELECT	DISTINCT ea.AccountId
	FROM		employer_account.EmployerAgreement AS ea
	WHERE	ea.LegalEntityId = @legalEntityId
END
