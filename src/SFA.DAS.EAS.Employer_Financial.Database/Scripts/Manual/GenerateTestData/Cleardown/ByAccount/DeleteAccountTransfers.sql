--note: currently deletes transfer related rows, but currently leaves the corresponding payment transactionline for the receiver (that CreateTransferPayments generates)
--todo: ^^ delete also

declare @senderAccountId bigint = 0
declare @receiverAccountId bigint = 1

delete employer_financial.AccountTransfers where SenderAccountId = @senderAccountId
delete employer_financial.PaymentMetaData where Id in (select PaymentMetaDataId from employer_financial.Payment where AccountId = @receiverAccountId and FundingSource = /*LevyTransfer*/'5')
--note: transactionType is /*Levy*/1, not /*Transfer*/4, so we pick up transfer rows by FundingSource
delete employer_financial.Payment where AccountId = @receiverAccountId and FundingSource = /*LevyTransfer*/'5'
delete employer_financial.TransactionLine where TransferSenderAccountId = @senderAccountId --and TransactionType in (/*Transfer*/4)