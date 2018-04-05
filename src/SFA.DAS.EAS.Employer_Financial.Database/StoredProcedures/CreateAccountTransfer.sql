CREATE PROCEDURE [employer_financial].[CreateAccountTransfer]
	@senderAccountId bigint,
	@senderAccountName NVARCHAR(100),
	@receiverAccountId bigint,
	@receiverAccountName NVARCHAR(100),
	@apprenticeshipId bigint,
	@courseName varchar(max),	
	@amount decimal(18,5),
	@periodEnd nvarchar(20),
	@type smallint,
	@transferDate datetime

AS
	INSERT INTO [employer_financial].[AccountTransfers] 
	(
		SenderAccountId, 
		SenderAccountName,
		ReceiverAccountId, 
		ReceiverAccountName,
		ApprenticeshipId, 
		CourseName,	
		PeriodEnd,
		Amount, 
		Type, 
		TransferDate, 
		CreatedDate
	)
	VALUES
	(
		@senderAccountId,
		@senderAccountName,
		@receiverAccountId,
		@receiverAccountName,
		@apprenticeshipId,
		@courseName,		
		@periodEnd,
		@amount,
		@type,
		@transferDate,
		GETDATE()
	)
