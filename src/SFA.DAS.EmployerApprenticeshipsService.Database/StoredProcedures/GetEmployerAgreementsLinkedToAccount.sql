CREATE PROCEDURE [account].[GetEmployerAgreementsLinkedToAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ea.Id,
		ea.StatusId AS [Status],
		ea.LegalEntityId, 
		le.Name AS LegalEntityName,
		ea.TemplateId,
		eat.Ref AS TemplateRef
	FROM [account].[LegalEntity] le
		JOIN [account].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [account].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
		JOIN [account].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
	WHERE aea.AccountId = @accountId
END
