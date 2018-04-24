CREATE PROCEDURE [employer_financial].[GetAccountTransferAllowance]
	@AccountId BIGINT = 0,
	@allowancePercentage DECIMAL(3,2) = 0
AS
	SET NOCOUNT ON

	DECLARE @previousYearLevyTotal DECIMAL(18,4)
	DECLARE @transferAllowance DECIMAL(18,4)
	DECLARE @transferSpent DECIMAL(18,5)

	-- Get last years total levy
	SELECT	@previousYearLevyTotal = SUM(lines.Amount)
	FROM		[employer_financial].[TransactionLine] as lines			
			CROSS JOIN employer_financial.GetPreviousFinancialYearDates(DEFAULT) as previousFinancialYear
	WHERE	lines.TransactionDate >= previousFinancialYear.YearStart
			AND lines.TransactionDate < previousFinancialYear.YearEnd
			AND lines.TransactionType = 1			
			AND lines.AccountId = @AccountId	

	-- Get the transfer allowance for the current year
	SET @transferAllowance = @previousYearLevyTotal * @allowancePercentage

	-- Get total transfer allowance spent this year
	SELECT @transferSpent = SUM(transfers.Amount) FROM [employer_financial].[AccountTransfers] transfers
	CROSS JOIN employer_financial.GetPreviousFinancialYearDates(DEFAULT) as previousFinancialYear
	WHERE transfers.TransferDate >= previousFinancialYear.YearEnd
	AND transfers.SenderAccountId = @AccountId	

	-- Return the current available transfer allowance	
	SELECT @transferAllowance - @transferSpent
GO
