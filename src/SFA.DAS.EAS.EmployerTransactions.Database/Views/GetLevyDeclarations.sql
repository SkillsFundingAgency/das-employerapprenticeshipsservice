CREATE VIEW [employer_transactions].[GetLevyDeclarations]
AS



SELECT 
	ld.Id AS Id,	
	ld.AccountId as AccountId,
	ld.empRef AS EmpRef,
	ld.SubmissionDate AS SubmissionDate,
	ld.SubmissionId AS SubmissionId,
	ld.LevyDueYTD AS LevyDueYTD,
	t.Amount AS EnglishFraction,
	w.amount as TopUpPercentage,
	ld.PayrollYear as PayrollYear,
	ld.PayrollMonth as PayrollMonth,
	CASE 
		x.submissionDate 
	when 
		ld.SubmissionDate then 1 
	else 0 end as LastSubmission
FROM [employer_transactions].[LevyDeclaration] ld
inner join
(
	select 
		Max(submissionDate) submissionDate, 
		empRef ,
		PayrollYear,
		PayrollMonth
	FROM 
		[employer_transactions].LevyDeclaration 
	group by empRef,PayrollYear,PayrollMonth
)x on x.empRef = ld.empRef and x.PayrollMonth = ld.PayrollMonth and x.PayrollYear = ld.PayrollYear

OUTER APPLY
(
	SELECT TOP 1 Amount
	FROM [employer_transactions].[EnglishFraction] ef
	WHERE ef.EmpRef = ld.empRef 
		AND ef.[DateCalculated] <= ld.[SubmissionDate]
	ORDER BY [DateCalculated] DESC
) t
outer apply
(
	SELECT top 1 Amount
	from [employer_transactions].[TopUpPercentage] tp
	WHERE tp.[DateFrom] <= ld.[SubmissionDate]
) w

GO




