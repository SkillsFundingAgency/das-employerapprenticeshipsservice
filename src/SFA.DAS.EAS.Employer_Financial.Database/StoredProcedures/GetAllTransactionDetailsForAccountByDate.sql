CREATE PROCEDURE [employer_financial].[GetAllTransactionDetailsForAccountByDate]
	@AccountId BIGINT,
	@FromDate DATETIME,
	@ToDate DATETIME

AS

SELECT	tl.DateCreated									AS DateCreated,
			tl.[AccountId]									AS AccountId,
			tlt.[Description]								AS TransactionType,
			tl.EmpRef										AS PayeScheme,
			ld.PayrollYear									AS PayrollYear,
			ld.PayrollMonth									AS PayrollMonth,
			tl.LevyDeclared									AS LevyDeclared,
			tl.EnglishFraction								AS EnglishFraction,
			tl.Amount - (LevyDeclared * EnglishFraction)	AS TenPercentTopUp,
			CONVERT(VARCHAR(MAX), NULL)						AS TrainingProvider,
			CONVERT(BIGINT, NULL)							AS Uln,
			CONVERT(VARCHAR(MAX), NULL)						AS Apprentice,
			CONVERT(VARCHAR(MAX), NULL)						AS ApprenticeTrainingCourse,
			CONVERT(INT, NULL)								AS ApprenticeTrainingCourseLevel,
			CONVERT(DECIMAL(18,4), NULL)					AS PaidFromLevy,
			CONVERT(DECIMAL(18,4), NULL)					AS EmployerContribution,
			CONVERT(DECIMAL(18,4), NULL)					AS GovermentContribution,
			tl.Amount										AS Total,
			NULL											AS TransferSenderAccountId,
			NULL											AS TransferSenderAccountName,
			NULL											AS TransferReceiverAccountId,
			NULL											AS TransferReceiverAccountName
	FROM	[employer_financial].TransactionLine tl
			LEFT JOIN [employer_financial].[TransactionLineTypes] tlt 
				ON tlt.TransactionType = IIF(Amount >= 0, 1, 2)
			INNER JOIN [employer_financial].LevyDeclarationTopup ldt 
				ON ldt.SubmissionId = tl.SubmissionId
			INNER JOIN [employer_financial].LevyDeclaration ld 
				ON ld.SubmissionId = tl.SubmissionId
	WHERE	tl.AccountId = @accountId 
			AND tl.TransactionType IN (1, 2) 
			AND DateCreated >= @FromDate 
			AND DateCreated < @ToDate

UNION ALL

	SELECT DISTINCT 
		funds.DateCreated					AS DateCreated,
		funds.AccountId						AS AccountId,
		funds.TransactionTypeDesc			AS TransactionType,
		CONVERT(VARCHAR(50), NULL)			AS PayeScheme,
		CONVERT(VARCHAR(50), NULL)			AS PayrollYear,
		CONVERT(INT, NULL)					AS PayrollMonth,
		CONVERT(DECIMAL(18,4), NULL)		AS LevyDeclared,
		CONVERT(DECIMAL(18, 5), NULL)		AS EnglishFraction,
		CONVERT(DECIMAL(18,4), NULL)		AS TenPercentTopUp,
		funds.TrainingProvider				AS TrainingProvider,
		funds.Uln							AS Uln, 
		funds.Apprentice					AS Apprentice,
		funds.ApprenticeTrainingCourse		AS ApprenticeTrainingCourse,
		funds.ApprenticeTrainingCourseLevel AS ApprenticeTrainingCourseLevel,
		funds.FundedFromLevy				AS PaidFromLevy,
		funds.FundedFromEmployer			AS EmployerContribution,
		funds.FundedFromGoverment			AS GovermentContribution,
		funds.fundedTotal					AS Total,
		NULL								AS TransferSenderAccountId,
		NULL								AS TransferSenderAccountName,
		NULL								AS TransferReceiverAccountId,
		NULL								AS TransferReceiverAccountName
	FROM (SELECT 	transLine.DateCreated,
				p.AccountId, 
				p.Ukprn, 
				p.Uln, 
				p.PeriodEnd, 
				SUM(CASE WHEN p.FundingSource = 1 THEN -p.Amount ELSE 0 END) AS fundedFromLevy, 
				SUM(CASE WHEN p.FundingSource = 2 THEN -p.Amount ELSE 0 END) AS fundedFromGoverment, 
				SUM(CASE WHEN p.FundingSource = 3 THEN -p.Amount ELSE 0 END) AS fundedFromEmployer, 
				SUM(-p.Amount) AS fundedTotal, 
				ptt.[Description] AS TransactionTypeDesc,
				meta.ProviderName AS TrainingProvider,
				meta.ApprenticeName AS Apprentice,
				meta.ApprenticeshipCourseName AS ApprenticeTrainingCourse,
				meta.ApprenticeshipCourseLevel AS ApprenticeTrainingCourseLevel
		FROM [employer_financial].[Payment]   p
				JOIN  employer_financial.TransactionLine transLine
					ON transLine.AccountId = p.AccountId 
						AND transLine.PeriodEnd = p.PeriodEnd 
						AND transLine.Ukprn = p.Ukprn
				JOIN [employer_financial].[PaymentTransactionTypes] ptt 
					ON ptt.TransactionType =  p.TransactionType
				JOIN [employer_financial].[PaymentMetaData] meta 
					ON p.PaymentMetaDataId = meta.Id
		WHERE  p.AccountId = @AccountId
				AND transLine.DateCreated >= @FromDate 
				AND transLine.DateCreated <= @ToDate
				AND p.FundingSource in (1,2,3)
		GROUP BY transLine.DateCreated,
				p.AccountId, 
				p.Ukprn, 
				p.Uln, 
				p.PeriodEnd, 
				ptt.[Description],
				meta.ProviderName,
				meta.ApprenticeName,
				meta.ApprenticeshipCourseName,
				meta.ApprenticeshipCourseLevel) AS funds

