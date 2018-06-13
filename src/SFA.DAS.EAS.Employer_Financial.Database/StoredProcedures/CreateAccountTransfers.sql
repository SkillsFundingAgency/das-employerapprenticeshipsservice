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
		CourseLevel,
		PeriodEnd,
		Amount, 
		Type, 		
		CreatedDate,
		RequiredPaymentId
	)
	SELECT	
		t.SenderAccountId,
		t.SenderAccountName,
		t.ReceiverAccountId,
		t.ReceiverAccountName,
		t.ApprenticeshipId,
		t.CourseName,
		t.CourseLevel,
		t.PeriodEnd,
		t.Amount,
		t.Type,		
		GETDATE(),
		RequiredPaymentId
	FROM @transfers t