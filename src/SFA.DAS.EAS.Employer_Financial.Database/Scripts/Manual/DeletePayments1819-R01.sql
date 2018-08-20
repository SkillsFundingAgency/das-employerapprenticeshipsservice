DELETE FROM [employer_financial].[Payment] WHERE PeriodEnd = '1819-R01'
DELETE FROM [employer_financial].[PaymentMetaData] WHERE Id NOT IN (SELECT PaymentMetaDataId FROM [employer_financial].[Payment])
DELETE FROM [employer_financial].[TransactionLine] WHERE PeriodEnd = '1819-R01'
DELETE FROM [employer_financial].[PeriodEnd] WHERE PeriodEndId = '1819-R01'