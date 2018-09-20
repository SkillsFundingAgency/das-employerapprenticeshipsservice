CREATE PROCEDURE [employer_financial].[GetProviderName]
@accountId BIGINT,
@ukprn BIGINT, 
@periodEnd NVARCHAR(MAX)
AS

SELECT m.ProviderName
FROM employer_financial.Payment p 
INNER JOIN employer_financial.PaymentMetaData m ON m.Id = p.PaymentMetaDataId
WHERE p.AccountId = @accountId
AND p.Ukprn = @ukprn
AND p.PeriodEnd = @periodEnd