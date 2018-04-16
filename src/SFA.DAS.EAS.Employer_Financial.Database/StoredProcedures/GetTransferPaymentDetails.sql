CREATE PROCEDURE [employer_financial].[GetTransferPaymentDetails]
	@senderAccountId BIGINT,
	@periodEnd NVARCHAR(20),
	@apprenticeshipId BIGINT
AS
	SELECT 
		meta.ApprenticeshipCourseName as CourseName
		,COUNT(DISTINCT Uln) as ApprenticeCount
		,SUM(p.Amount) as PaymentTotal		
	FROM [employer_financial].[Payment] p
	INNER JOIN [employer_financial].[PaymentMetaData] meta ON p.PaymentMetaDataId = meta.Id
	WHERE p.AccountId = @senderAccountId
	AND PeriodEnd = @periodEnd
	AND ApprenticeshipId = @apprenticeshipId
	GROUP BY meta.ApprenticeshipCourseName
