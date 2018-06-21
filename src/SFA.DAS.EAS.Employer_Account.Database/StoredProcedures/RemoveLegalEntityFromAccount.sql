CREATE PROCEDURE [employer_account].[RemoveLegalEntityFromAccount]
	@employerAgreementId BIGINT 
AS
BEGIN

	SET NOCOUNT ON;

	UPDATE	ea
	SET		StatusId = 5
	FROM	[employer_account].[EmployerAgreement] ea
			JOIN [employer_account].[EmployerAgreement] ea2 
				ON ea2.AccountLegalEntityId = ea.AccountLegalEntityId
	WHERE	ea2.Id = @employerAgreementId;

END;

