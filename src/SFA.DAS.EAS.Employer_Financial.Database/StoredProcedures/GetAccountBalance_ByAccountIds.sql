CREATE PROCEDURE [employer_financial].[GetAccountBalance_ByAccountIds]
	@accountIds [employer_financial].[AccountIds] Readonly
AS
	
	SELECT
		acc.AccountId,
		isnull(bal.Balance,0) as Balance,
		CASE
			WHEN IsLevyPayer IS NOT NULL THEN IsLevyPayer
			WHEN HasDeclaredLevy = 1 AND AmountLevyDeclared > 0 THEN 1
			WHEN HasDeclaredLevy = 1 AND AmountLevyDeclared = 0 THEN 1 -- this needs to go back to 0 after month 1
			ELSE 1
		END IsLevyPayer
	FROM @accountIds acc
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
			SUM(Amount) Balance
		FROM employer_financial.TransactionLine
		GROUP BY AccountId
	) bal
		ON acc.AccountId = bal.AccountId

