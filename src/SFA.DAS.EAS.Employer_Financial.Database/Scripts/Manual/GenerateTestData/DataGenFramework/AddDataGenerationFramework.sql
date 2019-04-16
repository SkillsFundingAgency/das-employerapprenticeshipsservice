
-- Schema

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'DataGen')
BEGIN
	EXEC('CREATE SCHEMA DataGen')
END
GO

-- Types

IF TYPE_ID(N'DataGen.PaymentGenerationSourceTable') IS NULL
BEGIN
	CREATE TYPE DataGen.PaymentGenerationSourceTable AS TABLE   
	(monthBeforeToDate int, amount decimal(18, 4), paymentsToGenerate int, createMonth datetime)
END
GO

IF TYPE_ID(N'DataGen.LevyGenerationSourceTable') IS NULL
BEGIN
	CREATE TYPE DataGen.LevyGenerationSourceTable AS TABLE   
	(/*rowNumber int IDENTITY, */monthBeforeToDate int, amount decimal(18, 4), createMonth datetime, payrollYear varchar(5), payrollMonth int)
END
GO

-- Functions

CREATE OR ALTER FUNCTION DataGen.CalendarPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)
  RETURN(@month);  
END; 
GO

CREATE OR ALTER FUNCTION DataGen.CalendarPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  DECLARE @year int = DATEPART(year,@date)
  RETURN @year
END; 
GO

CREATE OR ALTER FUNCTION DataGen.PayrollMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)

  SET @month = @month - 3
  IF @month < 1
	SET @month = @month + 12

  RETURN(@month);  
END; 
GO

CREATE OR ALTER FUNCTION DataGen.PayrollYear (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)
  DECLARE @year int = DATEPART(year,@date)

  IF @month < 4
	SET @year = @year - 1
	
  DECLARE @payrollYear varchar(5) = (SELECT RIGHT(CONVERT(varchar(5), @year, 1), 2)) + '-' + (SELECT RIGHT(CONVERT(varchar(4), @year+1, 1), 2))
  RETURN @payrollYear
END; 
GO

CREATE OR ALTER FUNCTION DataGen.CollectionPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  DECLARE @collectionPeriodMonth int = (SELECT DataGen.CalendarPeriodMonth(@date))
  RETURN @collectionPeriodMonth
END; 
GO

CREATE OR ALTER FUNCTION DataGen.CollectionPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  DECLARE @collectionPeriodYear int = (SELECT DataGen.CalendarPeriodYear(@date))
  RETURN @collectionPeriodYear
END; 
GO

--todo R13, R14
CREATE OR ALTER FUNCTION DataGen.PeriodEndMonth (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)

  SET @month = @month - 7
  IF @month < 1
	SET @month = @month + 12

  DECLARE @periodEndMonth varchar(5) = CONVERT(varchar(5), @month)
  RETURN @periodEndMonth; 
END; 
GO

--todo R13, R14
CREATE OR ALTER FUNCTION DataGen.PeriodEndYear (@date datetime)  
RETURNS varchar(5)
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)
  DECLARE @year int = DATEPART(year,@date)

  IF @month < 8
	SET @year = @year - 1
	
  DECLARE @periodEndYear varchar(4) = (SELECT RIGHT(CONVERT(varchar(5), @year, 1), 2)) + (SELECT RIGHT(CONVERT(varchar(4), @year+1, 1), 2))
  RETURN @periodEndYear
END; 
GO

CREATE OR ALTER FUNCTION DataGen.PeriodEnd (@date datetime)  
RETURNS varchar(8)
AS  
BEGIN  
  DECLARE @periodEnd varchar(8) = (SELECT DataGen.PeriodEndYear(@date) + '-R' + RIGHT('0' + DataGen.PeriodEndMonth(@date),2))
  RETURN @periodEnd
END; 
GO

--better name? GeneratePaymentSourceTable
CREATE OR ALTER FUNCTION DataGen.GenerateSourceTable(
	@toDate datetime,
	@numberOfMonthsToCreate int,
	@defaultMonthlyTotalPayments decimal(18,5),
	@defaultPaymentsPerMonth int)
RETURNS @source TABLE (
    /*rowNumber int IDENTITY,*/ monthBeforeToDate int, amount decimal(18, 4), paymentsToGenerate int, createMonth datetime)
