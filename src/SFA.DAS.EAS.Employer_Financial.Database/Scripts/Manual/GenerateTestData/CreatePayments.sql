--todo: separate script/sproc to add period end (levy and payment scripts can add add as appropriate)
-- make period end/dates align for payment gen
-- refunds
-- number of months to generate
-- fix rounding
-- transfers

-- DELETE THE TEMP STORED PROCEDURES IF THEY EXIST
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

CREATE FUNCTION CalendarPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)
  RETURN(@month);  
END; 
GO

CREATE FUNCTION CalendarPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  declare @year int = DATEPART(year,@date)
  return @year
END; 
GO

CREATE FUNCTION CollectionPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  declare @collectionPeriodMonth int = (select dbo.CalendarPeriodMonth(@date))
  return @collectionPeriodMonth

 -- declare @month int = DATEPART(month,@date)

 -- SET @month = @month - 7
 -- IF @month < 1
	--SET @month = @month + 12

 -- RETURN(@month);  
END; 
GO

CREATE FUNCTION CollectionPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  declare @collectionPeriodYear int = (select dbo.CalendarPeriodYear(@date))
  return @collectionPeriodYear

  --declare @year int = DATEPART(year,@date)
  --return @year
END; 
GO

--todo: approx 0-6 months before collection period
CREATE FUNCTION DeliveryPeriodMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  declare @deliveryPeriodMonth int = (select dbo.CalendarPeriodMonth(@date))
  return @deliveryPeriodMonth
END; 
GO

CREATE FUNCTION DeliveryPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  declare @deliveryPeriodYear int = (select dbo.CalendarPeriodYear(@date))
  return @deliveryPeriodYear
END; 
GO


--todo R13, R14
CREATE FUNCTION PeriodEndMonth (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)

  SET @month = @month - 7
  IF @month < 1
	SET @month = @month + 12

  declare @periodEndMonth VARCHAR(5) = CONVERT(varchar(5), @month)
  RETURN @periodEndMonth; 
END; 
GO

--todo R13, R14
CREATE FUNCTION PeriodEndYear (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)
  declare @year int = DATEPART(year,@date)

  if @month < 8
	SET @year = @year - 1
	
  DECLARE @periodEndYear VARCHAR(4) = (SELECT RIGHT(CONVERT(VARCHAR(5), @year, 1), 2)) + (SELECT RIGHT(CONVERT(VARCHAR(4), @year+1, 1), 2))
  return @periodEndYear
END; 
GO

CREATE FUNCTION PeriodEnd (@date datetime)  
RETURNS VARCHAR(8)
AS  
BEGIN  
  declare @periodEnd varchar(8) = (SELECT dbo.PeriodEndYear(@date) + '-R' + right('0' + dbo.PeriodEndMonth(@date),2))
  return @periodEnd
END; 
GO

CREATE FUNCTION CollectionPeriodId (@date datetime)  
RETURNS VARCHAR(8)
AS  
BEGIN  
  declare @periodEnd varchar(8) = (select dbo.PeriodEnd(@date))
  return @periodEnd
END; 
GO

