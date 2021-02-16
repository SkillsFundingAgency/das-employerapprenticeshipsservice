-- Replace all pending v3 / v4 agreements with pending v5 agreement
UPDATE [employer_account].[EmployerAgreement] 
SET    TemplateId = 6
WHERE  TemplateId IN (4, 5) AND StatusId = 1

UPDATE [employer_account].[AccountLegalEntity]
SET    PendingAgreementVersion = 5
WHERE  PendingAgreementVersion IN (3, 4)

-- Create a pending v5 agreement for all account legal entities (except those that have them)
INSERT INTO [employer_account].[EmployerAgreement] (TemplateId, StatusId, AccountLegalEntityId)
SELECT 6, 1, Id
FROM   [employer_account].[AccountLegalEntity] 
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL AND SignedAgreementVersion != 5

UPDATE ale
SET    PendingAgreementVersion = 5, PendingAgreementId = ea.Id
FROM   [employer_account].[AccountLegalEntity] ale 
JOIN   [employer_account].[EmployerAgreement] ea ON ea.AccountLegalEntityId = ale.Id AND ea.TemplateId = 6 AND ea.StatusId = 1
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL