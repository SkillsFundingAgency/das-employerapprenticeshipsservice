CREATE PROCEDURE [employer_financial].[GetPaymentDetail_ByAccountProviderCourseAndDateRange]
	@accountId BIGINT,
	@ukprn BIGINT,
	@courseName NVARCHAR(MAX),
	@courseLevel INT,
	@fromDate DATETIME,
	@toDate DATETIME
AS	
SELECT
    p.[AccountId]
     ,3 as TransactionType
	,MAX(dervx.DateCreated) as DateCreated
	,MAX(dervx.TransactionDate) as TransactionDate
	,MAX(p.PaymentId) as PaymentId
	,MAX(dervx.UkPrn) as UkPrn
	,MAX(p.PeriodEnd) as PeriodEnd	
	,MAX(meta.ProviderName) as ProviderName
	,(SUM(pays1.[Amount]) * -1) as LineAmount
	,meta.ApprenticeshipCourseName as CourseName
	,meta.ApprenticeshipCourseLevel	as CourseLevel  
	,MAX(meta.ApprenticeshipCourseStartDate) as CourseStartDate
	,MAX(meta.ApprenticeName) as ApprenticeName
	,MAX(meta.ApprenticeNINumber) as ApprenticeNINumber	
	,(SUM(pays2.Amount) * -1) as SfaCoInvestmentAmount
	,(SUM(pays3.Amount) * -1) as EmployerCoInvestmentAmount	
  FROM [employer_financial].[Payment] p
  inner JOIN [employer_financial].[PaymentMetaData] meta ON p.PaymentMetaDataId = meta.Id
  inner join (select PeriodEnd,AccountId,ukprn, TransactionDate, DateCreated from employer_financial.TransactionLine where DateCreated >= @fromDate AND 
        DateCreated <= @toDate) dervx on dervx.AccountId = p.AccountId and dervx.PeriodEnd = p.PeriodEnd and dervx.Ukprn = p.Ukprn
  left join [employer_financial].[Payment] pays1 on pays1.AccountId = p.AccountId and pays1.Ukprn = p.Ukprn and pays1.FundingSource = 1 and pays1.PaymentMetaDataId = meta.id
  left join [employer_financial].[Payment] pays2 on pays2.AccountId = p.AccountId and pays2.Ukprn = p.Ukprn and pays2.FundingSource = 2 and pays2.PaymentMetaDataId = meta.id
  left join [employer_financial].[Payment] pays3 on pays3.AccountId = p.AccountId and pays3.Ukprn = p.Ukprn and pays3.FundingSource = 3 and pays3.PaymentMetaDataId = meta.id
  where 
  p.AccountId = @accountid AND
  p.Ukprn = @ukprn AND
  meta.ApprenticeshipCourseName = @courseName AND
  meta.ApprenticeshipCourseLevel = @courseLevel
  group by p.AccountId, p.Ukprn, meta.ApprenticeshipCourseName, meta.ApprenticeshipCourseLevel, meta.ApprenticeName