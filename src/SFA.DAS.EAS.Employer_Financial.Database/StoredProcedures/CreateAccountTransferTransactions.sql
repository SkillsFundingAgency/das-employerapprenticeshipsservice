CREATE PROCEDURE [employer_financial].[CreateAccountTransferTransactions]
	@transferTransactions [employer_financial].[TransferTransactionsTable] READONLY
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
		,TransferSenderAccountName
		,TransferReceiverAccountId
		,TransferReceiverAccountName			
	)
	SELECT	
		tt.AccountId,
		GETDATE(),
		tt.TransactionDate,
		tt.TransactionType,
		tt.Amount,
		tt.PeriodEnd,
		tt.SenderAccountId,
		tt.SenderAccountName,
		tt.ReceiverAccountId,
		tt.ReceiverAccountName		
	FROM @transferTransactions tt
