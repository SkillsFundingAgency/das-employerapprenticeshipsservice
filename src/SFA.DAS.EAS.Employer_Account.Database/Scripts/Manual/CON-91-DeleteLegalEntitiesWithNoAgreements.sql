UPDATE ae
SET StatusId = 5
FROM [employer_account].[EmployerAgreement] AS ae
JOIN [employer_account].[AccountLegalEntity] AS ale
ON ae.AccountLegalEntityId = ale.Id
WHERE (ale.SignedAgreementId IS NULL AND ale.PendingAgreementId IS NULL)

UPDATE [employer_account].[AccountLegalEntity]
SET Deleted = GetUtcDate()
WHERE (SignedAgreementId IS NULL AND PendingAgreementId IS NULL)