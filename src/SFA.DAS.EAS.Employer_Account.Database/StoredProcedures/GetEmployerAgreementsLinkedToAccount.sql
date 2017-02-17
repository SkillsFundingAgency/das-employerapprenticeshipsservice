CREATE PROCEDURE [employer_account].[GetEmployerAgreementsLinkedToAccount]
	@accountId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
	ea.Id,
	ea.StatusId AS [Status],
	ea.LegalEntityId, 
	le.Name AS LegalEntityName,
	ea.TemplateId,
	eat.PartialViewName AS TemplatePartialViewName,
	ea.SignedByName
	FROM [employer_account].[LegalEntity] le
	JOIN [employer_account].[EmployerAgreement] ea ON ea.LegalEntityId = le.Id
	JOIN [employer_account].[AccountEmployerAgreement] aea ON aea.[EmployerAgreementId] = ea.Id
	JOIN [employer_account].[EmployerAgreementTemplate] eat ON eat.Id = ea.TemplateId
	WHERE aea.AccountId = @accountId
END
