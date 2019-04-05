-- danger: here be dragons!
-- deletes *all* payments (leaves transfers alone), periodends adn AccountTransfers!
delete employer_financial.AccountTransfers
delete employer_financial.Payment
delete employer_financial.PaymentMetaData
delete employer_financial.PeriodEnd
delete employer_financial.TransactionLine where TransactionType in (/*Payment*/ 3, /*Transfer*/ 4)