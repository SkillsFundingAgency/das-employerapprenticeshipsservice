CREATE PROCEDURE [employer_account].[GetLegalEntities_WithoutSpecificAgreement]
	@firstId AS BigInt,
	@count AS int,
	@agreementId AS int
AS

BEGIN
	SET NOCOUNT ON;

	SELECT	TOP(@count) le.Id 
	  FROM	employer_account.LegalEntity AS le
			LEFT JOIN employer_account.EmployerAgreement AS ea
				ON ea.LegalEntityId = le.Id
					AND ea.TemplateId = @agreementId
	WHERE	ea.Id IS NULL
			AND le.id >= @firstId
	ORDER BY le.Id
END