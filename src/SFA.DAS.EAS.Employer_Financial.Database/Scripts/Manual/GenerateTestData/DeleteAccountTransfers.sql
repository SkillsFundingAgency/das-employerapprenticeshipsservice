--Warning: work in progress, DON'T USE YET!

declare @accountId bigint = 0

delete employer_financial.AccountTransfers where SenderAccountId = @accountId
delete employer_financial.PaymentMetaData where Id in (select PaymentMetaDataId from employer_financial.Payment where AccountId = @accountId and FundingSource = /*LevyTransfer*/'5')
--note: transactionType is /*Levy*/1, not /*Transfer*/4, so we pick up transfer rows by FundingSource
delete employer_financial.Payment where AccountId = @accountId and FundingSource = /*LevyTransfer*/'5'
delete employer_financial.TransactionLine where AccountId = @accountId and TransactionType in (/*Transfer*/4)