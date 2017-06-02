﻿CREATE PROCEDURE [employer_financial].[CreatePayment]
	@PaymentId as uniqueidentifier,
	@Ukprn as BIGINT,
	@ProviderName as VARCHAR(MAX),
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
	@PeriodEnd as Varchar(25),
	@StandardCode as BIGINT,
	@FrameworkCode as INT,
	@ProgrammeType as INT,
	@PathwayCode as INT,
	@PathwayName as VARCHAR(MAX),
	@CourseName as VARCHAR(MAX),
	@ApprenticeName as NVARCHAR(MAX),
	@ApprenticeNINumber as VARCHAR(MAX),
	@ApprenticeshipCourseLevel as INT,
	@ApprenticeshipCourseStartDate as DATETIME
as

INSERT INTO [employer_financial].[PaymentMetaData]
			(ProviderName
			,StandardCode
			,FrameworkCode			
			,ProgrammeType
			,PathwayCode
			,PathwayName
			,ApprenticeshipCourseName
			,ApprenticeName
			,ApprenticeNINumber
			,ApprenticeshipCourseLevel
			,ApprenticeshipCourseStartDate)
		VALUES
			(@ProviderName
			,@StandardCode
			,@FrameworkCode			
			,@ProgrammeType
			,@PathwayCode
			,@PathwayName
			,@CourseName
			,@ApprenticeName
			,@ApprenticeNINumber
			,@ApprenticeshipCourseLevel
			,@ApprenticeshipCourseStartDate)

DECLARE @PaymentMetaDataId AS BIGINT = SCOPE_IDENTITY()

INSERT INTO [employer_financial].[Payment]
           ([PaymentId]
           ,[Ukprn]		   
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
		   ,[PeriodEnd]
		   ,[PaymentMetaDataId])
     VALUES
           (@PaymentId
           ,@Ukprn		   
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
		   ,@PeriodEnd
		   ,@PaymentMetaDataId)
GO


