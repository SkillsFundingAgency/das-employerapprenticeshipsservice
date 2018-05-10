DELETE FROM [employer_financial].[Payment] WHERE PeriodEnd = '1718-R04'
DELETE FROM [employer_financial].[PaymentMetaData] WHERE Id NOT IN (SELECT PaymentMetaDataId FROM [employer_financial].[Payment])
DELETE FROM [employer_financial].[TransactionLine] WHERE PeriodEnd = '1718-R04'
DELETE FROM [employer_financial].[PeriodEnd] WHERE PeriodEndId = '1718-R04'