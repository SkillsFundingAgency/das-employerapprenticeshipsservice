CREATE VIEW [employer_financial].[GetLevyDeclaration]
AS 
SELECT 
	ld.Id AS Id,	
	ld.AccountId as AccountId,
	ld.empRef AS EmpRef,
	ld.SubmissionDate AS SubmissionDate,
	ld.SubmissionId AS SubmissionId,
	ld.LevyDueYTD AS LevyDueYTD,
	coalesce(EnglishFractionOverride.Amount, EnglishFraction.Amount, 1) AS EnglishFraction, -- The EnglishFraction To Use
	TopUpPercentage.amount as TopUpPercentage, -- The TopUpPercentage To Use
	ld.PayrollYear as PayrollYear,
	ld.PayrollMonth as PayrollMonth,
	CASE 
		LatestSubmission.submissionid 
	when 
		ld.submissionid then 1 
	else 0 end as LastSubmission, -- Last submission if LevDeclaration Id matches the LatestSubmission
	ld.CreatedDate,
	ld.EndOfYearAdjustment,
	ld.EndOfYearAdjustmentAmount,
	ld.LevyAllowanceForYear AS LevyAllowanceForYear,
	ld.DateCeased AS DateCeased,
	ld.InactiveFrom AS InactiveFrom,
	ld.InactiveTo AS InactiveTo,
	ld.HmrcSubmissionId AS HmrcSubmissionId
FROM [employer_financial].[LevyDeclaration] ld
left join
(
	-- Find the latest submisison
	select max(submissionid) submissionid,empRef,PayrollMonth,PayrollYear from
	[employer_financial].LevyDeclaration xld
	where submissiondate in 
		(select max(submissiondate) from [employer_financial].LevyDeclaration 
		WHERE
		NoPaymentForPeriod = 0
		and EndOfYearAdjustment = 0 
		and submissiondate < [employer_financial].[CalculateSubmissionCutoffDate](PayrollMonth, PayrollYear)
		and PayrollYear = xld.PayrollYear
		and PayrollMonth = xld.PayrollMonth
		and empRef = xld.empRef
		group by empRef,PayrollYear,PayrollMonth)
	and submissiondate < [employer_financial].[CalculateSubmissionCutoffDate](PayrollMonth, PayrollYear)
		group by empRef,PayrollYear,PayrollMonth
)LatestSubmission on LatestSubmission.empRef = ld.empRef and LatestSubmission.PayrollMonth = ld.PayrollMonth and LatestSubmission.PayrollYear = ld.PayrollYear AND ld.EndOfYearAdjustment = 0

OUTER APPLY
(
	-- The English Fraction in use at the time of the LevyDeclaration
	SELECT TOP 1 Amount
	FROM [employer_financial].[EnglishFraction] ef
	WHERE ef.EmpRef = ld.empRef 
		AND ef.[DateCalculated] < [employer_financial].[CalculateSubmissionCutoffDate](ld.PayrollMonth, ld.PayrollYear)
	ORDER BY [DateCalculated] DESC
) EnglishFraction
outer apply
(
	-- The Top Up Percentage in use at the time of the LevyDeclaration
	SELECT top 1 Amount
	from [employer_financial].[TopUpPercentage] tp
	WHERE tp.[DateFrom] < [employer_financial].[CalculateSubmissionCutoffDate](ld.PayrollMonth, ld.PayrollYear)
) TopUpPercentage
outer apply
(
	-- Any English Fraction Override in use at the time of the LevyDeclaration
	SELECT top 1 Amount
	FROM [employer_financial].[EnglishFractionOverride] o
	WHERE o.AccountId = ld.AccountId and o.EmpRef = ld.empref AND o.DateFrom < [employer_financial].[CalculateSubmissionCutoffDate](ld.PayrollMonth, ld.PayrollYear)
	ORDER BY DateFrom DESC
) EnglishFractionOverride
