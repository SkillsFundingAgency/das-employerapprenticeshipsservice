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
		eat.PartialViewName AS TemplatePartialViewName,
		le.DateOfIncorporation as LegalEntityInceptionDate,
		le.Status as LegalEntityStatus,
		le.Sector,
		le.[Source] as LegalEntitySource,
		ea.AccountLegalEntityId,
		ale.PublicHashedId as AccountLegalEntityPublicHashedId
	FROM [employer_account].[EmployerAgreement] ea
		JOIN [employer_account].[AccountLegalEntity] ale
			ON ale.Id = ea.AccountLegalEntityId
			   AND ale.Deleted IS NULL
		JOIN [employer_account].[LegalEntity] le
			ON le.Id = ale.LegalEntityId
		JOIN [employer_account].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
		join [employer_account].Account acc
			on	acc.Id = ale.AccountId
	WHERE ea.Id = @agreementId;

END;