CREATE PROCEDURE [employer_financial].[CreateAccountTransfersV1]
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
	Select 
		tr.SenderAccountId, 
		actSender.Name, 
		tr.ReceiverAccountId, 
		actReceiver.Name, 
		tr.ApprenticeshipId, 
		IsNull(Ir.CourseName, 'Unknown Course'), 
		Ir.CourseLevel, 
		tr.PeriodEnd, 
		tr.Amount, 
		tr.Type, GetDate(), 
		tr.RequiredPaymentId  
		FROM 
			(SELECT
				p.AccountId,
				p.PeriodEnd,
				p.ApprenticeshipId,
				meta.ApprenticeshipCourseName as CourseName
				,meta.ApprenticeshipCourseLevel as CourseLevel
				,COUNT(DISTINCT Uln) as ApprenticeCount -- This can be commented out not used any where.
				,SUM(p.Amount) as PaymentTotal		-- This can also be commented out not used any where.
				FROM [employer_financial].[Payment] p
				INNER JOIN [employer_financial].[PaymentMetaData] meta ON p.PaymentMetaDataId = meta.Id
				Inner join @transfers tr on p.AccountId = tr.ReceiverAccountId and p.PeriodEnd = tr.PeriodEnd and p.ApprenticeshipId = tr.ApprenticeshipId
				GROUP BY meta.ApprenticeshipCourseName, meta.ApprenticeshipCourseLevel, p.AccountId, p.PeriodEnd, p.ApprenticeshipId)  Ir
		-- assumption combination of ReceiverAccountId,PeriodEnd & ApprenticeshipId gives a unique row for the information received from payments.
		Inner join @transfers tr on Ir.AccountId = tr.ReceiverAccountId and Ir.PeriodEnd = tr.PeriodEnd and Ir.ApprenticeshipId = tr.ApprenticeshipId
		Inner join [employer_financial].[Account] actSender on actSender.Id = tr.SenderAccountId
		Inner join [employer_financial].[Account] actReceiver on actReceiver.Id = tr.ReceiverAccountId
