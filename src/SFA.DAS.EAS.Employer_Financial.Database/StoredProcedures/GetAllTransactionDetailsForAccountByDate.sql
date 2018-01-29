CREATE PROCEDURE [employer_financial].[GetAllTransactionDetailsForAccountByDate]
	@AccountId BIGINT,
	@fromDate DATETIME,
	@toDate DATETIME

AS
create table #output
(
DateCreated datetime,
TransactionType varchar(100),
EmpRef nvarchar(50),
PeriodEnd nvarchar(50),	
LevyDeclared decimal(18,4),
EnglishFraction decimal(18,5),
TenPercentTopUp decimal(18,4),
TrainingProvider varchar(max),
ULN bigint,
Apprentice varchar(max),
ApprenticeTrainingCourse varchar(max),
ApprenticeTrainingCourseLevel int,
PaidFromLevy decimal(18,4),
EmployerContribution decimal(18,4),
GovermentContribution decimal(18,4),
Total decimal(18,4)
)

INSERT INTO #output (
		DateCreated,
		TransactionType,
		EmpRef,
		PeriodEnd,
		LevyDeclared,
		EnglishFraction,
		TenPercentTopUp,
		Total
	) 


SELECT DateCreated,
	tlt.[Description],
	EmpRef,
	PeriodEnd,
	LevyDeclared,
	EnglishFraction,
	Amount - LevyDeclared as TenPercentTopUp,
	Amount
 from [employer_financial].[TransactionLine] tl
LEFT JOIN [employer_financial].[TransactionLineTypes] tlt on tlt.TransactionType = IIF(Amount >= 0, 1, 2)
where tl.AccountId = @accountId 
AND tl.TransactionType in(1,2) 
AND DateCreated >= @fromDate 
AND DateCreated <= @toDate


INSERT INTO #output
(
	DateCreated,
	TransactionType,
	TrainingProvider,
	ULN,
	Apprentice,
	ApprenticeTrainingCourse,
	ApprenticeTrainingCourseLevel,
	PaidFromLevy,
	EmployerContribution,
	GovermentContribution,
	Total
)
SELECT MAX(transLine.DateCreated) as DateCreated,
	ptt.[Description],
	meta.ProviderName as TrainingProvider,
	p.Uln as ULN,
	meta.ApprenticeName as Apprentice,
	meta.ApprenticeshipCourseName as ApprenticeTrainingCourse,
	meta.ApprenticeshipCourseLevel as ApprenticeTrainingCourseLevel,
	COALESCE((SUM(pays1.[Amount]) * -1), 0) as PaidFromLevy,
	COALESCE((SUM(pays3.Amount) * -1), 0) as EmployerContribution,
	COALESCE((SUM(pays2.Amount) * -1), 0) as GovermentContribution,
	COALESCE((SUM(pays1.[Amount]) * -1),0) + COALESCE((SUM(pays2.[Amount]) * -1),0) + COALESCE((SUM(pays3.[Amount]) * -1),0)   as Total

FROM [employer_financial].[Payment] p
  inner join [employer_financial].[PaymentTransactionTypes] ptt on ptt.TransactionType =  p.TransactionType
  inner join [employer_financial].[PaymentMetaData] meta ON p.PaymentMetaDataId = meta.Id
  inner join (select PeriodEnd,AccountId,ukprn, EmpRef, TransactionDate, DateCreated, LevyDeclared, EnglishFraction from employer_financial.TransactionLine where DateCreated >= @fromDate AND 
        DateCreated <= @toDate) transLine on transLine.AccountId = p.AccountId and transLine.PeriodEnd = p.PeriodEnd and transLine.Ukprn = p.Ukprn
  left join [employer_financial].[Payment] pays1 
	on pays1.AccountId = p.AccountId 
		and pays1.Ukprn = p.Ukprn 
		and pays1.FundingSource = 1 
		and pays1.PaymentMetaDataId = meta.id
  left join [employer_financial].[Payment] pays2 
	on pays2.AccountId = p.AccountId 
	and pays2.Ukprn = p.Ukprn 
	and pays2.FundingSource = 2 
	and pays2.PaymentMetaDataId = meta.id
  left join [employer_financial].[Payment] pays3 
	on pays3.AccountId = p.AccountId 
	and pays3.Ukprn = p.Ukprn 
	and pays3.FundingSource = 3 
	and pays3.PaymentMetaDataId = meta.id
  where 
	  p.AccountId = @accountid AND
	  p.FundingSource IN (1,2,3)  
  group by p.AccountId, 
	  p.Ukprn, 
	  p.TransactionType, 
	  ptt.[Description],
	  p.Uln, 
	  meta.ProviderName, 
	  meta.ApprenticeshipCourseName, 
	  meta.ApprenticeshipCourseLevel, 
	  meta.PathwayName, 
	  meta.ApprenticeName

SELECT * From #output

drop table #output

