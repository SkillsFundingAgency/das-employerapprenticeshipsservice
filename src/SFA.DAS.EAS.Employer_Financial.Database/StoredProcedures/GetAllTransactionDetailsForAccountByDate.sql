CREATE PROCEDURE [employer_financial].[GetAllTransactionDetailsForAccountByDate]
	@AccountId BIGINT,
	@FromDate DATETIME,
	@ToDate DATETIME

AS
CREATE TABLE #output
(
DateCreated DATETIME,
TransactionType VARCHAR(100),
PayeScheme NVARCHAR(50),
PayrollYear NVARCHAR(50),
PayrollMonth INT,
LevyDeclared DECIMAL(18,4),
EnglishFraction DECIMAL(18,5),
TenPercentTopUp DECIMAL(18,4),
TrainingProvider VARCHAR(MAX),
Uln BIGINT,
Apprentice VARCHAR(MAX),
ApprenticeTrainingCourse VARCHAR(MAX),
ApprenticeTrainingCourseLevel INT,
PaidFromLevy DECIMAL(18,4),
EmployerContribution DECIMAL(18,4),
GovermentContribution DECIMAL(18,4),
Total DECIMAL(18,4)
)

INSERT INTO #output (
		DateCreated,
		TransactionType,
		PayeScheme,
		PayrollYear,
		PayrollMonth,
		LevyDeclared,
		EnglishFraction,
		TenPercentTopUp,
		Total
	) 

SELECT DateCreated,
	tlt.[Description],
	tl.EmpRef,
	ld.PayrollYear,
	ld.PayrollMonth,
	tl.LevyDeclared,
	tl.EnglishFraction,
	tl.Amount - (LevyDeclared * EnglishFraction) AS TenPercentTopUp,
	tl.Amount
FROM [employer_financial].TransactionLine tl
LEFT JOIN [employer_financial].[TransactionLineTypes] tlt ON tlt.TransactionType = IIF(Amount >= 0, 1, 2)
INNER JOIN [employer_financial].LevyDeclarationTopup ldt ON ldt.SubmissionId = tl.SubmissionId
INNER JOIN [employer_financial].LevyDeclaration ld ON ld.SubmissionId = tl.SubmissionId
WHERE tl.AccountId = @accountId 
AND tl.TransactionType IN (1, 2) 
AND DateCreated >= @FromDate 
AND DateCreated < @ToDate



-- We will use this Common Table Expression to get payments that relate to transaction lines and are funding source 1,2,3
;WITH AllFunds (DateCreated, AccountId, Ukprn, Uln, PeriodEnd, PaymentId, FundingSource, Amount, PaymentMetaDataId, TransactionTypeDesc,
	TrainingProvider,
	Apprentice,
	ApprenticeTrainingCourse,
	ApprenticeTrainingCourseLevel)
AS
(
	SELECT 
			transLine.DateCreated,
			p.AccountId, 
			p.Ukprn, 
			p.Uln, 
			p.PeriodEnd, 
			p.PaymentId, 
			p.FundingSource, 
			p.Amount, 
			p.PaymentMetaDataId, 
			ptt.[Description] AS TransactionTypeDesc,
			meta.ProviderName AS TrainingProvider,
			meta.ApprenticeName AS Apprentice,
			meta.ApprenticeshipCourseName AS ApprenticeTrainingCourse,
			meta.ApprenticeshipCourseLevel AS ApprenticeTrainingCourseLevel
	from [employer_financial].[Payment]   p
		INNER JOIN  employer_financial.TransactionLine transLine
			ON transLine.AccountId = p.AccountId 
				AND transLine.PeriodEnd = p.PeriodEnd 
				AND transLine.Ukprn = p.Ukprn
		INNER JOIN [employer_financial].[PaymentTransactionTypes] ptt 
			ON ptt.TransactionType =  p.TransactionType
		INNER JOIN [employer_financial].[PaymentMetaData] meta 
			ON p.PaymentMetaDataId = meta.Id
	WHERE  p.AccountId = @AccountId
		AND transLine.DateCreated >= @FromDate 
		AND transLine.DateCreated <= @ToDate
		AND p.FundingSource in (1,2,3)
)
, UniqueApprenticePaymentRecords -- This gets all the unique Account, Provider, Learner, PeriodEnd combinations
AS
(
SELECT AccountId, Ukprn, Uln, PeriodEnd 
	FROM [employer_financial].[Payment]   p
		WHERE  p.AccountId = @AccountId
			AND p.FundingSource in (1,2,3)
	GROUP BY AccountId, Ukprn, Uln, PeriodEnd
)


