CREATE PROCEDURE [employer_financial].[CreateAccountTransfer]
	@senderAccountId bigint,
	@recieverAccountId bigint,
	@commitmentId bigint,
	@amount decimal,
	@periodEnd nvarchar(20),
	@type smallint,
	@transferDate datetime

AS
	INSERT INTO [employer_financial].[AccountTransfers] 
	(
		SenderAccountId, 
		RecieverAccountId, 
		ApprenticeshipId, 
		PeriodEnd,
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
		@periodEnd,
		@amount,
		@type,
		@transferDate,
		GETDATE()
	)
