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
WHERE tl.AccountId = @AccountId 
AND tl.TransactionType IN (1, 2) 
AND DateCreated >= @FromDate 
AND DateCreated < @ToDate


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
SELECT MAX(transLine.DateCreated) AS DateCreated,
	ptt.[Description],
	meta.ProviderName AS TrainingProvider,
	p.Uln AS Uln,
	meta.ApprenticeName AS Apprentice,
	meta.ApprenticeshipCourseName AS ApprenticeTrainingCourse,
	meta.ApprenticeshipCourseLevel AS ApprenticeTrainingCourseLevel,
	COALESCE((SUM(pays1.[Amount]) * -1), 0) AS PaidFromLevy,
	COALESCE((SUM(pays3.Amount) * -1), 0) AS EmployerContribution,
	COALESCE((SUM(pays2.Amount) * -1), 0) AS GovermentContribution,
	COALESCE((SUM(pays1.[Amount]) * -1),0) + COALESCE((SUM(pays2.[Amount]) * -1),0) + COALESCE((SUM(pays3.[Amount]) * -1),0) AS Total

FROM [employer_financial].[Payment] p
  INNER JOIN [employer_financial].[PaymentTransactionTypes] ptt ON ptt.TransactionType =  p.TransactionType
  INNER JOIN [employer_financial].[PaymentMetaData] meta ON p.PaymentMetaDataId = meta.Id
  INNER JOIN (SELECT PeriodEnd,AccountId,Ukprn, EmpRef, TransactionDate, DateCreated, LevyDeclared, EnglishFraction FROM employer_financial.TransactionLine WHERE DateCreated >= @FromDate AND 
        DateCreated <= @ToDate) transLine ON transLine.AccountId = p.AccountId AND transLine.PeriodEnd = p.PeriodEnd AND transLine.Ukprn = p.Ukprn
  LEFT JOIN [employer_financial].[Payment] pays1 
	ON pays1.AccountId = p.AccountId 
		AND pays1.Ukprn = p.Ukprn 
		AND pays1.FundingSource = 1 
		AND pays1.PaymentMetaDataId = meta.Id
  LEFT JOIN [employer_financial].[Payment] pays2 
	ON pays2.AccountId = p.AccountId 
	AND pays2.Ukprn = p.Ukprn 
	AND pays2.FundingSource = 2 
	AND pays2.PaymentMetaDataId = meta.Id
  LEFT JOIN [employer_financial].[Payment] pays3 
	ON pays3.AccountId = p.AccountId 
	AND pays3.Ukprn = p.Ukprn 
	AND pays3.FundingSource = 3 
	AND pays3.PaymentMetaDataId = meta.Id
  WHERE 
	  p.AccountId = @AccountId AND
	  p.FundingSource IN (1,2,3)  
  GROUP BY p.AccountId, 
	  p.Ukprn, 
	  p.TransactionType, 
	  ptt.[Description],
	  p.Uln, 
	  meta.ProviderName, 
	  meta.ApprenticeshipCourseName, 
	  meta.ApprenticeshipCourseLevel, 
	  meta.PathwayName, 
	  meta.ApprenticeName

SELECT * FROM #output

DROP TABLE #output