INSERT INTO #output
(
	DateCreated,
	TransactionType,
	TrainingProvider,
	Uln,
	Apprentice,
	ApprenticeTrainingCourse,
	ApprenticeTrainingCourseLevel,
	PaidFromLevy,
	EmployerContribution,
	GovermentContribution,
	Total
)

-- Join the Views together
SELECT DISTINCT 
	COALESCE(MAX(fundsPaidFromLevy.DateCreated),MAX(fundsGovermentContribution.DateCreated),MAX(fundsEmployerContribution.DateCreated)) AS DateCreated,
	COALESCE(MAX(fundsPaidFromLevy.TransactionTypeDesc),MAX(fundsGovermentContribution.TransactionTypeDesc),MAX(fundsEmployerContribution.TransactionTypeDesc)) AS [Description],
	COALESCE(MAX(fundsPaidFromLevy.TrainingProvider),MAX(fundsGovermentContribution.TrainingProvider),MAX(fundsEmployerContribution.TrainingProvider)) AS TrainingProvider,
	uniquePayments.Uln AS Uln, 
	COALESCE(MAX(fundsPaidFromLevy.Apprentice),MAX(fundsGovermentContribution.Apprentice),MAX(fundsEmployerContribution.Apprentice)) AS Apprentice,
	COALESCE(MAX(fundsPaidFromLevy.ApprenticeTrainingCourse), MAX(fundsGovermentContribution.ApprenticeTrainingCourse), MAX(fundsEmployerContribution.ApprenticeTrainingCourse)) AS ApprenticeTrainingCourse,
	COALESCE(MAX(fundsPaidFromLevy.ApprenticeTrainingCourseLevel), MAX(fundsGovermentContribution.ApprenticeTrainingCourseLevel), MAX(fundsEmployerContribution.ApprenticeTrainingCourseLevel)) AS ApprenticeTrainingCourseLevel,
	COALESCE((SUM(fundsPaidFromLevy.[Amount]) * -1), 0) AS PaidFromLevy,
	COALESCE((SUM(fundsEmployerContribution.Amount) * -1), 0) AS EmployerContribution,
	COALESCE((SUM(fundsGovermentContribution.Amount) * -1), 0) AS GovermentContribution,
	COALESCE((SUM(fundsPaidFromLevy.[Amount]) * -1),0) + COALESCE((SUM(fundsGovermentContribution.[Amount]) * -1),0) + COALESCE((SUM(fundsEmployerContribution.[Amount]) * -1),0) AS Total
FROM UniqueApprenticePaymentRecords uniquePayments
LEFT JOIN AllFunds fundsPaidFromLevy 
	ON fundsPaidFromLevy.AccountId = uniquePayments.AccountId 
		AND fundsPaidFromLevy.Ukprn = uniquePayments.Ukprn 
		AND fundsPaidFromLevy.Uln = uniquePayments.Uln
		AND fundsPaidFromLevy.PeriodEnd = uniquePayments.PeriodEnd
		AND fundsPaidFromLevy.FundingSource = 1
LEFT JOIN AllFunds fundsGovermentContribution 
	ON fundsGovermentContribution.AccountId = uniquePayments.AccountId 
		AND fundsGovermentContribution.Ukprn = uniquePayments.Ukprn 
		AND fundsGovermentContribution.Uln = uniquePayments.Uln
		AND fundsGovermentContribution.PeriodEnd = uniquePayments.PeriodEnd
		AND fundsGovermentContribution.FundingSource = 2
LEFT JOIN AllFunds fundsEmployerContribution
	ON fundsEmployerContribution.AccountId = uniquePayments.AccountId 
		AND fundsEmployerContribution.Ukprn = uniquePayments.Ukprn 
		AND fundsEmployerContribution.Uln = uniquePayments.Uln
		AND fundsEmployerContribution.PeriodEnd = uniquePayments.PeriodEnd
		AND fundsEmployerContribution.FundingSource = 3
GROUP BY uniquePayments.AccountId, 
	uniquePayments.Ukprn, 
	uniquePayments.Uln, 
	uniquePayments.PeriodEnd
HAVING
	MAX(fundsPaidFromLevy.[Amount]) > 0 OR MAX(fundsGovermentContribution.[Amount]) > 0 OR MAX(fundsEmployerContribution.[Amount]) > 0

SELECT * FROM #output

DROP TABLE #output