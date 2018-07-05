CREATE PROCEDURE [employer_account].[GetEmployerAgreement]
	@agreementId BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		ea.Id,
		ale.AccountId,
		acc.HashedId as HashedAccountId,
		ea.StatusId AS [Status],
		ale.LegalEntityId, 
		ea.SignedByName,
		ea.SignedDate,
		ea.ExpiredDate,
		ale.Name AS LegalEntityName,
		ale.Address AS LegalEntityAddress,
		le.Code AS LegalEntityCode,
		ea.TemplateId,
		eat.PartialViewName AS TemplatePartialViewName
	FROM [employer_account].[EmployerAgreement] ea
		JOIN [employer_account].[AccountLegalEntity] ale
			ON ale.Id = ea.AccountLegalEntityId
		JOIN [employer_account].[LegalEntity] le
			ON le.Id = ale.LegalEntityId
		JOIN [employer_account].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
		join [employer_account].Account acc
			on	acc.Id = ale.AccountId
	WHERE ea.Id = @agreementId;

END;