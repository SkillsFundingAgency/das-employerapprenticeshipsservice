CREATE PROCEDURE [employer_financial].[CreateAccountTransfer]
	@senderAccountId bigint,
	@recieverAccountId bigint,
	@commitmentId bigint,
	@amount decimal,
	@type smallint,
	@transferDate datetime

AS
	INSERT INTO [employer_financial].[AccountTransfers] 
	(
		SenderAccountId, 
		RecieverAccountId, 
		CommitmentId, 
		Amount, 
		Type, 
		TransferDate, 
		CreatedDate
	)
	VALUES
	(
		@senderAccountId,
		@recieverAccountId,
		@commitmentId,
		@amount,
		@type,
		@transferDate,
		GETDATE()
	)
