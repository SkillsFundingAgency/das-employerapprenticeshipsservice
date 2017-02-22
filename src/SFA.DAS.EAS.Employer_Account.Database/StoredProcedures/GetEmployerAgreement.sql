CREATE PROCEDURE [employer_account].[GetEmployerAgreement]
	@agreementId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ea.Id,
		aea.AccountId,
		acc.HashedId as HashedAccountId,
		ea.StatusId AS [Status],
		ea.LegalEntityId, 
		ea.SignedByName,
		ea.SignedDate,
		ea.ExpiredDate,
		le.Name AS LegalEntityName,
		le.RegisteredAddress AS LegalEntityAddress,
		le.Code AS LegalEntityCode,
		ea.TemplateId,
		eat.PartialViewName AS TemplatePartialViewName
	FROM [employer_account].[LegalEntity] le
		JOIN [employer_account].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [employer_account].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
		JOIN [employer_account].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
		join [employer_account].Account acc
			on	acc.Id = aea.AccountId
	WHERE ea.Id = @agreementId
END