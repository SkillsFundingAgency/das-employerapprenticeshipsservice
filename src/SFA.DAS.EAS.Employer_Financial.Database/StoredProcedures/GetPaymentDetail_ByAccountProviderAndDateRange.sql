CREATE PROCEDURE [employer_financial].[GetPaymentDetail_ByAccountProviderAndDateRange]
	@accountId BIGINT,
	@ukprn BIGINT,
	@fromDate DATETIME,
	@toDate DATETIME
AS
	
select 
    tl.AccountId,
    null as Amount,
    null as EnglishFraction,
    null as TopUp,
    null as empref,
	meta.ApprenticeshipCourseName as CourseName,
	meta.ApprenticeshipCourseLevel as CourseLevel,
	meta.ApprenticeshipCourseStartDate as CourseStartDate,
	meta.ProviderName as ProviderName,
	meta.ApprenticeName, 
	meta.ApprenticeNINumber,	
    (tl.TransactionDate) as transactiondate,
    (p.Amount) as LineAmount,
    tl.TransactionType,
	p.Ukprn as UkPrn,
	p.PeriodEnd as PeriodEnd,
	DateCreated,
	null as PayrollYear,
	null as PayrollMonth,
	tl.SfaCoInvestmentAmount,
	tl.EmployerCoInvestmentAmount
from [employer_financial].TransactionLine tl
inner join [employer_financial].Payment p on p.PeriodEnd = tl.PeriodEnd and p.AccountId = tl.AccountId and p.Ukprn = tl.UkPrn
inner join [employer_financial].PaymentMetaData meta on p.PaymentMetaDataId = meta.Id
where   tl.DateCreated >= @fromDate AND 
        tl.DateCreated <= @toDate AND 
        tl.AccountId = @accountId AND
		p.Ukprn = @ukprn
