CREATE PROCEDURE [employer_financial].[GetPaymentDetail_ByAccountProviderAndDateRange]
    @accountId BIGINT,
    @ukprn BIGINT,
    @fromDate DATETIME,
    @toDate DATETIME
AS
	SELECT
		@accountId AS AccountId,
		@ukprn AS Ukprn,
		p.PeriodEnd,
		3 AS TransactionType,
		pm.ApprenticeshipCourseName AS CourseName,
		pm.ApprenticeshipCourseLevel AS CourseLevel,
		pm.PathwayName,
		pm.PathwayCode,
		MAX(pm.ApprenticeshipCourseStartDate) AS CourseStartDate,
		MAX(pm.ProviderName) AS ProviderName,
		MIN(t.DateCreated) AS DateCreated,
		SUM(CASE WHEN p.FundingSource IN (1, 5) THEN p.Amount END) * -1 AS LineAmount,
		SUM(CASE WHEN p.FundingSource = 2 THEN p.Amount END) * -1 AS SfaCoInvestmentAmount,
		SUM(CASE WHEN p.FundingSource = 3 THEN p.Amount END) * -1 AS EmployerCoInvestmentAmount
	FROM [employer_financial].[Payment] p
	LEFT JOIN [employer_financial].[PaymentMetaData] pm ON pm.Id = p.PaymentMetaDataId
	INNER JOIN [employer_financial].[TransactionLine] t ON t.AccountId = p.AccountId AND t.PeriodEnd = p.PeriodEnd AND t.Ukprn = p.Ukprn
	WHERE p.AccountId = @accountId
	AND p.Ukprn = @ukprn
	AND p.FundingSource IN (1, 2, 3, 5)
	AND t.DateCreated >= @fromDate
	AND t.DateCreated <= @toDate
	GROUP BY p.PeriodEnd, pm.ApprenticeshipCourseName, pm.ApprenticeshipCourseLevel, pm.PathwayName, pm.PathwayCode
