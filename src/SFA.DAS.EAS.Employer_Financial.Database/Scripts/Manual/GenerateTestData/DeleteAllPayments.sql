-- danger: here be dragons!
-- deletes *all* payments (leaves transfers alone) and periodends!
delete employer_financial.Payment
delete employer_financial.PaymentMetaData
delete employer_financial.PeriodEnd
delete employer_financial.TransactionLine where TransactionType in (3)