BEGIN
	INSERT INTO @source
	SELECT TOP (@numberOfMonthsToCreate)
				monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
				@defaultMonthlyTotalPayments,
				@defaultPaymentsPerMonth,
				DATEADD(month,/*monthBeforeToDate*/ -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)
	FROM sys.all_objects
	ORDER BY monthBeforeToDate;

    RETURN
END
GO

CREATE OR ALTER FUNCTION DataGen.GenerateLevySourceTable(
	@toDate datetime,
	@numberOfMonthsToCreate int,
	@monthlyLevy decimal(18, 4))
RETURNS @source TABLE
    (monthBeforeToDate int, amount decimal(18, 4), createMonth datetime, payrollYear varchar(5), payrollMonth int)
BEGIN

	DECLARE @firstPayrollMonth datetime = DATEADD(month,-@numberOfMonthsToCreate+1-1,@toDate)
	DECLARE @firstPayrollYear varchar(5) = DataGen.PayrollYear(@firstPayrollMonth)

	-- generates same levy per month
	INSERT INTO @source
	SELECT TOP (@numberOfMonthsToCreate)
				monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
				(CASE
				WHEN DataGen.PayrollYear(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)) = @firstPayrollYear 
					THEN @monthlyLevy*ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
				ELSE
					@monthlyLevy*DataGen.PayrollMonth(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate))
				END),
				DATEADD(month,/*monthBeforeToDate*/ -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate),
				DataGen.PayrollYear(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)),
				DataGen.PayrollMonth(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate))
	FROM sys.all_objects
	ORDER BY monthBeforeToDate;

    RETURN
END
GO

-- Stored Procedures

-- Add period end if its not already there
CREATE OR ALTER PROCEDURE DataGen.CreatePeriodEnd
(
	@periodEndDate datetime
)
AS
BEGIN
	DECLARE @periodEndId varchar(8) = DataGen.PeriodEnd(@periodEndDate)

	IF NOT EXISTS
	(
		SELECT 1 FROM [employer_financial].[periodend] 
		WHERE periodendid = @periodEndId
	)
	BEGIN        
		INSERT employer_financial.periodend (periodendid, calendarperiodmonth, calendarperiodyear, accountdatavalidat, commitmentdatavalidat, completiondatetime, paymentsforperiod)
		VALUES (@periodEndId, DataGen.CalendarPeriodMonth(@periodEndDate), DataGen.CalendarPeriodYear(@periodEndDate), '2018-05-04 00:00:00.000', '2018-05-04 09:07:34.457', '2018-05-04 10:50:27.760', 'https://pp-payments.apprenticeships.sfa.bis.gov.uk/api/payments?periodId=' + @periodEndId)
	END
END
GO

CREATE OR ALTER PROCEDURE DataGen.CreateEnglishFractions(
	@payeScheme nvarchar(50),
	@levyDecByMonth LevyGenerationSourceTable READONLY
)
AS
BEGIN

	-- engligh fractions usually generated on 1, 4, 7, 10 (not 3, 6, 9, 12), but can be generated anytime
	DECLARE @englishFractionMonths TABLE (dateCalculated datetime)

	-- first quarter before (or on) first month
	INSERT @englishFractionMonths
	SELECT TOP 1 DATEADD(month,-DATEPART(month,createMonth)%3, createMonth) FROM @levyDecByMonth ORDER BY createMonth

	-- rest of the quarters
	INSERT @englishFractionMonths
	SELECT createMonth FROM (SELECT createMonth FROM @levyDecByMonth EXCEPT SELECT TOP 1 createMonth FROM @levyDecByMonth ORDER BY createMonth) x WHERE DATEPART(month,createMonth)%3 = 0

	-- only insert english fraction rows that don't already exist (and add english fraction calcs on consistent day of the month)
	DECLARe @newEnglishFractionMonths TABLE (dateCalculated datetime)

	INSERT @newEnglishFractionMonths
	SELECT DATEFROMPARTS(DATEPART(year,dateCalculated), DATEPART(month,dateCalculated), 7) FROM @englishFractionMonths
	EXCEPT SELECT dateCalculated FROM employer_financial.EnglishFraction WHERE EmpRef = @payeScheme

	INSERT employer_financial.EnglishFraction (DateCalculated, Amount, EmpRef, DateCreated)
	SELECT dateCalculated, 1.0, @payeScheme, dateCalculated FROM @newEnglishFractionMonths

END;
GO

