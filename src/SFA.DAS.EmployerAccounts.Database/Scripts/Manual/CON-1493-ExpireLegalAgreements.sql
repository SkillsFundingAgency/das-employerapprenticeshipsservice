-- Set all v1 & v2 agreements to superseded (both pending and signed)
UPDATE [employer_account].[EmployerAgreement] 
SET    StatusId = 4,
	   ExpiredDate = GETDATE()
WHERE  StatusId IN (1, 2) AND TemplateId IN (1, 2)

UPDATE [employer_account].[AccountLegalEntity]
SET    SignedAgreementVersion = NULL, SignedAgreementId = NULL
WHERE  SignedAgreementVersion IN (1, 2) AND Deleted IS NULL

UPDATE [employer_account].[AccountLegalEntity]
SET    PendingAgreementVersion = NULL, PendingAgreementId = NULL
WHERE  PendingAgreementVersion IN (1, 2) AND Deleted IS NULL

-- Replace all pending v3 agreements with pending v4 agreement
UPDATE [employer_account].[EmployerAgreement] 
SET    TemplateId = 5
WHERE  TemplateId = 4 AND StatusId = 1

UPDATE [employer_account].[AccountLegalEntity]
SET    PendingAgreementVersion = 4
WHERE  PendingAgreementVersion = 3

-- Create a pending v4 agreement for all account legal entities (except those that have them)
INSERT INTO [employer_account].[EmployerAgreement] (TemplateId, StatusId, AccountLegalEntityId)
SELECT 5, 1, Id
FROM   [employer_account].[AccountLegalEntity] 
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL

UPDATE ale
SET    PendingAgreementVersion = 4, PendingAgreementId = ea.Id
FROM   [employer_account].[AccountLegalEntity] ale 
JOIN   [employer_account].[EmployerAgreement] ea ON ea.AccountLegalEntityId = ale.Id AND ea.TemplateId = 5 AND ea.StatusId = 1
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL