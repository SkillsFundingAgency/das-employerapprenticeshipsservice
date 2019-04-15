-- danger: here be dragons!
-- deletes *all* AccountTransfers, Payments, PaymentMetaDatas, PeriodEnds and TransactionLines of type payment and tranfer

DELETE employer_financial.AccountTransfers
DELETE employer_financial.Payment
DELETE employer_financial.PaymentMetaData
DELETE employer_financial.PeriodEnd
DELETE employer_financial.TransactionLine WHERE TransactionType IN (/*Payment*/ 3, /*Transfer*/ 4)