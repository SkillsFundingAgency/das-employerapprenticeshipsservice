CREATE PROCEDURE [employer_account].[RemoveLegalEntityFromAccount]
	@employerAgreementId BIGINT 
AS
	SET NOCOUNT ON

	UPDATE ea
	SET ea.StatusId = 5
	FROM [employer_account].[EmployerAgreement] ea
	INNER JOIN [employer_account].[EmployerAgreement] ea2 ON ea2.AccountId = ea.AccountId AND ea2.LegalEntityId = ea.LegalEntityId
	WHERE ea2.Id = @employerAgreementId