CREATE PROCEDURE [dbo].[GetEmployerAgreementsLinkedToAccount]
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
	FROM [dbo].[LegalEntity] le
		JOIN [dbo].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [dbo].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
		JOIN [dbo].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
	WHERE aea.AccountId = @accountId
END
