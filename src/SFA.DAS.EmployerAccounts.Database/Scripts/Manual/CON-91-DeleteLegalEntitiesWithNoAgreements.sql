UPDATE ale
SET Deleted = GETUTCDATE()
FROM [employer_account].[EmployerAgreement] AS ea
JOIN [employer_account].[AccountLegalEntity] AS ale
ON ea.AccountLegalEntityId = ale.Id
WHERE ea.StatusId = 5 AND ale.Deleted IS NULL AND ale.PendingAgreementId IS NULL AND ale.SignedAgreementId IS NULL