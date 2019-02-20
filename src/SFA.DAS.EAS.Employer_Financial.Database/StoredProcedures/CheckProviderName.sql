CREATE PROCEDURE [employer_financial].[CheckProviderName] @providerName NVARCHAR(MAX), @ukprn BIGINT, @output NVARCHAR(MAX) OUTPUT, @detailsOut NVARCHAR(MAX) OUTPUT
AS
	IF @providerName IS NULL
	BEGIN
		SET @output = (SELECT TOP 1 ProviderName
		FROM [employer_financial].[PaymentMetaData] AS pmd
		JOIN [employer_financial].[Payment] AS p
		ON p.PaymentMetaDataId = pmd.Id
		WHERE p.Ukprn = @ukprn AND pmd.ProviderName IS NOT NULL
		ORDER BY p.CollectionPeriodYear DESC,p.CollectionPeriodMonth DESC)
		SET @detailsOut = 'ProviderName taken from historical data as API returned NULL'
	END
	ELSE
	BEGIN
		SET @output = @providerName
		SET @detailsOut = 'ProviderName taken from API'
	END