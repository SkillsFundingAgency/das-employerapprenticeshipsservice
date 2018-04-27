CREATE PROCEDURE [employer_financial].[GetStatistics]
AS
	SELECT COUNT(PaymentId) AS TotalPayments
	FROM [employer_financial].[Payment]
	WHERE CollectionPeriodYear = YEAR(GETDATE())

