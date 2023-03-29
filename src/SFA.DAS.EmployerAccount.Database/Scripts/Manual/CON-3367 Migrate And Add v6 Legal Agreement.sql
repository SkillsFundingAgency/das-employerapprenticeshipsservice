-- Replace all pending v3 / v4 / v5 agreements with pending v6 agreement
UPDATE [employer_account].[EmployerAgreement] 
SET    TemplateId = 7
WHERE  TemplateId IN (4, 5, 6) AND StatusId = 1

UPDATE [employer_account].[AccountLegalEntity]
SET    PendingAgreementVersion = 6
WHERE  PendingAgreementVersion IN (3, 4, 5)

-- Create a pending v6 agreement for all account legal entities (except those that have them)
INSERT INTO [employer_account].[EmployerAgreement] (TemplateId, StatusId, AccountLegalEntityId)
SELECT 7, 1, Id
FROM   [employer_account].[AccountLegalEntity] 
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL AND SignedAgreementVersion != 6

UPDATE ale
SET    PendingAgreementVersion = 6, PendingAgreementId = ea.Id
FROM   [employer_account].[AccountLegalEntity] ale 
JOIN   [employer_account].[EmployerAgreement] ea ON ea.AccountLegalEntityId = ale.Id AND ea.TemplateId = 7 AND ea.StatusId = 1
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL