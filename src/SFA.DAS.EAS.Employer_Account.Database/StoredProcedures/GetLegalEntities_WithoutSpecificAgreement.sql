CREATE PROCEDURE [employer_account].[GetLegalEntities_WithoutSpecificAgreement]
	@firstId AS BigInt,
	@count AS int,
	@agreementId AS int
AS

BEGIN
	SET NOCOUNT ON;

	SELECT	TOP(@count) LegalEntityId
	FROM		(SELECT		DISTINCT EA.LegalEntityId
				FROM		employer_account.EmployerAgreement AS EA
				WHERE	EA.LegalEntityId > @firstId
						AND EXISTS(	SELECT	1 
									FROM		employer_account.EmployerAgreement 
									WHERE	LegalEntityId = EA.LegalEntityId 
											AND AccountId = EA.AccountId 
											AND StatusId != 5)
						AND EXISTS(	SELECT	1 
									FROM		employer_account.EmployerAgreement 
									WHERE	LegalEntityId = EA.LegalEntityId 
									GROUP BY AccountId 
									HAVING MAX(TemplateId) < @agreementId)
			) AS AffectedLegalEntities
	ORDER BY AffectedLegalEntities.LegalEntityId 

END