-- CREATE THE STORED PROCEDURES
CREATE PROCEDURE #createPayment
(     
    @accountId BIGINT,
    @providerName NVARCHAR(MAX),
    @apprenticeshipCourseName NVARCHAR(MAX),   
    @apprenticeshipCourseLevel INT,
    @apprenticeName VARCHAR(MAX), 	
    @ukprn BIGINT,
    @uln BIGINT,
    @apprenticeshipid BIGINT,
    @fundingSource INT,	
    @Amount DECIMAL(18,5),
    @periodEndDate DATETIME  
)  
AS  
BEGIN  
    DECLARE @paymentMetadataId bigint

    INSERT INTO employer_financial.paymentmetadata 
    (ProviderName, StandardCode, FrameworkCode, ProgrammeType, PathwayCode, ApprenticeshipCourseName, ApprenticeshipCourseStartDate, ApprenticeshipCourseLevel, ApprenticeName, ApprenticeNINumber)
    VALUES
    (@providerName,4,null,null,null, @apprenticeshipCourseName,'01/06/2018', @apprenticeshipCourseLevel, @apprenticeName, null)

    SELECT @paymentMetadataId  = SCOPE_IDENTITY()

	--todo: @deliveryPeriodDate approx 0-6 months before collection period
	--declare @deliveryPeriodDate datetime = DATEADD(month, -2, @periodEndDate)
	-- evidencesubmittedon >= devliveryperiod (can also be > collectionperiod)
	--declare @evidenceSubmittedOn datetime = DATEADD(month, 1, @deliveryPeriodDate)

    INSERT INTO employer_financial.payment
    (paymentid, ukprn,uln,accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, 
	collectionperiodid, collectionperiodmonth, collectionperiodyear, 
	evidencesubmittedon, employeraccountversion,apprenticeshipversion, fundingsource, transactiontype,amount,periodend,paymentmetadataid)
    VALUES
    (newid(), @ukprn, @uln, @accountid, @apprenticeshipid, dbo.DeliveryPeriodMonth(@periodEndDate), dbo.DeliveryPeriodYear(@periodEndDate), 
	dbo.PeriodEnd(@periodEndDate), dbo.CollectionPeriodMonth(@periodEndDate), dbo.CollectionPeriodYear(@periodEndDate), 
	'2018-06-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount, dbo.PeriodEnd(@periodEndDate), @paymentMetadataId)
END;  
GO  

CREATE PROCEDURE #createAccountPayments
(    	
    @accountId BIGINT,
    @accountName NVARCHAR(100),
    @providerName NVARCHAR(MAX),
    @ukprn BIGINT,
    @courseName NVARCHAR(MAX),
    --@periodEnd NVARCHAR(25),
	@periodEndDate DATETIME,
    @totalAmount DECIMAL(18,5)
)  
AS  
BEGIN  	

    DECLARE @paymentAmount DECIMAL(18,5) = @totalAmount / 3

    -- Create transfer payments
    EXEC #createPayment @accountId, @providerName, @courseName, 1, 'Mark Redwood', @ukprn, 1003, 3333, 1, @paymentAmount, @periodEndDate
    EXEC #createPayment @accountId, @providerName, @courseName, 1, 'Sarah Redwood', @ukprn, 2003, 7777, 1, @paymentAmount, @periodEndDate
    EXEC #createPayment @accountId, @providerName, @courseName, 1, 'Tim Woods', @ukprn, 2004, 8888, 1, @paymentAmount, @periodEndDate
END
GO

CREATE PROCEDURE #ProcessPaymentDataTransactionsGenerateDataEdition
	@AccountId BIGINT,
	@DateCreated DATETIME
AS

--- Process Levy Payments ---
INSERT INTO [employer_financial].[TransactionLine]
select mainUpdate.* from
    (
    select 
            x.AccountId as AccountId,
            @DateCreated as DateCreated,
            null as SubmissionId,
            Max(pe.CompletionDateTime) as TransactionDate,
            3 as TransactionType,			
            null as LevyDeclared,
            Sum(ISNULL(p.Amount, 0)) * -1 Amount,
            null as EmpRef,
            x.PeriodEnd,
            x.Ukprn,
            Sum(ISNULL(pco.Amount, 0)) * -1 as SfaCoInvestmentAmount,
            Sum(ISNULL(pci.Amount, 0)) * -1 as EmployerCoInvestmentAmount,
			0 as EnglishFraction,
			null as TransferSenderAccountId,
			null as TransferSenderAccountName,
			null as TransferReceiverAccountId,
			null as TransferReceiverAccountName		
        FROM 
            employer_financial.[Payment] x
		inner join [employer_financial].[PeriodEnd] pe 
				on pe.PeriodEndId = x.PeriodEnd
        left join [employer_financial].[Payment] p 
				on p.PeriodEnd = pe.PeriodEndId and p.PaymentId = x.PaymentId and p.FundingSource IN (1, 5)
        left join [employer_financial].[Payment] pco 
				on pco.PeriodEnd = pe.PeriodEndId and pco.PaymentId = x.PaymentId and pco.FundingSource = x.FundingSource and pco.FundingSource = 2 
        left join [employer_financial].[Payment] pci 
				on pci.PeriodEnd = pe.PeriodEndId and pci.PaymentId = x.PaymentId  and pci.FundingSource = x.FundingSource and pci.FundingSource = 3 
		WHERE x.AccountId = @AccountId
        Group by
            x.Ukprn,x.PeriodEnd,x.AccountId
    ) mainUpdate
    inner join (
        select AccountId,Ukprn,PeriodEnd from [employer_financial].Payment where FundingSource IN (1,2,3,5)      
    EXCEPT
        select AccountId,Ukprn,PeriodEnd from [employer_financial].[TransactionLine] where TransactionType = 3
    ) dervx on dervx.AccountId = mainUpdate.AccountId and dervx.PeriodEnd = mainUpdate.PeriodEnd and dervx.Ukprn = mainUpdate.Ukprn
GO

BEGIN TRANSACTION

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

    DECLARE @accountId BIGINT                 = 0
    DECLARE @accountName NVARCHAR(100)        = 'XXX'	
    DECLARE @payeScheme NVARCHAR(16)          = 'XXX'
    
    DECLARE @createDate DATETIME              = GETDATE()
    DECLARE @periodEndDate DATETIME           = DATEADD(month, -1, @createDate)
    DECLARE @totalPaymentAmount DECIMAL(18,5) = 10000
    
	--  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____  _____ 
	-- [_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____][_____]

	--SELECT dbo.CalendarPeriodYear(@periodEndDate)
	--SELECT dbo.CalendarPeriodMonth(@periodEndDate)
	--SELECT dbo.CollectionPeriodYear(@periodEndDate)
	--SELECT dbo.CollectionPeriodMonth(@periodEndDate)
	--SELECT dbo.PeriodEndYear(@periodEndDate)
	--SELECT dbo.PeriodEndMonth(@periodEndDate)

	DECLARE @periodEndId VARCHAR(8) = dbo.PeriodEnd(@periodEndDate)
	--SELECT @periodEndId

	-- Add period end if its not already there
	IF NOT EXISTS
	(
		select 1 FROM [employer_financial].[periodend] 
		WHERE periodendid = @periodEndId
	)
	BEGIN        
		insert into employer_financial.periodend (periodendid, calendarperiodmonth, calendarperiodyear, accountdatavalidat, commitmentdatavalidat, completiondatetime, paymentsforperiod)
		values
		(@periodEndId,dbo.CalendarPeriodMonth(@periodEndDate),dbo.CalendarPeriodYear(@periodEndDate),'2018-05-04 00:00:00.000','2018-05-04 09:07:34.457','2018-05-04 10:50:27.760','https://pp-payments.apprenticeships.sfa.bis.gov.uk/api/payments?periodId=1617-R10')
	END

    EXEC #createAccountPayments @accountId, @accountName, 'CHESTERFIELD COLLEGE', 10001378, 'Accounting', @periodEndDate, @totalPaymentAmount	
        
    exec #ProcessPaymentDataTransactionsGenerateDataEdition @accountId, @createDate

COMMIT TRANSACTION
GO

drop function CalendarPeriodYear
go
drop function CalendarPeriodMonth
go
drop function CollectionPeriodYear
go
drop function CollectionPeriodMonth
go
drop function CollectionPeriodId
go
drop function DeliveryPeriodYear
go
drop function DeliveryPeriodMonth
go
drop function PeriodEndYear
go
drop function PeriodEndMonth
go
drop function PeriodEnd
go
