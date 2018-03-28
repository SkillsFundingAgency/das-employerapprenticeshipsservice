CREATE PROCEDURE [employer_financial].[GetTransferSenderTransactionDetails]
	@senderAccountId BIGINT,
	@receiverAccountId BIGINT,
	@periodEnd NVARCHAR(20)	
AS
	SELECT * FROM [employer_financial].[AccountTransfers] 	
	WHERE SenderAccountId = @senderAccountId
	AND ReceiverAccountId = @receiverAccountId
	AND PeriodEnd = @periodEnd	
	
