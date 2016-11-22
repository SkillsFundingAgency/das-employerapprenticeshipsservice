CREATE PROCEDURE [levy].[CreatePayment]
	@PaymentId as uniqueidentifier,
	@Ukprn as BIGINT,
	@ProviderName as NVARCHAR(250),
	@Uln as BIGINT,
	@AccountId as BIGINT,
	@ApprenticeshipId as BIGINT,
	@DeliveryPeriodMonth as INT,
	@DeliveryPeriodYear as INT,
	@CollectionPeriodId as CHAR(8),
	@CollectionPeriodMonth as INT,
	@CollectionPeriodYear as INT,
	@EvidenceSubmittedOn as DATETIME,
	@EmployerAccountVersion as VARCHAR(50),
	@ApprenticeshipVersion as VARCHAR(10),
	@FundingSource as VARCHAR(25),
	@TransactionType as VARCHAR(25),
	@Amount as Decimal(18,5),
	@PeriodEnd as Varchar(25)
as

INSERT INTO [levy].[Payment]
           ([PaymentId]
           ,[Ukprn]
		   ,[ProviderName]
           ,[Uln]
           ,[AccountId]
           ,[ApprenticeshipId]
           ,[DeliveryPeriodMonth]
           ,[DeliveryPeriodYear]
           ,[CollectionPeriodId]
           ,[CollectionPeriodMonth]
           ,[CollectionPeriodYear]
           ,[EvidenceSubmittedOn]
           ,[EmployerAccountVersion]
           ,[ApprenticeshipVersion]
           ,[FundingSource]
           ,[TransactionType]
           ,[Amount]
		   ,[PeriodEnd])
     VALUES
           (@PaymentId
           ,@Ukprn
		   ,@ProviderName
           ,@Uln
           ,@AccountId
           ,@ApprenticeshipId
           ,@DeliveryPeriodMonth
           ,@DeliveryPeriodYear
           ,@CollectionPeriodId
           ,@CollectionPeriodMonth
           ,@CollectionPeriodYear
           ,@EvidenceSubmittedOn
           ,@EmployerAccountVersion
           ,@ApprenticeshipVersion
           ,@FundingSource
           ,@TransactionType
           ,@Amount
		   ,@PeriodEnd)
GO


