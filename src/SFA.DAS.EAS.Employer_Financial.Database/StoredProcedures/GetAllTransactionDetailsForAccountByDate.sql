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
				SUM(CASE WHEN p.FundingSource in (1,5) THEN -p.Amount ELSE 0 END) AS fundedFromLevy, 
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
				AND p.FundingSource in (1,2,3,5)
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
				ON tlt.TransactionType = 5
	WHERE	tl.AccountId = @accountId 
			AND DateCreated >= @FromDate 
			AND DateCreated < @ToDate
			AND tl.TransactionType = 5
			AND tl.Amount <> 0
			
UNION ALL

-- sender transfers
SELECT  DATEADD(dd, DATEDIFF(dd, 0, [employer_financial].[AccountTransfers].CreatedDate), 0)	AS DateCreated,
		[employer_financial].[AccountTransfers].SenderAccountId									AS AccountId,
		'Transfer'																				AS TransactionType,
		NULL																					AS PayeScheme,
		NULL																					AS PayrollYear,
		NULL																					AS PayrollMonth,
		NULL																					AS LevyDeclared,
		NULL																					AS EnglishFraction,
		NULL																					AS TenPercentTopUp,
		NULL																					AS TrainingProvider,
		NULL																					AS Uln,
		NULL																					AS Apprentice,
		[employer_financial].[AccountTransfers].CourseName										AS ApprenticeTrainingCourse,
		[employer_financial].[AccountTransfers].CourseLevel										AS ApprenticeTrainingCourseLevel,
		cast([employer_financial].[AccountTransfers].[Amount]as decimal(18,4))					AS PaidFromLevy,
		NULL																					AS EmployerContribution,
		NULL																					AS GovermentContribution,
		-cast([employer_financial].[AccountTransfers].[Amount] as decimal(18,4))				AS Total,
		[employer_financial].[AccountTransfers].SenderAccountID									AS TransferSenderAccountId,
		[employer_financial].[AccountTransfers].[SenderAccountName]								AS TransferSenderAccountName,
		[employer_financial].[AccountTransfers].[ReceiverAccountId]								AS TransferReceiverAccountId,
		[employer_financial].[AccountTransfers].[ReceiverAccountName]							AS TransferReceiverAccountName 
	FROM [employer_financial].[AccountTransfers]
	WHERE [employer_financial].[AccountTransfers].[SenderAccountId] = @AccountId
		AND [employer_financial].[AccountTransfers].[CreatedDate] >= @FromDate
		AND [employer_financial].[AccountTransfers].[CreatedDate] < @ToDate

UNION ALL

-- receiver transfers
SELECT  DATEADD(dd, DATEDIFF(dd, 0, [employer_financial].[AccountTransfers].CreatedDate), 0)	AS DateCreated,
		[employer_financial].[AccountTransfers].SenderAccountId									AS AccountId,
		'Transfer'																				AS TransactionType,
		NULL																					AS PayeScheme,
		NULL																					AS PayrollYear,
		NULL																					AS PayrollMonth,
		NULL																					AS LevyDeclared,
		NULL																					AS EnglishFraction,
		NULL																					AS TenPercentTopUp,
		NULL																					AS TrainingProvider,
		NULL																					AS Uln,
		NULL																					AS Apprentice,
		[employer_financial].[AccountTransfers].CourseName										AS ApprenticeTrainingCourse,
		[employer_financial].[AccountTransfers].CourseLevel										AS ApprenticeTrainingCourseLevel,
		cast([employer_financial].[AccountTransfers].[Amount] as decimal(18,4))					AS PaidFromLevy,
		NULL																					AS EmployerContribution,
		NULL																					AS GovermentContribution,
		cast([employer_financial].[AccountTransfers].[Amount] as decimal(18,4))				AS Total,
		[employer_financial].[AccountTransfers].SenderAccountID									AS TransferSenderAccountId,
		[employer_financial].[AccountTransfers].[SenderAccountName]								AS TransferSenderAccountName,
		[employer_financial].[AccountTransfers].[ReceiverAccountId]								AS TransferReceiverAccountId,
		[employer_financial].[AccountTransfers].[ReceiverAccountName]							AS TransferReceiverAccountName
	FROM [employer_financial].[AccountTransfers]
	WHERE [employer_financial].[AccountTransfers].[ReceiverAccountId] = @AccountId
		AND [employer_financial].[AccountTransfers].[CreatedDate] >= @FromDate
		AND [employer_financial].[AccountTransfers].[CreatedDate] < @ToDate
	ORDER BY datecreated