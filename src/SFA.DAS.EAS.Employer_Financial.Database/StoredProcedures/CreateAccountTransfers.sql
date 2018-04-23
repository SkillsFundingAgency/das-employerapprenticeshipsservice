CREATE PROCEDURE [employer_financial].[CreateAccountTransfers]
	@transfers [employer_financial].[AccountTransferTable] READONLY
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
	SELECT	
		t.SenderAccountId,
		t.SenderAccountName,
		t.ReceiverAccountId,
		t.ReceiverAccountName,
		t.ApprenticeshipId,
		t.CourseName,
		t.PeriodEnd,
		t.Amount,
		t.Type,
		t.TransferDate,
		GETDATE()	
	FROM @transfers t