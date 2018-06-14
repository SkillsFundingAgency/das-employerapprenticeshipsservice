﻿CREATE PROCEDURE [employer_financial].[GetPaymentDetail_ByAccountProviderCourseAndDateRange]
	@AccountId BIGINT,
	@Ukprn BIGINT,
	@courseName NVARCHAR(MAX),
	@pathwayCode INT,
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
	,MAX(dervx.Ukprn) as Ukprn
	,MAX(p.PeriodEnd) as PeriodEnd	
	,MAX(meta.ProviderName) as ProviderName
	,(SUM(pays1.[Amount]) * -1) as LineAmount
	,meta.ApprenticeshipCourseName as CourseName
	,meta.ApprenticeshipCourseLevel	as CourseLevel 
	,meta.PathwayName as PathwayName
	,MAX(meta.ApprenticeshipCourseStartDate) as CourseStartDate
	,MAX(meta.ApprenticeName) as ApprenticeName
	,MAX(meta.ApprenticeNINumber) as ApprenticeNINumber	
	,(SUM(pays2.Amount) * -1) as SfaCoInvestmentAmount
	,(SUM(pays3.Amount) * -1) as EmployerCoInvestmentAmount	
  FROM [employer_financial].[Payment] p
  inner JOIN [employer_financial].[PaymentMetaData] meta ON p.PaymentMetaDataId = meta.Id
  inner join (select PeriodEnd,AccountId,Ukprn, TransactionDate, DateCreated from employer_financial.TransactionLine where DateCreated >= @fromDate AND 
        DateCreated <= @toDate) dervx on dervx.AccountId = p.AccountId and dervx.PeriodEnd = p.PeriodEnd and dervx.Ukprn = p.Ukprn
  left join [employer_financial].[Payment] pays1 on pays1.AccountId = p.AccountId and pays1.Ukprn = p.Ukprn and pays1.FundingSource IN (1, 5) and pays1.PaymentMetaDataId = meta.Id
  left join [employer_financial].[Payment] pays2 on pays2.AccountId = p.AccountId and pays2.Ukprn = p.Ukprn and pays2.FundingSource = 2 and pays2.PaymentMetaDataId = meta.Id
  left join [employer_financial].[Payment] pays3 on pays3.AccountId = p.AccountId and pays3.Ukprn = p.Ukprn and pays3.FundingSource = 3 and pays3.PaymentMetaDataId = meta.Id
  where 
  p.AccountId = @AccountId AND
  p.Ukprn = @Ukprn AND
  ((@courseName IS NULL AND meta.ApprenticeshipCourseName IS NULL) OR (meta.ApprenticeshipCourseName = @courseName)) AND
  ((@courseLevel IS NULL AND meta.ApprenticeshipCourseLevel IS NULL) OR (meta.ApprenticeshipCourseLevel = @courseLevel)) AND
  ISNULL(meta.PathwayCode, -1) = ISNULL(@pathwayCode, -1) AND
  p.FundingSource IN (1,2,3,5)  
  group by p.AccountId, p.Ukprn, meta.ApprenticeshipCourseName, meta.ApprenticeshipCourseLevel, meta.PathwayName, meta.ApprenticeName