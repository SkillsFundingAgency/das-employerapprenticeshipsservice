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
	LevyDeclared,
	EnglishFraction,
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
			PaymentId, 
			FundingSource, 
			Amount, 
			p.PaymentMetaDataId, 
			ptt.[Description] AS TransactionTypeDesc,
			meta.ProviderName AS TrainingProvider,
			meta.ApprenticeName AS Apprentice,
			meta.ApprenticeshipCourseName AS ApprenticeTrainingCourse,
			meta.ApprenticeshipCourseLevel AS ApprenticeTrainingCourseLevel
	from [employer_financial].[Payment]   p
		INNER JOIN (SELECT PeriodEnd,AccountId,ukprn, EmpRef, TransactionDate, DateCreated, LevyDeclared, EnglishFraction 
					FROM employer_financial.TransactionLine 
					WHERE DateCreated >= @FromDate AND 
						DateCreated <= @ToDate) transLine 
			ON transLine.AccountId = p.AccountId 
				AND transLine.PeriodEnd = p.PeriodEnd 
				AND transLine.Ukprn = p.Ukprn
		INNER JOIN [employer_financial].[PaymentTransactionTypes] ptt 
			ON ptt.TransactionType =  p.TransactionType
		INNER JOIN [employer_financial].[PaymentMetaData] meta 
			ON p.PaymentMetaDataId = meta.Id
	WHERE  p.AccountId = @AccountId
		AND DateCreated >= @FromDate 
		AND DateCreated <= @ToDate
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
	COALESCE(MAX(accountFunds1.DateCreated),MAX(accountFunds2.DateCreated),MAX(accountFunds3.DateCreated)) AS DateCreated,
	COALESCE(MAX(accountFunds1.TransactionTypeDesc),MAX(accountFunds2.TransactionTypeDesc),MAX(accountFunds3.TransactionTypeDesc)) AS [Description],
	COALESCE(MAX(accountFunds1.TrainingProvider),MAX(accountFunds2.TrainingProvider),MAX(accountFunds3.TrainingProvider)) AS TrainingProvider,
	uniquePayments.Uln AS Uln, 
	COALESCE(MAX(accountFunds1.Apprentice),MAX(accountFunds2.Apprentice),MAX(accountFunds3.Apprentice)) AS Apprentice,
	COALESCE(MAX(accountFunds1.ApprenticeTrainingCourse), MAX(accountFunds2.ApprenticeTrainingCourse), MAX(accountFunds3.ApprenticeTrainingCourse)) AS ApprenticeTrainingCourse,
	COALESCE(MAX(accountFunds1.ApprenticeTrainingCourseLevel), MAX(accountFunds2.ApprenticeTrainingCourseLevel), MAX(accountFunds3.ApprenticeTrainingCourseLevel)) AS ApprenticeTrainingCourseLevel,
	COALESCE((SUM(accountFunds1.[Amount]) * -1), 0) AS PaidFromLevy,
	COALESCE((SUM(accountFunds3.Amount) * -1), 0) AS EmployerContribution,
	COALESCE((SUM(accountFunds2.Amount) * -1), 0) AS GovermentContribution,
	COALESCE((SUM(accountFunds1.[Amount]) * -1),0) + COALESCE((SUM(accountFunds2.[Amount]) * -1),0) + COALESCE((SUM(accountFunds3.[Amount]) * -1),0) AS Total
FROM UniqueApprenticePaymentRecords uniquePayments
LEFT JOIN AllFunds accountFunds1 
	ON accountFunds1.AccountId = uniquePayments.AccountId 
		AND accountFunds1.Ukprn = uniquePayments.Ukprn 
		AND accountFunds1.Uln = uniquePayments.Uln
		AND accountFunds1.PeriodEnd = uniquePayments.PeriodEnd
		AND accountFunds1.FundingSource = 1
LEFT JOIN AllFunds accountFunds2 
	ON accountFunds2.AccountId = uniquePayments.AccountId 
		AND accountFunds2.Ukprn = uniquePayments.Ukprn 
		AND accountFunds2.Uln = uniquePayments.Uln
		AND accountFunds2.PeriodEnd = uniquePayments.PeriodEnd
		AND accountFunds2.FundingSource = 2
LEFT JOIN AllFunds accountFunds3
	ON accountFunds3.AccountId = uniquePayments.AccountId 
		AND accountFunds3.Ukprn = uniquePayments.Ukprn 
		AND accountFunds3.Uln = uniquePayments.Uln
		AND accountFunds3.PeriodEnd = uniquePayments.PeriodEnd
		AND accountFunds3.FundingSource = 3
GROUP BY uniquePayments.AccountId, 
	uniquePayments.Ukprn, 
	uniquePayments.Uln, 
	uniquePayments.PeriodEnd
HAVING
	MAX(accountFunds1.[Amount]) > 0 OR MAX(accountFunds2.[Amount]) > 0 OR MAX(accountFunds3.[Amount]) > 0

SELECT * FROM #output

DROP TABLE #output