CREATE PROCEDURE [employer_account].[GetAccountsLinkedToLegalEntity]
	@legalEntityId as bigint
AS
BEGIN
	SET NOCOUNT ON

	SELECT	DISTINCT ea.AccountId
	FROM		employer_account.EmployerAgreement as ea
	WHERE	ea.LegalEntityId = @legalEntityId
END
