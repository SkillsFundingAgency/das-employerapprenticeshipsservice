-- Set all v3, v4, v5, v6 & v7 agreements to superseded (both pending and signed)
UPDATE [employer_account].[EmployerAgreement] 
SET    StatusId = 4,
	   ExpiredDate = GETDATE()
WHERE  StatusId IN (1, 2) AND TemplateId IN (3,4,5,6,7,8) 

UPDATE [employer_account].[AccountLegalEntity]
SET    SignedAgreementVersion = NULL, SignedAgreementId = NULL
WHERE  SignedAgreementVersion IN (3,4,5,6,7) AND Deleted IS NULL

UPDATE [employer_account].[AccountLegalEntity]
SET    PendingAgreementVersion = NULL, PendingAgreementId = NULL
WHERE  PendingAgreementVersion IN (3,4,5,6,7) AND Deleted IS NULL

-- Replace all pending v3, v4, v5, v6, v7 agreements with pending v8 agreement
UPDATE [employer_account].[AccountLegalEntity]
SET    PendingAgreementVersion = 8
WHERE  PendingAgreementVersion IN (3,4,5,6,7)

-- Create a pending v8 agreement for all account legal entities (except those that have them)
INSERT INTO [employer_account].[EmployerAgreement] (TemplateId, StatusId, AccountLegalEntityId)
SELECT 9, 1, Id
FROM   [employer_account].[AccountLegalEntity] 
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL

UPDATE ale
SET    PendingAgreementVersion = 8, PendingAgreementId = ea.Id
FROM   [employer_account].[AccountLegalEntity] ale 
JOIN   [employer_account].[EmployerAgreement] ea ON ea.AccountLegalEntityId = ale.Id AND ea.TemplateId = 9 AND ea.StatusId = 1
WHERE  PendingAgreementVersion IS NULL AND PendingAgreementId IS NULL AND Deleted IS NULL
