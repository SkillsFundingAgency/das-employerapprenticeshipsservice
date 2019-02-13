CREATE PROCEDURE [employer_financial].[GetPaymentDetail_ByAccountProviderCourseAndDateRange]
	@AccountId BIGINT,
	@Ukprn BIGINT,
	@courseName NVARCHAR(MAX),
	@pathwayCode INT,
	@courseLevel INT,
	@fromDate DATETIME,
	@toDate DATETIME
AS	
SELECT TransactionDetails.*, 3 as TransactionType, Payments.*
FROM (
	-- HACK: the meta fields such as CourseName, Level etc should not be being aggregated like this. Nor should they be grouped.
	SELECT  
		p.[AccountId],
		MAX(p.PaymentId) as PaymentId,
		p.Ukprn,
		MAX(p.PeriodEnd) as PeriodEnd,
		MAX(meta.ProviderName) AS ProviderName,
		SUM(CASE WHEN p.FundingSource IN (1, 5) THEN -p.Amount ELSE 0 END) as LineAmount,
		MAX(meta.ApprenticeshipCourseName) as CourseName,
		MAX(meta.ApprenticeshipCourseLevel)	as CourseLevel,
		MAX(meta.PathwayName) as PathwayName,
		MAX(meta.ApprenticeshipCourseStartDate) as CourseStartDate,
		MAX(meta.ApprenticeName) as ApprenticeName,
		p.Uln AS ApprenticeULN,
		MAX(meta.ApprenticeNINumber) as ApprenticeNINumber,
		SUM(CASE WHEN p.FundingSource = 2 THEN -p.Amount ELSE 0 END) as SfaCoInvestmentAmount,
		SUM(CASE WHEN p.FundingSource = 3 THEN -p.Amount ELSE 0 END) as EmployerCoInvestmentAmount
	FROM [employer_financial].[TransactionLine] t
		JOIN [employer_financial].[Payment] p
			ON t.AccountId = p.AccountId AND t.PeriodEnd = p.PeriodEnd AND t.Ukprn = p.Ukprn
		LEFT JOIN [employer_financial].[PaymentMetaData] meta 
			ON	p.PaymentMetaDataId = meta.Id
	WHERE 
		p.AccountId = @AccountId
		AND p.Ukprn = @Ukprn 
		AND (
			@courseName IS NULL AND meta.ApprenticeshipCourseName IS NULL 
			OR meta.ApprenticeshipCourseName = @courseName
		) 
		AND	(
			@courseLevel IS NULL AND meta.ApprenticeshipCourseLevel IS NULL 
			OR meta.ApprenticeshipCourseLevel = @courseLevel
		) 
		AND ISNULL(meta.PathwayCode, -1) = ISNULL(@pathwayCode, -1) 
		AND	p.FundingSource IN (1, 2, 3, 5)
		AND t.AccountId = @AccountId 
						AND t.Ukprn = @Ukprn
						AND t.DateCreated BETWEEN @fromDate AND @toDate
	GROUP BY 
		p.AccountId, 
		p.Ukprn, 
		meta.ApprenticeshipCourseName, 
		meta.ApprenticeshipCourseLevel, 
		meta.PathwayName, 
		p.Uln,
		meta.ApprenticeName) AS Payments


OUTER APPLY (
		SELECT	TOP 1	MAX(DateCreated) AS DateCreated, MAX(TransactionDate) AS TransactionDate
				FROM	employer_financial.TransactionLine 
				WHERE	AccountId = @AccountId 
						AND Ukprn = @Ukprn
						AND DateCreated BETWEEN @fromDate AND @toDate
			) AS TransactionDetails