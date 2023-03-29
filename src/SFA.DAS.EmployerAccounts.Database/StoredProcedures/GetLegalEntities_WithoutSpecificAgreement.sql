CREATE PROCEDURE [employer_account].[GetLegalEntities_WithoutSpecificAgreement]
	@firstId AS BigInt,
	@count AS int,
	@agreementId AS int
AS

BEGIN
	SET NOCOUNT ON;

	SELECT	DISTINCT ale.LegalEntityId
	FROM	AccountLegalEntity AS ale
	WHERE	ale.LegalEntityId >= @firstId AND
			ale.Deleted IS NULL AND
			NOT EXISTS(SELECT 1 FROM employer_account.EmployerAgreement AS ea WHERE ea.AccountLegalEntityId = ale.Id AND ea.TemplateId >= @agreementId AND ea.StatusId != 5)
	ORDER BY ale.LegalEntityId ;

END