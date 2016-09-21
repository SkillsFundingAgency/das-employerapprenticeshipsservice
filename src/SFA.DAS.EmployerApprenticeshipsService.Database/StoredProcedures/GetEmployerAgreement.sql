CREATE PROCEDURE [account].[GetEmployerAgreement]
	@agreementId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ea.Id,
		aea.AccountId,
		ea.StatusId AS [Status],
		ea.LegalEntityId, 
		u.FirstName + ' ' + u.LastName AS SignedName,
		ea.SignedDate,
		ea.ExpiredDate,
		le.Name AS LegalEntityName,
		le.RegisteredAddress AS LegalEntityRegisteredAddress,
		ea.TemplateId,
		eat.[Text] AS TemplateText
	FROM [account].[LegalEntity] le
		JOIN [account].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [account].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
		JOIN [account].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
		LEFT OUTER JOIN [account].[User] u
			ON u.Id = ea.SignedById
	WHERE ea.Id = @agreementId
END