UNION ALL

SELECT		DATEADD(dd, DATEDIFF(dd, 0, tl.DateCreated), 0)		AS DateCreated,
			tl.[AccountId]										AS AccountId,
			tlt.[Description]									AS TransactionType,
			tl.EmpRef											AS PayeScheme,
			NULL												AS PayrollYear,
			NULL												AS PayrollMonth,
			NULL												AS LevyDeclared,
			NULL												AS EnglishFraction,
			NULL												AS TenPercentTopUp,
			NULL												AS TrainingProvider,
			NULL												AS Uln,
			NULL												AS Apprentice,
			NULL												AS ApprenticeTrainingCourse,
			NULL												AS ApprenticeTrainingCourseLevel,
			NULL												AS PaidFromLevy,
			NULL												AS EmployerContribution,
			NULL												AS GovermentContribution,
			tl.Amount											AS Total,
			NULL												AS TransferSenderAccountId,
			NULL												AS TransferSenderAccountName,
			NULL												AS TransferReceiverAccountId,
			NULL												AS TransferReceiverAccountName
	FROM	[employer_financial].TransactionLine tl
			LEFT JOIN [employer_financial].[TransactionLineTypes] tlt
				ON tlt.TransactionType = 3
	WHERE	tl.AccountId = @accountId 
			AND DateCreated >= @FromDate 
			AND DateCreated < @ToDate
			AND tl.TransactionType = 5

UNION ALL
	
	-- sender transfers
 SELECT     
  DATEADD(dd, DATEDIFF(dd, 0, tl.DateCreated), 0)  AS DateCreated,  
  tl.AccountId          AS AccountId,  
  tlt.[Description]         AS TransactionType,  
  NULL            AS PayeScheme,  
  NULL            AS PayrollYear,  
  NULL            AS PayrollMonth,  
  NULL            AS LevyDeclared,  
  NULL            AS EnglishFraction,  
  NULL            AS TenPercentTopUp,  
  CASE tlt.[Description]  
   WHEN 'Payment' THEN meta.ProviderName   
   ELSE NULL  
  END             AS TrainingProvider,  
  NULL            AS Uln,  
  NULL            AS Apprentice,  
  trans.CourseName          AS ApprenticeTrainingCourse,  
  meta.CourseLevel         AS ApprenticeTrainingCourseLevel,  
  SUM(tl.Amount)          AS PaidFromLevy,  
  SUM(tl.SfaCoInvestmentAmount)      AS EmployerContribution,  
  SUM(tl.EmployerCoInvestmentAmount)     AS GovermentContribution,  
  SUM(tl.Amount)          AS Total,  
  tl.TransferSenderAccountId       AS TransferSenderAccountId,  
  tl.TransferSenderAccountName      AS TransferSenderAccountName,  
  tl.TransferReceiverAccountId      AS TransferReceiverAccountId,  
  tl.TransferReceiverAccountName      AS TransferReceiverAccountName     
  FROM [employer_financial].[TransactionLine] tl  
  
  JOIN [employer_financial].[TransactionLineTypes] tlt  
    ON tl.TransactionType = tlt.TransactionType  
  
  LEFT JOIN   
 (SELECT tl2.AccountId, tr2.CourseName, tl2.PeriodEnd
  FROM [employer_financial].[TransactionLine] tl2  
  
  LEFT JOIN [employer_financial].[AccountTransfers] tr2  
   ON (tr2.SenderAccountId = tl2.AccountId and tr2.PeriodEnd = tl2.PeriodEnd)      
  
  WHERE tl2.AccountId = @AccountId
  AND tl2.TransactionType = 4 -- Transfer  
  ) as trans  
   on trans.AccountId = tl.AccountId  
   and trans.PeriodEnd = tl.PeriodEnd     
  
   LEFT JOIN   
   (SELECT DISTINCT p3.AccountId, p3.PeriodEnd, m3.ProviderName, m3.ApprenticeshipCourseLevel as 'CourseLevel'  
  FROM [employer_financial].[Payment] p3  
  inner join [employer_financial].[TransactionLine] tl3
	on tl3.AccountId = @AccountId
	AND tl3.AccountId = tl3.TransferSenderAccountId
	AND tl3.TransactionType IN (3,  4)
  INNER JOIN [employer_financial].[PaymentMetaData] m3   
   ON m3.Id = p3.PaymentMetaDataId  
   AND p3.AccountId = tl3.TransferReceiverAccountId     
  WHERE m3.ProviderName IS NOT NULL  
   ) as meta  
   on meta.AccountId = tl.TransferReceiverAccountId  
   and meta.PeriodEnd = tl.PeriodEnd  
  
  WHERE tl.AccountId = @AccountId   
  AND tl.AccountId = tl.TransferSenderAccountId
  AND tl.TransactionType IN (3,  4) -- Payment and Transfer  
  GROUP BY   
  tl.DateCreated,   
  tl.AccountId,   
  tlt.[Description],  
  tl.PeriodEnd,   
  meta.ProviderName,  
  trans.CourseName,  
  meta.CourseLevel,  
  tl.TransferSenderAccountId,   
  tl.TransferSenderAccountName,   
  tl.TransferReceiverAccountId,   
  tl.TransferReceiverAccountName
  
  UNION

