CREATE PROCEDURE [employer_financial].[CheckProviderName] @providerName NVARCHAR(MAX), @ukprn BIGINT
AS
	IF @providerName IS NULL
	BEGIN
		RETURN (SELECT TOP 1 ProviderName
		FROM [employer_financial].[PaymentMetaData] AS pmd
		JOIN [employer_financial].[Payment] AS p
		ON p.PaymentMetaDataId = pmd.Id
		WHERE p.Ukprn = @ukprn
		ORDER BY p.CollectionPeriodYear DESC,p.CollectionPeriodMonth DESC)
	END
	ELSE
	BEGIN
		RETURN @providerName
	END