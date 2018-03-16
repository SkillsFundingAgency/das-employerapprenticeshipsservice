CREATE PROCEDURE [employer_financial].[GetAccountTransfersByPeriodEnd]
	@senderAccountId BIGINT,
	@periodEnd NVARCHAR(20)
AS
	SELECT * FROM [employer_financial].[AccountTransfers]
	WHERE SenderAccountId = @senderAccountId
	AND PeriodEnd = @periodEnd

