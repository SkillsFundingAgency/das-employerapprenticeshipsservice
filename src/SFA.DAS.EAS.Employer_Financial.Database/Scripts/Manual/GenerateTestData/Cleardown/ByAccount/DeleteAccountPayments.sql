DECLARE @accountId bigint = 0

DELETE employer_financial.PaymentMetaData WHERE Id IN (SELECT PaymentMetaDataId FROM employer_financial.Payment WHERE AccountId = @accountId)
DELETE employer_financial.Payment WHERE AccountId = @accountId
DELETE employer_financial.TransactionLine WHERE AccountId = @accountId AND TransactionType IN (3)