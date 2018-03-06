CREATE PROCEDURE [employer_account].[GetLegalEntities_WithoutSpecificAgreement]
	@firstId AS BigInt,
	@count as int,
	@agreementId as int
AS

BEGIN
	SET NOCOUNT ON;

	SELECT	TOP(@count) le.Id 
	  FROM	employer_account.LegalEntity as le
			LEFT JOIN employer_account.EmployerAgreement as ea
				ON ea.LegalEntityId = le.Id
					AND ea.TemplateId = @agreementId
	WHERE	ea.Id IS NULL
			AND le.id >= @firstId
	ORDER BY le.Id
END