CREATE PROCEDURE [employer_financial].[GetAccountTransferAllowance]
	@accountId BIGINT = 0,
	@allowancePercentage DECIMAL(3,2) = 0
AS
	SET NOCOUNT ON

	SELECT	SUM(Amount) * @allowancePercentage 
	FROM		[employer_financial].[TransactionLine] as lines
			CROSS JOIN employer_financial.GetPreviousFinancialYearDates(DEFAULT) as previousFinancialYear
	WHERE	TransactionDate >= previousFinancialYear.YearStart
			AND TransactionDate < previousFinancialYear.YearEnd
			AND TransactionType = 1
			AND AccountId = @accountId

GO
