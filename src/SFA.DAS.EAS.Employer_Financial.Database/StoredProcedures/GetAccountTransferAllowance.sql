CREATE PROCEDURE [employer_financial].[GetAccountTransferAllowance]
	@AccountId BIGINT = 0,
	@allowancePercentage DECIMAL(3,2) = 0
AS
	SET NOCOUNT ON

	SELECT	(SUM(lines.Amount) * @allowancePercentage) - SUM(transfers.Amount)
	FROM		[employer_financial].[TransactionLine] as lines
			INNER JOIN [employer_financial].[AccountTransfers] transfers ON lines.AccountId = transfers.SenderAccountId
			CROSS JOIN employer_financial.GetPreviousFinancialYearDates(DEFAULT) as previousFinancialYear
	WHERE	lines.TransactionDate >= previousFinancialYear.YearStart
			AND lines.TransactionDate < previousFinancialYear.YearEnd
			AND lines.TransactionType = 1			
			AND lines.AccountId = @AccountId
			AND transfers.TransferDate >= previousFinancialYear.YearEnd
GO
