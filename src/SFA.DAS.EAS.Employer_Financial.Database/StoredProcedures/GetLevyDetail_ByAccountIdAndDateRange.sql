CREATE PROCEDURE [employer_financial].[GetLevyDetail_ByAccountIdAndDateRange]
	@accountId bigint,
	@fromDate datetime,
	@toDate datetime

AS
select 
    tl.AccountId,
    tl.LevyDeclared as Amount,
	coalesce(efo.Amount, t.Amount, 1) AS EnglishFraction,
    ldt.Amount as TopUp,
    tl.EmpRef,
	null as CourseName,
	null as CourseLevel,
	null as CourseStartDate,
	null as ProviderName,
	null as ApprenticeName,
	null as ApprenticeNINumber,	
    tl.TransactionDate,
    tl.Amount as LineAmount,
    tl.TransactionType,
	null as UkPrn,
	null as PeriodEnd,
	DateCreated,
	ld.PayrollYear,
	ld.PayrollMonth,
	tl.SfaCoInvestmentAmount,
	tl.EmployerCoInvestmentAmount
from [employer_financial].TransactionLine tl
inner join [employer_financial].LevyDeclarationTopup ldt on ldt.SubmissionId = tl.SubmissionId
inner join [employer_financial].LevyDeclaration ld on ld.submissionid = tl.submissionid
OUTER APPLY
(
	SELECT TOP 1 Amount
	FROM [employer_financial].[EnglishFraction] ef
	WHERE ef.EmpRef = tl.empRef 
		AND ef.[DateCalculated] <= tl.TransactionDate
	ORDER BY [DateCalculated] DESC
) t
outer apply
(
	SELECT top 1 Amount
	FROM [employer_financial].[EnglishFractionOverride] o
	WHERE o.AccountId = tl.AccountId and o.EmpRef = tl.empref AND o.DateFrom <= tl.TransactionDate
	ORDER BY DateFrom DESC
) efo
where    tl.DateCreated >= @fromDate AND 
        tl.DateCreated <= @toDate AND 
        tl.AccountId = @accountId 
