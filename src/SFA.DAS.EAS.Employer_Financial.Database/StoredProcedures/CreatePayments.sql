CREATE PROCEDURE [employer_financial].[CreatePayments]
	@payments [employer_financial].[PaymentsTable] READONLY
AS
	DECLARE @paymentMetaDataIds TABLE (PaymentId UNIQUEIDENTIFIER, PaymentMetaDataId BIGINT)

	MERGE [employer_financial].[PaymentMetaData]
	USING @payments p
	ON 0 = 1
	WHEN NOT MATCHED THEN
		INSERT (
			ProviderName,
			StandardCode,
			FrameworkCode,		
			ProgrammeType,
			PathwayCode,
			PathwayName,
			ApprenticeshipCourseName,
			ApprenticeName,
			ApprenticeNINumber,
			ApprenticeshipCourseLevel,
			ApprenticeshipCourseStartDate
		) VALUES (
			p.ProviderName,
			p.StandardCode,
			p.FrameworkCode,		
			p.ProgrammeType,
			p.PathwayCode,
			p.PathwayName,
			p.ApprenticeshipCourseName,
			p.ApprenticeName,
			p.ApprenticeNINumber,
			p.ApprenticeshipCourseLevel,
			p.ApprenticeshipCourseStartDate
		)
		OUTPUT p.PaymentId, INSERTED.Id INTO @paymentMetaDataIds;		

	INSERT INTO [employer_financial].[Payment] (
		PaymentId,
		Ukprn,
		Uln,
		AccountId,
		ApprenticeshipId,
		DeliveryPeriodMonth,
		DeliveryPeriodYear,
		CollectionPeriodId,
		CollectionPeriodMonth,
		CollectionPeriodYear,
		EvidenceSubmittedOn,
		EmployerAccountVersion,
		ApprenticeshipVersion,
		FundingSource,
		TransactionType,
		Amount,
		PeriodEnd,
		PaymentMetaDataId,
		DateImported
	)
	SELECT
		p.PaymentId,
		p.Ukprn,
		p.Uln,
		p.AccountId,
		p.ApprenticeshipId,
		p.DeliveryPeriodMonth,
		p.DeliveryPeriodYear,
		p.CollectionPeriodId,
		p.CollectionPeriodMonth,
		p.CollectionPeriodYear,
		p.EvidenceSubmittedOn,
		p.EmployerAccountVersion,
		p.ApprenticeshipVersion,
		p.FundingSource,
		p.TransactionType,
		p.Amount,
		p.PeriodEnd,
		m.PaymentMetaDataId,
		GETUTCDATE()
	FROM @payments p
	INNER JOIN @paymentMetaDataIds m ON m.PaymentId = p.PaymentId