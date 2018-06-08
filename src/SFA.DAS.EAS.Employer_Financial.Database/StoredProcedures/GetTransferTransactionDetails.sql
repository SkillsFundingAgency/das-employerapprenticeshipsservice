CREATE PROCEDURE [employer_financial].[GetTransferTransactionDetails]
	@accountId BIGINT,
	@targetAccountId BIGINT,
	@periodEnd NVARCHAR(20)	
AS
	SELECT * FROM [employer_financial].[AccountTransfers] 	
	WHERE ((SenderAccountId = @accountId AND ReceiverAccountId = @targetAccountId)
	OR (SenderAccountId = @targetAccountId AND ReceiverAccountId = @accountId))
	AND PeriodEnd = @periodEnd	
	