CREATE OR ALTER PROCEDURE DataGen.CreateLevyDecs(
	@accountId bigint,
	@payeScheme nvarchar(50),
	@toDate datetime2,
	@levyDecByMonth LevyGenerationSourceTable READONLY
)
AS
BEGIN

	DECLARE @maxSubmissionId BIGINT = ISNULL((SELECT MAX(SubmissionId) FROM employer_financial.levydeclaration),0)
	DECLARE @numberOfMonths int
	
	SELECT @numberOfMonths = count(1) FROM @levyDecByMonth

	DECLARE @baselineSubmissionDate datetime = DATEFROMPARTS(year(@toDate), month(@toDate), 18)
	DECLARE @baselineCreatedDate datetime = DATEFROMPARTS(year(@toDate), month(@toDate), 20)
	DECLARE @baselinePayrollDate datetime = DATEADD(month, -1, @toDate)

	INSERT INTO employer_financial.levydeclaration (
		AccountId, empref,
		levydueytd,
		levyallowanceforyear,
		submissiondate,
		submissionid,
		payrollyear,
		payrollmonth,
		createddate,
		hmrcsubmissionid)
	SELECT @accountId, @payeScheme,
		amount,
		1500.0000,
		DATEADD(month, monthBeforeToDate, @baselineSubmissionDate),
		@maxSubmissionId + monthBeforeToDate + @numberOfMonths,
		DataGen.PayrollYear(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)),
		DataGen.PayrollMonth(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)),
		DATEADD(month, monthBeforeToDate, @baselineCreatedDate),
		@maxSubmissionId + monthBeforeToDate + @numberOfMonths
	FROM @levyDecByMonth

	--- Process the levy decs into transaction lines
	EXEC employer_financial.processdeclarationstransactions @accountId, @payeScheme

END;
GO

