CREATE PROCEDURE [employer_financial].[GetLastKnownProviderNameForUkprn] @ukprn BIGINT
AS
		SELECT TOP 1 ProviderName
		FROM [employer_financial].[PaymentMetaData] AS pmd
		JOIN [employer_financial].[Payment] AS p
		ON p.PaymentMetaDataId = pmd.Id
		WHERE p.Ukprn = @ukprn AND pmd.ProviderName IS NOT NULL
		ORDER BY p.CollectionPeriodYear DESC,p.CollectionPeriodMonth DESC

