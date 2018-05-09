CREATE PROCEDURE [employer_account].[GetAccountsLinkedToLegalEntity]
	@legalEntityId AS bigint,
	@withoutAgreementVersion as int = null
AS
BEGIN
	SET NOCOUNT ON

	SELECT	DISTINCT ea.AccountId
	FROM	employer_account.EmployerAgreement AS ea
	WHERE	ea.LegalEntityId = @legalEntityId
			AND EXISTS(	SELECT	1 
						FROM		employer_account.EmployerAgreement 
						WHERE	LegalEntityId = EA.LegalEntityId 
								AND AccountId = EA.AccountId 
								AND StatusId != 5)
			AND ( @withoutAgreementVersion is null 
					OR NOT EXISTS(SELECT		1 
									FROM		employer_account.EmployerAgreement 
									WHERE	LegalEntityId = EA.LegalEntityId 
											AND AccountId = ea.AccountId
											AND TemplateId >= @withoutAgreementVersion)
				)

END