CREATE OR ALTER PROCEDURE DataGen.CreatePayment
(     
    @accountId bigint,
    @providerName nvarchar(max),
    @apprenticeshipCourseName nvarchar(max),   
    @apprenticeshipCourseLevel int,
    @apprenticeName varchar(max), 	
    @ukprn bigint,
    @uln bigint,
    @apprenticeshipid bigint,
    @fundingSource int,	
    @Amount decimal(18,5),
    @periodEndDate datetime  
)  
AS  
BEGIN  
    DECLARE @paymentMetadataId bigint

    INSERT INTO employer_financial.paymentmetadata 
    (ProviderName, StandardCode, FrameworkCode, ProgrammeType, PathwayCode, ApprenticeshipCourseName, ApprenticeshipCourseStartDate, ApprenticeshipCourseLevel, ApprenticeName, ApprenticeNINumber)
    VALUES
    (@providerName,4,null,null,null, @apprenticeshipCourseName,'01/06/2018', @apprenticeshipCourseLevel, @apprenticeName, null)

    SELECT @paymentMetadataId  = SCOPE_IDENTITY()

	-- @deliveryPeriodDate approx 0-6 months before collection period
	--todo: needs to be in same ay? if so, get month, knock some off, don't go below 1, then convert back to date
	DECLARE @deliveryPeriodDate datetime = DATEADD(month, -floor(rand()*6), @periodEndDate)
	-- evidencesubmittedon >= devliveryperiod (can also be > collectionperiod)
	--declare @evidenceSubmittedOn datetime = DATEADD(month, 1, @deliveryPeriodDate)

    INSERT INTO employer_financial.payment
    (paymentid, ukprn, uln, accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, 
	collectionperiodid, collectionperiodmonth, collectionperiodyear, 
	evidencesubmittedon, employeraccountversion, apprenticeshipversion, fundingsource, transactiontype, amount, periodend, paymentmetadataid)
    VALUES
    (NEWID(), @ukprn, @uln, @accountid, @apprenticeshipid, DataGen.CalendarPeriodMonth(@deliveryPeriodDate), DataGen.CalendarPeriodYear(@deliveryPeriodDate),
	DataGen.PeriodEnd(@periodEndDate), DataGen.CollectionPeriodMonth(@periodEndDate), DataGen.CollectionPeriodYear(@periodEndDate), 
	'2018-06-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount, DataGen.PeriodEnd(@periodEndDate), @paymentMetadataId)
END;  
GO  

CREATE OR ALTER PROCEDURE DataGen.CreateAccountPayments
(    	
    @accountId bigint,
    @accountName nvarchar(100),
    @providerName nvarchar(MAX),
    @ukprn bigint,
    @courseName nvarchar(MAX),
	@fundingSource int,
	@periodEndDate datetime,
    @totalAmount decimal(18,5),
	@numberOfPayments int,
	@firstApprenticeshipId bigint output
)  
AS  
BEGIN  	

    DECLARE @paymentAmount decimal(18,5) = @totalAmount / @numberOfPayments
	DECLARE @fundingSourceString varchar(25) = CAST(@fundingSource AS varchar(25))

	DECLARE @name varchar(100)
	DECLARE @uln bigint
	DECLARE @apprenticeshipId bigint

	SET @firstApprenticeshipId = (ISNULL((SELECT MAX(ApprenticeshipId) FROM employer_financial.Payment),0) + 1)

	WHILE (@numberOfPayments > 0)
	BEGIN

	  SET @numberOfPayments = @numberOfPayments - 1

	  SET @name = (CHAR(ASCII('A') + @numberOfPayments)) + ' Apprentice'
	  SET @uln = 1000000000 + @numberOfPayments
	  SET @apprenticeshipId = @firstApprenticeshipId + @numberOfPayments

      EXEC DataGen.CreatePayment @accountId, @providerName, @courseName, 1, @name, @ukprn, @uln, @apprenticeshipId, @fundingSourceString, @paymentAmount, @periodEndDate
	END
END
GO

CREATE OR ALTER PROCEDURE DataGen.CreateTransfer
(
	@senderAccountId bigint,
	@senderAccountName nvarchar(100),
	@receiverAccountId bigint,
	@receiverAccountName nvarchar(100),
	@apprenticeshipId bigint,
	@courseName varchar(max),	
	@amount decimal(18,5),
	@periodEnd nvarchar(20),
	@type varchar(50),
	@transferDate datetime
)
AS
BEGIN
	INSERT INTO [employer_financial].[AccountTransfers] 
	(
		SenderAccountId, SenderAccountName, ReceiverAccountId, ReceiverAccountName,
		ApprenticeshipId, CourseName,	
		PeriodEnd,
		Amount, 
		Type, 		
		CreatedDate,
		RequiredPaymentId
	)
	VALUES
	(
		@senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName,
		@apprenticeshipId, @courseName,		
		@periodEnd,
		@amount,
		@type,		
		@transferDate,
		NEWID()
	)
END;
GO

CREATE OR ALTER PROCEDURE DataGen.CreateAccountTransferTransaction
	@accountId bigint,
	@senderAccountId bigint,
	@senderAccountName nvarchar(100),
	@receiverAccountId bigint,
	@receiversAccountName nvarchar(100),
	@periodEnd nvarchar(20),
	@amount decimal(18,4),
	@createDate datetime
AS	
BEGIN
	--Create transfer sender transaction
	INSERT INTO [employer_financial].[TransactionLine]
	(
		AccountId
		,DateCreated 		
		,TransactionDate, TransactionType 
		,Amount 		
		,PeriodEnd 		
		,TransferSenderAccountId, TransferReceiverAccountId, TransferReceiverAccountName, TransferSenderAccountName
	)
	VALUES
	(
		@accountId,
		@createDate,
		@createDate, 4,
		@amount,
		@periodEnd,
		@senderAccountId, @receiverAccountId, @receiversAccountName, @senderAccountName
	)
END
GO

CREATE OR ALTER PROCEDURE DataGen.ProcessPaymentDataTransactionsGenerateDataEdition
	@AccountId bigint,
	@DateCreated datetime
AS

--- Process Levy Payments ---
INSERT INTO [employer_financial].[TransactionLine]
SELECT mainUpdate.* FROM
    (
    SELECT 
            x.AccountId AS AccountId,
			DATEFROMPARTS(DATEPART(yyyy,@DateCreated),DATEPART(MM,@DateCreated),DATEPART(dd,@DateCreated)) AS DateCreated,
            NULL AS SubmissionId,
            MAX(pe.CompletionDateTime) AS TransactionDate,
            3 AS TransactionType,			
            NULL AS LevyDeclared,
            SUM(ISNULL(p.Amount, 0)) * -1 Amount,
            NULL AS EmpRef,
            x.PeriodEnd,
            x.Ukprn,
            SUM(ISNULL(pco.Amount, 0)) * -1 AS SfaCoInvestmentAmount,
            SUM(ISNULL(pci.Amount, 0)) * -1 AS EmployerCoInvestmentAmount,
			0 AS EnglishFraction,
			NULL AS TransferSenderAccountId,
			NULL AS TransferSenderAccountName,
			NULL AS TransferReceiverAccountId,
			NULL AS TransferReceiverAccountName		
        FROM 
            employer_financial.[Payment] x
		INNER JOIN [employer_financial].[PeriodEnd] pe 
				ON pe.PeriodEndId = x.PeriodEnd
        LEFT JOIN [employer_financial].[Payment] p 
				ON p.PeriodEnd = pe.PeriodEndId AND p.PaymentId = x.PaymentId AND p.FundingSource IN (1, 5)
        LEFT JOIN [employer_financial].[Payment] pco 
				ON pco.PeriodEnd = pe.PeriodEndId AND pco.PaymentId = x.PaymentId AND pco.FundingSource = x.FundingSource AND pco.FundingSource = 2 
        LEFT JOIN [employer_financial].[Payment] pci 
				ON pci.PeriodEnd = pe.PeriodEndId AND pci.PaymentId = x.PaymentId AND pci.FundingSource = x.FundingSource AND pci.FundingSource = 3 
		WHERE x.AccountId = @AccountId
        GROUP BY
            x.Ukprn,x.PeriodEnd,x.AccountId
    ) mainUpdate
    INNER JOIN (
        SELECT AccountId,Ukprn,PeriodEnd FROM [employer_financial].Payment WHERE FundingSource IN (1,2,3,5)      
    EXCEPT
        SELECT AccountId,Ukprn,PeriodEnd FROM [employer_financial].[TransactionLine] WHERE TransactionType = 3
    ) dervx ON dervx.AccountId = mainUpdate.AccountId AND dervx.PeriodEnd = mainUpdate.PeriodEnd AND dervx.Ukprn = mainUpdate.Ukprn
GO

CREATE OR ALTER PROCEDURE DataGen.CreatePaymentsForMonth
(    	
    @accountId bigint,
    @accountName nvarchar(100),
	@createDate datetime,
	@totalPaymentAmount decimal(18,5),
	@numberOfPayments int
)  
AS  
BEGIN  
BEGIN TRANSACTION

    DECLARE @periodEndDate datetime = DATEADD(month, -1, @createDate)
	DECLARE @periodEndId varchar(8) = DataGen.PeriodEnd(@periodEndDate)
	DECLARE @ukprn bigint = 10001378
	DECLARE @apprenticeshipId bigint

	EXEC DataGen.CreatePeriodEnd @periodEndDate

    EXEC DataGen.CreateAccountPayments @accountId, @accountName, 'CHESTERFIELD COLLEGE', @ukprn, 'Accounting', /*Levy*/1, @periodEndDate, @totalPaymentAmount, @numberOfPayments,
										@firstApprenticeshipId = @apprenticeshipId OUTPUT

	-- #ProcessPaymentDataTransactionsGenerateDataEdition doesn't create a new payment transactionline where one already exists
	-- so we remove any current payment transactionline first, so that payments can be additively generated in a month
	DELETE [employer_financial].[TransactionLine] WHERE AccountId = @accountId AND Ukprn = @ukprn AND PeriodEnd = @periodEndId AND TransactionType = 3

    EXEC DataGen.ProcessPaymentDataTransactionsGenerateDataEdition @accountId, @createDate

COMMIT TRANSACTION
END
GO

--todo: eith merge CreatePaymentForMonth & createPaymentAndTransferForMonth
-- or get transfer version to call just payment version
CREATE OR ALTER PROCEDURE DataGen.CreatePaymentAndTransferForMonth
(    	
	@senderAccountId bigint,
	@senderAccountName nvarchar(100),
	@receiverAccountId bigint,
	@receiverAccountName nvarchar(100),
	@createDate datetime,
	@totalPaymentAmount decimal(18,5),
	@numberOfPayments int
)  
AS  
BEGIN  
BEGIN TRANSACTION

    DECLARE @periodEndDate datetime = DATEADD(month, -1, @createDate)
	DECLARE @periodEndId varchar(8) = DataGen.PeriodEnd(@periodEndDate)
	DECLARE @courseName nvarchar(max) = 'Plate Spinning'
	DECLARE @ukprn bigint = 10001378
	DECLARE @apprenticeshipId bigint

	EXEC DataGen.CreatePeriodEnd @periodEndDate

	--todo: we currently make sure that we always use an unique apprenticeshipid, so we don't fall over the unique index [IX_PeriodEndAccountTransfer]
	-- ^^ this might be good enough, but in reality, each month would have transfers for the same apprenticeshipid
	-- so it would be better to generate the first one outside of this sproc and reuse it for each month (once we generate >1 transfer at a time)
	-- or else have it as a user param (+ve can generate correct data, -ve requires manual work and greater understanding)
    EXEC DataGen.CreateAccountPayments @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', @ukprn, @courseName, /*LevyTransfer*/5, @periodEndDate, @totalPaymentAmount, @numberOfPayments,
									   @firstApprenticeshipId = @apprenticeshipId OUTPUT 

	--note: employer finance inserts the first apprenticeshipId from the set of transfers (and the others are ignored for the accounttransfer row)
	EXEC DataGen.CreateTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @apprenticeshipId, @courseName, @totalPaymentAmount, @periodEndId, 'Levy', @createDate

	DECLARE @negativeTotalPaymentAmount decimal(18,5) = -@totalPaymentAmount
	EXEC DataGen.CreateAccountTransferTransaction @senderAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @negativeTotalPaymentAmount, @createDate
	EXEC DataGen.CreateAccountTransferTransaction @receiverAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @totalPaymentAmount, @createDate

	--todo: check what these are doing. are they required? if so will need datageneration edition that accepts date rather than using getdate
	--exec employer_financial.processdeclarationstransactions @senderAccountId, @senderPayeScheme
	--exec employer_financial.processdeclarationstransactions @receiverAccountId, @receiverPayeScheme

	-- #ProcessPaymentDataTransactionsGenerateDataEdition doesn't create a new payment transactionline where one already exists
	-- so we remove any current payment transactionline first, so that payments can be additively generated in a month
	DELETE [employer_financial].[TransactionLine] WHERE AccountId = @receiverAccountId AND Ukprn = @ukprn AND PeriodEnd = @periodEndId AND TransactionType = 3

    EXEC DataGen.ProcessPaymentDataTransactionsGenerateDataEdition @receiverAccountId, @createDate

COMMIT TRANSACTION
END
GO

CREATE OR ALTER PROCEDURE DataGen.CreatePaymentsForMonths
(
	@accountId bigint,
    @accountName nvarchar(100),
	@source AS PaymentGenerationSourceTable READONLY
)
AS
BEGIN

	DECLARE @monthBeforeToDate int = 1
	DECLARE @createDate datetime
	DECLARE @amount decimal(18, 4)
	DECLARE @paymentsToGenerate int

	WHILE (1 = 1) 
	BEGIN  

	  SELECT TOP 1 @monthBeforeToDate = monthBeforeToDate, @createDate = createMonth, @amount = amount, @paymentsToGenerate = paymentsToGenerate
	  FROM @source
	  WHERE monthBeforeToDate < @monthBeforeToDate
	  ORDER BY monthBeforeToDate DESC

	  IF @@ROWCOUNT = 0 BREAK;

	  EXEC DataGen.CreatePaymentsForMonth @accountId, @accountName, @createDate, @amount, @paymentsToGenerate

	END

END
GO

CREATE OR ALTER PROCEDURE DataGen.CreateTransfersForMonths
(
	@senderAccountId bigint,
    @senderAccountName nvarchar(100),
	@receiverAccountId bigint,
    @receiverAccountName nvarchar(100),
	@source AS PaymentGenerationSourceTable READONLY
)
AS
BEGIN

	DECLARE @monthBeforeToDate int = 1
	DECLARE @createDate datetime
	DECLARE @amount decimal(18, 4)
	DECLARE @paymentsToGenerate int

	WHILE (1 = 1) 
	BEGIN  

	  SELECT TOP 1 @monthBeforeToDate = monthBeforeToDate, @createDate = createMonth, @amount = amount, @paymentsToGenerate = paymentsToGenerate
	  FROM @source
	  WHERE monthBeforeToDate < @monthBeforeToDate
	  ORDER BY monthBeforeToDate DESC

	  IF @@ROWCOUNT = 0 BREAK;

	  EXEC DataGen.CreatePaymentAndTransferForMonth	@senderAccountId,   @senderAccountName,
													@receiverAccountId, @receiverAccountName,
													@createDate, @amount, @paymentsToGenerate
	END

END
GO
