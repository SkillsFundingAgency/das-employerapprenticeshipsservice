UPDATE [SFA.DAS.EAS.Employer_Account.Database].[employer_account].[Account]
SET ApprenticeshipEmployerType = 1
WHERE Id IN (
	SELECT DISTINCT AccountId
	FROM [SFA.DAS.EAS.Employer_Financial.Database].[employer_financial].[TransactionLine]
	WHERE TransactionType = 1
	AND Amount > 0
)