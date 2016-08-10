CREATE PROCEDURE [dbo].[GetEmployerAgreementsLinkedToAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ea.Id,
		ea.StatusId,
		ea.LegalEntityId, 
		le.Name AS LegalEntityName,
		ea.TemplateId
	FROM [dbo].[LegalEntity] le
		JOIN [dbo].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [dbo].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id	
	WHERE aea.AccountId = @accountId
END
