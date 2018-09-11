CREATE VIEW [employer_financial].[GetLevyDeclaration]
AS 
SELECT 
	ld.Id AS Id,	
	ld.AccountId as AccountId,
	ld.EmpRef AS EmpRef,
	ld.SubmissionDate AS SubmissionDate,
	ld.SubmissionId AS SubmissionId,
	ld.LevyDueYTD AS LevyDueYTD,
	coalesce(EnglishFractionOverride.Amount, EnglishFraction.Amount, 1) AS EnglishFraction, -- The EnglishFraction To Use
	TopUpPercentage.Amount as TopUpPercentage, -- The TopUpPercentage To Use
	ld.PayrollYear as PayrollYear,
	ld.PayrollMonth as PayrollMonth,
	CASE 
		LatestSubmission.SubmissionId 
	when 
		ld.SubmissionId then 1 
	else 0 end as LastSubmission, -- Last submission if LevDeclaration Id matches the LatestSubmission
	ld.CreatedDate,
	ld.EndOfYearAdjustment,
	ld.EndOfYearAdjustmentAmount,
	ld.LevyAllowanceForYear AS LevyAllowanceForYear,
	ld.DateCeased AS DateCeased,
	ld.InactiveFrom AS InactiveFrom,
	ld.InactiveTo AS InactiveTo,
	ld.HmrcSubmissionId AS HmrcSubmissionId,
	ld.NoPaymentForPeriod
FROM [employer_financial].[LevyDeclaration] ld
left join
(
	-- Find the latest submission
	select max(SubmissionId) SubmissionId,EmpRef,PayrollMonth,PayrollYear from
	[employer_financial].LevyDeclaration xld
	where SubmissionDate in 
		(select max(SubmissionDate) from [employer_financial].LevyDeclaration 
		WHERE EndOfYearAdjustment = 0 
		and SubmissionDate < [employer_financial].[CalculateSubmissionCutoffDate](PayrollMonth, PayrollYear)
		and PayrollYear = xld.PayrollYear
		and PayrollMonth = xld.PayrollMonth
		and EmpRef = xld.EmpRef
		group by EmpRef,PayrollYear,PayrollMonth)
	and SubmissionDate < [employer_financial].[CalculateSubmissionCutoffDate](PayrollMonth, PayrollYear)
		group by EmpRef,PayrollYear,PayrollMonth
)LatestSubmission on LatestSubmission.EmpRef = ld.EmpRef and LatestSubmission.PayrollMonth = ld.PayrollMonth and LatestSubmission.PayrollYear = ld.PayrollYear AND ld.EndOfYearAdjustment = 0

OUTER APPLY
(
	-- The English Fraction in use at the time of the LevyDeclaration
	SELECT TOP 1 Amount
	FROM [employer_financial].[EnglishFraction] ef
	WHERE ef.EmpRef = ld.EmpRef 
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
	WHERE o.AccountId = ld.AccountId and o.EmpRef = ld.EmpRef AND o.DateFrom < [employer_financial].[CalculateSubmissionCutoffDate](ld.PayrollMonth, ld.PayrollYear)
	ORDER BY DateFrom DESC
) EnglishFractionOverride
