DELETE meta FROM [employer_financial].[PaymentMetaData] meta
INNER JOIN [employer_financial].[Payment] p ON p.PaymentMetaDataId = meta.Id
WHERE p.FundingSource = 4

DELETE FROM [employer_financial].[Payment]
WHERE FundingSource = 4