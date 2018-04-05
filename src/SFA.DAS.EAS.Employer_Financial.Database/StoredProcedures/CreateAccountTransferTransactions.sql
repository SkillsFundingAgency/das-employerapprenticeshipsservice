CREATE PROCEDURE [employer_financial].[CreateAccountTransferTransactions]
	@accountId bigint,
	@senderAccountId bigint,
	@senderAccountName nvarchar(100),
	@receiverAccountId bigint,
	@receiversAccountName nvarchar(100),
	@periodEnd nvarchar(20),
	@amount decimal(18,4),
	@transactionType smallint,
	@transactionDate datetime
AS	
	--Create transfer sender transaction
	INSERT INTO [employer_financial].[TransactionLine]
	(
		AccountId
		,DateCreated 		
		,TransactionDate 
		,TransactionType 
		,Amount 		
		,PeriodEnd 		
		,TransferSenderAccountId
		,TransferReceiverAccountId
		,TransferReceiverAccountName		
		,TransferSenderAccountName
	)
	VALUES
	(
		@accountId,
		GETDATE(),
		@transactionDate,
		@transactionType,
		@amount,
		@periodEnd,
		@senderAccountId,
		@receiverAccountId,
		@receiversAccountName,
		@senderAccountName
	)
