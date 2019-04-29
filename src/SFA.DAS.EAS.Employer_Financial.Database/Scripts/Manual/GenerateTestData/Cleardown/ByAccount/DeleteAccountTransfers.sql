--note: currently deletes transfer related rows, but currently leaves the corresponding payment transactionline for the receiver (that CreateTransferPayments generates)
--todo: ^^ delete also

DECLARE @senderAccountId bigint = 0
DECLARE @receiverAccountId bigint = 1

DELETE employer_financial.AccountTransfers WHERE SenderAccountId = @senderAccountId
DELETE employer_financial.PaymentMetaData WHERE Id IN (SELECT PaymentMetaDataId FROM employer_financial.Payment WHERE AccountId = @receiverAccountId AND FundingSource = /*LevyTransfer*/'5')
--note: transactionType is /*Levy*/1, not /*Transfer*/4, so we pick up transfer rows by FundingSource
DELETE employer_financial.Payment WHERE AccountId = @receiverAccountId AND FundingSource = /*LevyTransfer*/'5'
DELETE employer_financial.TransactionLine WHERE TransferSenderAccountId = @senderAccountId --AND TransactionType in (/*Transfer*/4)