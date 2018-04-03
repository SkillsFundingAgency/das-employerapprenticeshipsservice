CREATE PROCEDURE [employer_financial].[GetAccountTransfersByPeriodEnd]
	@receiverAccountId BIGINT,
	@periodEnd NVARCHAR(20)
AS
	SELECT * FROM [employer_financial].[AccountTransfers]
	WHERE ReceiverAccountId = @receiverAccountId
	AND PeriodEnd = @periodEnd

