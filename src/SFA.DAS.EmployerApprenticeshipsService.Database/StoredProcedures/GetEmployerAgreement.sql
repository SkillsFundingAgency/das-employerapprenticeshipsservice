CREATE PROCEDURE [dbo].[GetEmployerAgreement]
	@agreementId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ea.Id,
		aea.AccountId,
		ea.StatusId AS [Status],
		ea.LegalEntityId, 
		ea.SignedByName,
		ea.SignedDate,
		ea.ExpiredDate,
		le.Name AS LegalEntityName,
		le.RegisteredAddress AS LegalEntityRegisteredAddress,
		ea.TemplateId,
		eat.[Text] AS TemplateText
	FROM [dbo].[LegalEntity] le
		JOIN [dbo].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [dbo].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
		JOIN [dbo].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
	WHERE ea.Id = @agreementId
END