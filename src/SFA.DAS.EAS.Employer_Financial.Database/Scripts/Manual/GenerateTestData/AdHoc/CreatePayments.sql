-- Instructions: Find clearly signposted 'Payment Generation Knobs' section below to change generation variable defaults

IF OBJECT_ID('tempdb..#createPeriodEnd') IS NOT NULL
BEGIN
    DROP PROC #createPeriodEnd
END
GO

IF OBJECT_ID('tempdb..#createPayment') IS NOT NULL
BEGIN
    DROP PROC #createPayment
END
GO

IF OBJECT_ID('tempdb..#createCoursePaymentsForAccount') IS NOT NULL
BEGIN
    DROP PROC #createCoursePaymentsForAccount
END
GO

IF OBJECT_ID('tempdb..#createAccountPayments') IS NOT NULL
BEGIN
    DROP PROC #createAccountPayments
END
GO

IF OBJECT_ID('tempdb..#ProcessPaymentDataTransactionsGenerateDataEdition') IS NOT NULL
BEGIN
    DROP PROC #ProcessPaymentDataTransactionsGenerateDataEdition
END
GO

IF OBJECT_ID('tempdb..#createPaymentsForMonth') IS NOT NULL
BEGIN
    DROP PROC #createPaymentsForMonth
END
GO

CREATE FUNCTION CalendarPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)
  RETURN(@month);  
END; 
GO

CREATE FUNCTION CalendarPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  DECLARE @year int = DATEPART(year,@date)
  RETURN @year
END; 
GO

CREATE FUNCTION CollectionPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  DECLARE @collectionPeriodMonth int = (SELECT dbo.CalendarPeriodMonth(@date))
  RETURN @collectionPeriodMonth
END; 
GO

CREATE FUNCTION CollectionPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  DECLARE @collectionPeriodYear int = (SELECT dbo.CalendarPeriodYear(@date))
  RETURN @collectionPeriodYear
END; 
GO

--todo R13, R14
CREATE FUNCTION PeriodEndMonth (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)

  SET @month = @month - 7
  IF @month < 1
	SET @month = @month + 12

  DECLARE @periodEndMonth VARCHAR(5) = CONVERT(varchar(5), @month)
  RETURN @periodEndMonth; 
END; 
GO

--todo R13, R14
CREATE FUNCTION PeriodEndYear (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)
  DECLARE @year int = DATEPART(year,@date)

  if @month < 8
	SET @year = @year - 1
	
  DECLARE @periodEndYear VARCHAR(4) = (SELECT RIGHT(CONVERT(VARCHAR(5), @year, 1), 2)) + (SELECT RIGHT(CONVERT(VARCHAR(4), @year+1, 1), 2))
  RETURN @periodEndYear
END; 
GO

CREATE FUNCTION PeriodEnd (@date datetime)  
RETURNS VARCHAR(8)
AS  
BEGIN  
  DECLARE @periodEnd varchar(8) = (SELECT dbo.PeriodEndYear(@date) + '-R' + RIGHT('0' + dbo.PeriodEndMonth(@date),2))
  RETURN @periodEnd
END; 
GO

--CREATE FUNCTION CollectionPeriodId (@date datetime)  
--RETURNS VARCHAR(8)
--AS  
--BEGIN  
--  DECLARE @periodEnd varchar(8) = (SELECT dbo.PeriodEnd(@date))
--  RETURN @periodEnd
--END; 
--GO

-- Add period end if its not already there
CREATE PROCEDURE #createPeriodEnd
(
	@periodEndDate datetime
)
AS
BEGIN
	DECLARE @periodEndId VARCHAR(8) = dbo.PeriodEnd(@periodEndDate)

	IF NOT EXISTS
	(
		SELECT 1 FROM [employer_financial].[periodend] 
		WHERE periodendid = @periodEndId
	)
	BEGIN        
		INSERT employer_financial.periodend (periodendid, calendarperiodmonth, calendarperiodyear, accountdatavalidat, commitmentdatavalidat, completiondatetime, paymentsforperiod)
		VALUES (@periodEndId, dbo.CalendarPeriodMonth(@periodEndDate), dbo.CalendarPeriodYear(@periodEndDate), '2018-05-04 00:00:00.000', '2018-05-04 09:07:34.457', '2018-05-04 10:50:27.760', 'https://pp-payments.apprenticeships.sfa.bis.gov.uk/api/payments?periodId=' + @periodEndId)
	END
END
GO

