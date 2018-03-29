CREATE PROCEDURE [employer_financial].[GetLevyDetail_ByAccountIdAndDateRange]
	@AccountId bigint,
	@fromDate datetime,
	@toDate datetime

AS
select 
    tl.AccountId,
    tl.LevyDeclared as Amount,
	tl.EnglishFraction,
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
	null as Ukprn,
	null as PeriodEnd,
	DateCreated,
	ld.PayrollYear,
	ld.PayrollMonth,
	tl.SfaCoInvestmentAmount,
	tl.EmployerCoInvestmentAmount
from [employer_financial].TransactionLine tl
inner join [employer_financial].LevyDeclarationTopup ldt on ldt.SubmissionId = tl.SubmissionId
inner join [employer_financial].LevyDeclaration ld on ld.SubmissionId = tl.SubmissionId
where    tl.DateCreated >= @fromDate AND 
        tl.DateCreated <= @toDate AND 
        tl.AccountId = @AccountId 
