CREATE PROCEDURE [employer_account].[GetEmployerAgreement]
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
		eat.PartialViewName AS TemplatePartialViewName
	FROM [employer_account].[LegalEntity] le
		JOIN [employer_account].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [employer_account].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
		JOIN [employer_account].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
	WHERE ea.Id = @agreementId
END