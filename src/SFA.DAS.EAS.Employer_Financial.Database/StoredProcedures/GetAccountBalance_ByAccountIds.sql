CREATE PROCEDURE [employer_financial].[GetAccountBalance_ByAccountIds]
	@AccountIds [employer_financial].[AccountIds] Readonly,
	@allowancePercentage DECIMAL(8,5) = 0
AS
	SELECT
		acc.AccountId,
		isnull(bal.Balance,0) as Balance,
		isnull(bal.RemainingTransferAllowance, 0) as RemainingTransferAllowance,
		isnull(bal.StartingTransferAllowance, 0) as StartingTransferAllowance,
		CASE
			WHEN IsLevyPayer IS NOT NULL THEN IsLevyPayer
			WHEN HasDeclaredLevy = 1 AND AmountLevyDeclared > 0 THEN 1
			WHEN HasDeclaredLevy = 1 AND AmountLevyDeclared = 0 THEN 1 -- this needs to go back to 0 after month 1
			ELSE 1
		END IsLevyPayer
	FROM @AccountIds acc
	OUTER APPLY
	(
		SELECT TOP 1 IsLevyPayer
		FROM [employer_financial].LevyOverride lo
		WHERE lo.AccountId = acc.AccountId 
		ORDER BY [DateAdded] DESC
	) t
	OUTER APPLY
	(
		SELECT 
			CASE
				WHEN COUNT(1) > 0 THEN 1
				ELSE 0
			END HasDeclaredLevy,
		SUM(LevyDueYTD) AmountLevyDeclared
		FROM employer_financial.LevyDeclaration ld
		WHERE ld.AccountId = acc.AccountId
	) x
	LEFT JOIN
	(
		SELECT
			AccountId,
			SUM(Amount) Balance,
			SUM(CASE 
					WHEN TransactionDate >= previousFinancialYear.YearStart
						AND TransactionDate < previousFinancialYear.YearEnd
						AND TransactionType = 1 THEN Amount * @allowancePercentage
					ELSE 0
				END) +
				SUM(CASE 
					WHEN TransactionDate >= previousFinancialYear.YearEnd						
						AND TransactionType = 4
						AND TransferSenderAccountId = AccountId THEN Amount
					ELSE 0
				END) AS RemainingTransferAllowance,
		SUM(CASE 
					WHEN TransactionDate >= previousFinancialYear.YearStart
						AND TransactionDate < previousFinancialYear.YearEnd
						AND TransactionType = 1 THEN Amount * @allowancePercentage
					ELSE 0
				END) AS StartingTransferAllowance
		FROM employer_financial.TransactionLine
		CROSS JOIN employer_financial.GetPreviousFinancialYearDates(DEFAULT) as previousFinancialYear
		GROUP BY AccountId
	) bal
		ON acc.AccountId = bal.AccountId