-- receiver transfers  
 SELECT     
  DATEADD(dd, DATEDIFF(dd, 0, tl.DateCreated), 0)  AS DateCreated,  
  tl.AccountId          AS AccountId,  
  tlt.[Description]         AS TransactionType,  
  NULL            AS PayeScheme,  
  NULL            AS PayrollYear,  
  NULL            AS PayrollMonth,  
  NULL            AS LevyDeclared,  
  NULL            AS EnglishFraction,  
  NULL            AS TenPercentTopUp,  
  CASE tlt.[Description]  
   WHEN 'Payment' THEN meta.ProviderName   
   ELSE NULL  
  END             AS TrainingProvider,  
  NULL            AS Uln,  
  NULL            AS Apprentice,  
  trans.CourseName          AS ApprenticeTrainingCourse,  
  meta.CourseLevel         AS ApprenticeTrainingCourseLevel,  
  SUM(tl.Amount)          AS PaidFromLevy,  
  SUM(tl.SfaCoInvestmentAmount)      AS EmployerContribution,  
  SUM(tl.EmployerCoInvestmentAmount)     AS GovermentContribution,  
  SUM(tl.Amount)          AS Total,  
  tl.TransferSenderAccountId       AS TransferSenderAccountId,  
  tl.TransferSenderAccountName      AS TransferSenderAccountName,  
  tl.TransferReceiverAccountId      AS TransferReceiverAccountId,  
  tl.TransferReceiverAccountName      AS TransferReceiverAccountName     
  FROM [employer_financial].[TransactionLine] tl  
  
  JOIN [employer_financial].[TransactionLineTypes] tlt  
    ON tl.TransactionType = tlt.TransactionType  
  LEFT JOIN   
 (SELECT tl2.AccountId, tr2.CourseName, tl2.PeriodEnd
  FROM [employer_financial].[TransactionLine] tl2  
  
  LEFT JOIN [employer_financial].[AccountTransfers] tr2  
   ON (tr2.ReceiverAccountId = tl2.AccountId and tr2.PeriodEnd = tl2.PeriodEnd)   
  
  WHERE tl2.AccountId = @AccountId   
  AND tl2.TransactionType = 4 -- Transfer  
  ) as trans  
   on trans.AccountId = tl.AccountId  
   and trans.PeriodEnd = tl.PeriodEnd     
  
   LEFT JOIN   
   (SELECT DISTINCT p3.AccountId, p3.PeriodEnd, m3.ProviderName, m3.ApprenticeshipCourseLevel as 'CourseLevel'  
  FROM [employer_financial].[Payment] p3  
  INNER JOIN [employer_financial].[PaymentMetaData] m3   
   ON m3.Id = p3.PaymentMetaDataId  
   AND p3.AccountId = @AccountId  
  WHERE m3.ProviderName IS NOT NULL  
   ) as meta  
   on meta.AccountId = tl.TransferReceiverAccountId  
   and meta.PeriodEnd = tl.PeriodEnd  
  
  WHERE tl.AccountId = @AccountId   
  AND tl.AccountId = tl.TransferReceiverAccountId
  AND tl.TransactionType IN (3,  4) -- Payment and Transfer  
  GROUP BY   
  tl.DateCreated,   
  tl.AccountId,   
  tlt.[Description],  
  tl.PeriodEnd,   
  meta.ProviderName,  
  trans.CourseName,  
  meta.CourseLevel,  
  tl.TransferSenderAccountId,   
  tl.TransferSenderAccountName,   
  tl.TransferReceiverAccountId,   
  tl.TransferReceiverAccountName