CREATE PROCEDURE #createPayment
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
	DECLARE @deliveryPeriodDate datetime = DATEADD(month, -FLOOR(RAND()*6), @periodEndDate)
	-- evidencesubmittedon >= devliveryperiod (can also be > collectionperiod)
	--DECLARE @evidenceSubmittedOn datetime = DATEADD(month, 1, @deliveryPeriodDate)

    INSERT INTO employer_financial.payment
    (paymentid, ukprn,uln,accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, 
	collectionperiodid, collectionperiodmonth, collectionperiodyear, 
	evidencesubmittedon, employeraccountversion,apprenticeshipversion, fundingsource, transactiontype,amount,periodend,paymentmetadataid)
    VALUES
    (NEWID(), @ukprn, @uln, @accountid, @apprenticeshipid, dbo.CalendarPeriodMonth(@deliveryPeriodDate), dbo.CalendarPeriodYear(@deliveryPeriodDate),
	dbo.PeriodEnd(@periodEndDate), dbo.CollectionPeriodMonth(@periodEndDate), dbo.CollectionPeriodYear(@periodEndDate), 
	'2018-06-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount, dbo.PeriodEnd(@periodEndDate), @paymentMetadataId)
END;  
GO  

CREATE PROCEDURE #createAccountPayments
(    	
    @accountId bigint,
    @accountName nvarchar(100),
    @providerName nvarchar(max),
    @ukprn bigint,
    @courseName nvarchar(max),
	@periodEndDate datetime,
    @totalAmount decimal(18,5),
	@numberOfPayments int
)  
AS  
BEGIN  	

    DECLARE @paymentAmount DECIMAL(18,5) = @totalAmount / @numberOfPayments

	DECLARE @name varchar(100)
	DECLARE @uln bigint
	DECLARE @id bigint

	WHILE (@numberOfPayments > 0)
	BEGIN

	  SET @numberOfPayments = @numberOfPayments - 1

	  SET @name = (CHAR(ASCII('A') + @numberOfPayments)) + ' Apprentice'
	  SET @uln = 1000000000 + @numberOfPayments
	  SET @id = 1000 + @numberOfPayments

      EXEC #createPayment @accountId, @providerName, @courseName, 1, @name, @ukprn, @uln, @id, /*Levy*/1, @paymentAmount, @periodEndDate
	END
END
GO

CREATE PROCEDURE #ProcessPaymentDataTransactionsGenerateDataEdition
	@AccountId bigint,
	@DateCreated datetime
AS

--- Process Levy Payments ---
INSERT INTO [employer_financial].[TransactionLine]
SELECT mainUpdate.* FROM
    (
    SELECT 
            x.AccountId AS AccountId,
			DATEFROMPARTS(DatePart(yyyy,@DateCreated),DATEPART(MM,@DateCreated),DATEPART(dd,@DateCreated)) as DateCreated,
            NULL AS SubmissionId,
            MAX(pe.CompletionDateTime) AS TransactionDate,
            3 AS TransactionType,			
            null AS LevyDeclared,
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

CREATE PROCEDURE #createPaymentsForMonth
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
	DECLARE @periodEndId varchar(8) = dbo.PeriodEnd(@periodEndDate)
	DECLARE @ukprn bigint = 10001378

	exec #createPeriodEnd @periodEndDate

    EXEC #createAccountPayments @accountId, @accountName, 'CHESTERFIELD COLLEGE', @ukprn, 'Accounting', @periodEndDate, @totalPaymentAmount, @numberOfPayments

	-- #ProcessPaymentDataTransactionsGenerateDataEdition doesn't create a new payment transactionline where one already exists
	-- so we remove any current payment transactionline first, so that payments can be additively generated in a month
	DELETE [employer_financial].[TransactionLine] WHERE AccountId = @accountId AND Ukprn = @ukprn AND PeriodEnd = @periodEndId AND TransactionType = 3

    EXEC #ProcessPaymentDataTransactionsGenerateDataEdition @accountId, @createDate

COMMIT TRANSACTION
END
GO

	-- ________                                                                         ____                                                                                  ___    __                    ___               
	-- `MMMMMMMb.                                                                      6MMMMb/                                                   68b                          `MM    d'  @                   MM              
	--  MM    `Mb                                                        /            8P    YM                                             /     Y89                           MM   d'                      MM               
	--  MM     MM    ___  ____    ___ ___  __    __     ____  ___  __   /M           6M      Y   ____  ___  __     ____  ___  __    ___   /M     ___   _____  ___  __          MM  d'   ___  __     _____   MM____     ____  
	--  MM     MM  6MMMMb `MM(    )M' `MM 6MMb  6MMb   6MMMMb `MM 6MMb /MMMMM        MM         6MMMMb `MM 6MMb   6MMMMb `MM 6MM  6MMMMb /MMMMM  `MM  6MMMMMb `MM 6MMb         MM d'    `MM 6MMb   6MMMMMb  MMMMMMb   6MMMMb\
	--  MM    .M9 8M'  `Mb `Mb    d'   MM69 `MM69 `Mb 6M'  `Mb MMM9 `Mb MM           MM        6M'  `Mb MMM9 `Mb 6M'  `Mb MM69 " 8M'  `Mb MM      MM 6M'   `Mb MMM9 `Mb        MMd'      MMM9 `Mb 6M'   `Mb MM'  `Mb MM'    `
	--  MMMMMMM9'     ,oMM  YM.  ,P    MM'   MM'   MM MM    MM MM'   MM MM           MM     ___MM    MM MM'   MM MM    MM MM'        ,oMM MM      MM MM     MM MM'   MM        MMYM.     MM'   MM MM     MM MM    MM YM.     
	--  MM        ,6MM9'MM   MM  M     MM    MM    MM MMMMMMMM MM    MM MM           MM     `M'MMMMMMMM MM    MM MMMMMMMM MM     ,6MM9'MM MM      MM MM     MM MM    MM        MM YM.    MM    MM MM     MM MM    MM  YMMMMb 
	--  MM        MM'   MM   `Mbd'     MM    MM    MM MM       MM    MM MM           YM      M MM       MM    MM MM       MM     MM'   MM MM      MM MM     MM MM    MM        MM  YM.   MM    MM MM     MM MM    MM      `Mb
	--  MM        MM.  ,MM    YMP      MM    MM    MM YM    d9 MM    MM YM.  ,        8b    d9 YM    d9 MM    MM YM    d9 MM     MM.  ,MM YM.  ,  MM YM.   ,M9 MM    MM        MM   YM.  MM    MM YM.   ,M9 MM.  ,M9 L    ,MM
	-- _MM_       `YMMM9'Yb.   M      _MM_  _MM_  _MM_ YMMMM9 _MM_  _MM_ YMMM9         YMMMM9   YMMMM9 _MM_  _MM_ YMMMM9 _MM_    `YMMM9'Yb.YMMM9 _MM_ YMMMMM9 _MM_  _MM_      _MM_   YM._MM_  _MM_ YMMMMM9 _MYMMMM9  MYMMMM9 
	--                        d'                                                                                                                                                                                             
	--                    (8),P                                                                                                                                                                                              
	--                     YMM                                                                                                                                                                                               

	DECLARE @accountId bigint                          = 1
    DECLARE @accountName nvarchar(100)                 = 'Insert Name Here'
	DECLARE @toDate datetime                           = GETDATE()
	DECLARE @numberOfMonthsToCreate int                = 25
	DECLARE @defaultMonthlyTotalPayments decimal(18,5) = 100
	DECLARE @defaultPaymentsPerMonth int               = 3

	--  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____ 
	-- [_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____]

--todo: sproc to generate gen source table

DECLARE @paymentsByMonth TABLE (monthBeforeToDate int, amount decimal(18, 4), paymentsToGenerate int, createMonth datetime)

-- generate defaults
INSERT INTO @paymentsByMonth
SELECT TOP (@numberOfMonthsToCreate)
			monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
			@defaultMonthlyTotalPayments,
			@defaultPaymentsPerMonth,
			DATEADD(month,/*monthBeforeToDate*/ -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)
FROM sys.all_objects
ORDER BY monthBeforeToDate;

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 WHERE monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 WHERE monthBeforeToDate = -7

SELECT * FROM @paymentsByMonth

--todo: sproc that takes gen source table and generates payments

DECLARE @monthBeforeToDate int = 1
DECLARE @createDate datetime
DECLARE @amount decimal(18, 4)
DECLARE @paymentsToGenerate int

WHILE (1 = 1) 
BEGIN  

  SELECT TOP 1 @monthBeforeToDate = monthBeforeToDate, @createDate = createMonth, @amount = amount, @paymentsToGenerate = paymentsToGenerate
  FROM @paymentsByMonth
  WHERE monthBeforeToDate < @monthBeforeToDate
  ORDER BY monthBeforeToDate DESC

  IF @@ROWCOUNT = 0 BREAK;

  EXEC #createPaymentsForMonth @accountId, @accountName, @createDate, @amount, @paymentsToGenerate

END

DROP FUNCTION CalendarPeriodYear
GO
DROP FUNCTION CalendarPeriodMonth
GO
DROP FUNCTION CollectionPeriodYear
GO
DROP FUNCTION CollectionPeriodMonth
GO
--DROP FUNCTION CollectionPeriodId
--GO
DROP FUNCTION PeriodEndYear
GO
DROP FUNCTION PeriodEndMonth
GO
DROP FUNCTION PeriodEnd
GO
