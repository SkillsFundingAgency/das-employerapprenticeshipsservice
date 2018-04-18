CREATE PROCEDURE [employer_financial].[GetSenderAccountTransactionsInCurrentFinancialYear]
	@senderAccountId BIGINT	
AS
	SELECT transfers.* FROM [employer_financial].[AccountTransfers] transfers
	CROSS JOIN employer_financial.GetPreviousFinancialYearDates(DEFAULT) as previousFinancialYear
	WHERE transfers.SenderAccountId = @senderAccountId
	AND transfers.TransferDate >= previousFinancialYear.YearEnd