declare @accountId bigint = 0

-- todo: delete transfers always, by flag/param, or not at all?

delete employer_financial.PaymentMetaData where Id in (select PaymentMetaDataId from employer_financial.Payment where AccountId = @accountId)
delete employer_financial.Payment where AccountId = @accountId
delete employer_financial.TransactionLine where AccountId = @accountId and TransactionType in (3)