CREATE PROCEDURE [employer_account].[GetAccountsLinkedToLegalEntity]
	@legalEntityId AS bigint,
	@withoutAgreementVersion as int = null
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	ALE.AccountId
	FROM	employer_account.AccountLegalEntity AS ALE
	WHERE	ALE.LegalEntityId = @legalEntityId
			AND ALE.Deleted IS NULL
			AND EXISTS(	SELECT	1 
						FROM	employer_account.EmployerAgreement AS EA
						WHERE	EA.AccountLegalEntityId = ALE.Id
								AND StatusId != 5)
			AND ( @withoutAgreementVersion is null 
					OR NOT EXISTS(SELECT	1 
									FROM	employer_account.EmployerAgreement AS EA
									WHERE	EA.AccountLegalEntityId = ALE.Id
											AND TemplateId >= @withoutAgreementVersion)
				)
END

