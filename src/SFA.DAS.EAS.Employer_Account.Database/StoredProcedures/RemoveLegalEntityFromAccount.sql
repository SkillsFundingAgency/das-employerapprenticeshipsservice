CREATE PROCEDURE [employer_account].[RemoveLegalEntityFromAccount]
	@employerAgreementId BIGINT 
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE	@accountLegalEntityId as BIGINT

	SELECT	@accountLegalEntityId = accountLegalEntityId
	FROM	employer_account.EmployerAgreement AS EA
	WHERE	EA.Id = @employerAgreementId

	UPDATE	[employer_account].[EmployerAgreement] 
	SET		StatusId = 5
	WHERE	AccountLegalEntityId = @accountLegalEntityId

	UPDATE	[employer_account].[AccountLegalEntity] 
	SET		Deleted = GetUtcDate()
	WHERE	id = @accountLegalEntityId

END;

