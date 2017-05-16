CREATE PROCEDURE [employer_financial].[GetPaymentDetail_ByAccountProviderAndDateRange]
	@accountId BIGINT,
	@ukprn BIGINT,
	@fromDate DATETIME,
	@toDate DATETIME
AS	
SELECT
	p.[AccountId]
	,3 as TransactionType
	,Max(dervx.DateCreated) as DateCreated
	,Max(dervx.TransactionDate) as TransactionDate
	,Max(p.PaymentId) as PaymentId
	,MAX(dervx.UkPrn) as UkPrn
	,MAX(p.PeriodEnd) as PeriodEnd	
	,MAX(meta.ProviderName) as ProviderName
	,SUM(pays1.[Amount]) as LineAmount
	,meta.ApprenticeshipCourseName as CourseName
	,meta.ApprenticeshipCourseLevel	as CourseLevel  
	,MAX(meta.ApprenticeName) as ApprenticeName
	,MAX(meta.ApprenticeNINumber) as ApprenticeNINumber	
	,Sum(pays3.Amount) as SfaCoInvestmentAmount
	,Sum(pays2.Amount) as EmployerCoInvestmentAmount	
  FROM [SFA.DAS.EAS.Employer_Financial].[employer_financial].[Payment] p
  inner JOIN [SFA.DAS.EAS.Employer_Financial].[employer_financial].[PaymentMetaData] meta ON p.PaymentMetaDataId = meta.Id
  inner join (select PeriodEnd,AccountId,ukprn,TransactionDate, DateCreated from employer_financial.TransactionLine where DateCreated >= @fromDate AND 
        DateCreated <= @toDate) dervx on dervx.AccountId = p.AccountId and dervx.PeriodEnd = p.PeriodEnd and dervx.Ukprn = p.Ukprn
  left join [SFA.DAS.EAS.Employer_Financial].[employer_financial].[Payment] pays1 on pays1.AccountId = p.AccountId and pays1.Ukprn = p.Ukprn and pays1.FundingSource = 1 and pays1.PaymentMetaDataId = meta.id
  left join [SFA.DAS.EAS.Employer_Financial].[employer_financial].[Payment] pays2 on pays2.AccountId = p.AccountId and pays2.Ukprn = p.Ukprn and pays2.FundingSource = 2 and pays2.PaymentMetaDataId = meta.id
  left join [SFA.DAS.EAS.Employer_Financial].[employer_financial].[Payment] pays3 on pays3.AccountId = p.AccountId and pays3.Ukprn = p.Ukprn and pays3.FundingSource = 3 and pays3.PaymentMetaDataId = meta.id
  where 
  p.AccountId = @accountid AND
  p.Ukprn = @ukprn 
  group by p.AccountId, p.Ukprn, meta.ApprenticeshipCourseName, meta.ApprenticeshipCourseLevel, p.TransactionType