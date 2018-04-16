CREATE PROCEDURE [employer_financial].[CreateAccountTransferTransactions]
	@senderAccountId bigint,
	@receiverAccountId bigint,
	@receiversAccountName nvarchar(100),
	@periodEnd nvarchar(20),
	@amount decimal,
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
		,TransferReceiverAccountId
		,TransferReceiverAccountName		
	)
	VALUES
	(
		@senderAccountId,
		GETDATE(),
		@transactionDate,
		@transactionType,
		@amount,
		@periodEnd,
		@receiverAccountId,
		@receiversAccountName
	)
