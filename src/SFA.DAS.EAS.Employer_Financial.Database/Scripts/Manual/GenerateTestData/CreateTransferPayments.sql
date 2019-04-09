-- See 'Transfer Generation Knobs' section towards the end of this file for the control knobs

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

IF OBJECT_ID('tempdb..#createPaymentAndTransferForMonth') IS NOT NULL
BEGIN
    DROP PROC #createPaymentAndTransferForMonth
END
GO

IF OBJECT_ID('tempdb..#ProcessPaymentDataTransactionsGenerateDataEdition') IS NOT NULL
BEGIN
    DROP PROC #ProcessPaymentDataTransactionsGenerateDataEdition
END
GO

IF OBJECT_ID('tempdb..#createAccountPayments') IS NOT NULL
BEGIN
    DROP PROC #createAccountPayments
END
GO

IF OBJECT_ID('tempdb..#createCoursePaymentsForAccount') IS NOT NULL
BEGIN
    DROP PROC #createCoursePaymentsForAccount
END
GO

IF OBJECT_ID('tempdb..#createTransfer') IS NOT NULL
BEGIN
    DROP PROC #createTransfer
END
GO

--IF OBJECT_ID('tempdb..#createAccountTransfers') IS NOT NULL
--BEGIN
--    DROP PROC #createAccountTransfers
--END
--GO

IF OBJECT_ID('tempdb..#CreateAccountTransferTransaction') IS NOT NULL
BEGIN
    DROP PROC #CreateAccountTransferTransaction
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
END;
GO

CREATE FUNCTION CollectionPeriodYear (@date datetime)  
RETURNS int
AS  
BEGIN  
  declare @collectionPeriodYear int = (select dbo.CalendarPeriodYear(@date))
  return @collectionPeriodYear
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

-- Add period end if its not already there
CREATE PROCEDURE #createPeriodEnd
(
	@periodEndDate DATETIME
)
AS
BEGIN
	DECLARE @periodEndId VARCHAR(8) = dbo.PeriodEnd(@periodEndDate)

	IF NOT EXISTS
	(
		select 1 FROM [employer_financial].[periodend] 
		WHERE periodendid = @periodEndId
	)
	BEGIN        
		insert employer_financial.periodend (periodendid, calendarperiodmonth, calendarperiodyear, accountdatavalidat, commitmentdatavalidat, completiondatetime, paymentsforperiod)
		values (@periodEndId, dbo.CalendarPeriodMonth(@periodEndDate), dbo.CalendarPeriodYear(@periodEndDate), '2018-05-04 00:00:00.000', '2018-05-04 09:07:34.457', '2018-05-04 10:50:27.760', 'https://pp-payments.apprenticeships.sfa.bis.gov.uk/api/payments?periodId=' + @periodEndId)
	END
END
GO

CREATE PROCEDURE #createAccountPayments
(    	
    @accountId BIGINT,
    @accountName NVARCHAR(100),
    @providerName NVARCHAR(MAX),
    @ukprn BIGINT,
    @courseName NVARCHAR(MAX),
	@fundingSource int,
	@periodEndDate DATETIME,
    @totalAmount DECIMAL(18,5),
	@numberOfPayments INT,
	@firstApprenticeshipId bigint output
)  
AS  
BEGIN  	

    DECLARE @paymentAmount DECIMAL(18,5) = @totalAmount / @numberOfPayments
	declare @fundingSourceString varchar(25) = cast(@fundingSource as VARCHAR(25))

	declare @name varchar(100)
	declare @uln bigint
	declare @apprenticeshipId bigint

	set @firstApprenticeshipId = (ISNULL((SELECT MAX(ApprenticeshipId) FROM employer_financial.Payment),0) + 1)

	while (@numberOfPayments > 0)
	BEGIN

	  set @numberOfPayments = @numberOfPayments - 1

	  SET @name = (CHAR(ASCII('A') + @numberOfPayments)) + ' Apprentice'
	  SET @uln = 1000000000 + @numberOfPayments
	  SET @apprenticeshipId = @firstApprenticeshipId + @numberOfPayments

      EXEC #createPayment @accountId, @providerName, @courseName, 1, @name, @ukprn, @uln, @apprenticeshipId, @fundingSourceString, @paymentAmount, @periodEndDate
	END
END
GO


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

	-- @deliveryPeriodDate approx 0-6 months before collection period
	--todo: needs to be in same ay? if so, get month, knock some off, don't go below 1, then convert back to date
	declare @deliveryPeriodDate datetime = DATEADD(month, -floor(rand()*6), @periodEndDate)
	-- evidencesubmittedon >= devliveryperiod (can also be > collectionperiod)
	--declare @evidenceSubmittedOn datetime = DATEADD(month, 1, @deliveryPeriodDate)

    INSERT INTO employer_financial.payment
    (paymentid, ukprn, uln, accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, 
	collectionperiodid, collectionperiodmonth, collectionperiodyear, 
	evidencesubmittedon, employeraccountversion, apprenticeshipversion, fundingsource, transactiontype, amount, periodend, paymentmetadataid)
    VALUES
    (newid(), @ukprn, @uln, @accountid, @apprenticeshipid, dbo.CalendarPeriodMonth(@deliveryPeriodDate), dbo.CalendarPeriodYear(@deliveryPeriodDate),
	dbo.PeriodEnd(@periodEndDate), dbo.CollectionPeriodMonth(@periodEndDate), dbo.CollectionPeriodYear(@periodEndDate), 
	'2018-06-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount, dbo.PeriodEnd(@periodEndDate), @paymentMetadataId)
END;  
GO 

CREATE PROCEDURE #createTransfer
(
	@senderAccountId bigint,
	@senderAccountName NVARCHAR(100),
	@receiverAccountId bigint,
	@receiverAccountName NVARCHAR(100),
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
		SenderAccountId, 
		SenderAccountName,
		ReceiverAccountId, 
		ReceiverAccountName,
		ApprenticeshipId, 
		CourseName,	
		PeriodEnd,
		Amount, 
		Type, 		
		CreatedDate,
		RequiredPaymentId
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
		NEWID()
	)
END;
GO

CREATE PROCEDURE #CreateAccountTransferTransaction
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
		,TransactionDate 
		,TransactionType 
		,Amount 		
		,PeriodEnd 		
		,TransferSenderAccountId
		,TransferReceiverAccountId
		,TransferReceiverAccountName		
		,TransferSenderAccountName
	)
	VALUES
	(
		@accountId,
		@createDate,
		@createDate,
		4,
		@amount,
		@periodEnd,
		@senderAccountId,
		@receiverAccountId,
		@receiversAccountName,
		@senderAccountName
	)
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
			DATEFROMPARTS(DatePart(yyyy,@DateCreated),DatePart(MM,@DateCreated),DATEPART(dd,@DateCreated)) as DateCreated,
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

CREATE PROCEDURE #createPaymentAndTransferForMonth
(    	
	@senderAccountId BIGINT,
	@senderAccountName NVARCHAR(100),
	--@senderPayeScheme NVARCHAR(16),
	@receiverAccountId BIGINT,
	@receiverAccountName NVARCHAR(100),
	--@receiverPayeScheme NVARCHAR(16),
	@createDate DATETIME,
	@totalPaymentAmount DECIMAL(18,5),
	@numberOfPayments INT
)  
AS  
BEGIN  
BEGIN TRANSACTION

    DECLARE @periodEndDate DATETIME = DATEADD(month, -1, @createDate)
	DECLARE @periodEndId VARCHAR(8) = dbo.PeriodEnd(@periodEndDate)
	declare @courseName nvarchar(max) = 'Plate Spinning'
	declare @ukprn bigint = 10001378
	declare @apprenticeshipId bigint

	exec #createPeriodEnd @periodEndDate

	--todo: we currently make sure that we always use an unique apprenticeshipid, so we don't fall over the unique index [IX_PeriodEndAccountTransfer]
	-- ^^ this might be good enough, but in reality, each month would have transfers for the same apprenticeshipid
	-- so it would be better to generate the first one outside of this sproc and reuse it for each month (once we generate >1 transfer at a time)
	-- or else have it as a user param (+ve can generate correct data, -ve requires manual work and greater understanding)
    EXEC #createAccountPayments @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', @ukprn, @courseName, /*LevyTransfer*/5, @periodEndDate, @totalPaymentAmount, @numberOfPayments,
								@firstApprenticeshipId = @apprenticeshipId output 

	--note: employer finance inserts the first apprenticeshipId from the set of transfers (and the others are ignored for the accounttransfer row)
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @apprenticeshipId, @courseName, @totalPaymentAmount, @periodEndId, 'Levy', @createDate

	declare @negativeTotalPaymentAmount DECIMAL(18,5) = -@totalPaymentAmount
	EXEC #CreateAccountTransferTransaction @senderAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @negativeTotalPaymentAmount, @createDate
	EXEC #CreateAccountTransferTransaction @receiverAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @totalPaymentAmount, @createDate

	--todo: check what these are doing. are they required? if so will need datageneration edition that accepts date rather than using getdate
	--exec employer_financial.processdeclarationstransactions @senderAccountId, @senderPayeScheme
	--exec employer_financial.processdeclarationstransactions @receiverAccountId, @receiverPayeScheme

	-- #ProcessPaymentDataTransactionsGenerateDataEdition doesn't create a new payment transactionline where one already exists
	-- so we remove any current payment transactionline first, so that payments can be additively generated in a month
	delete [employer_financial].[TransactionLine] where AccountId = @receiverAccountId and Ukprn = @ukprn and PeriodEnd = @periodEndId and TransactionType = 3

    exec #ProcessPaymentDataTransactionsGenerateDataEdition @receiverAccountId, @createDate

COMMIT TRANSACTION
END
GO

	--  ,--.--------.              ,---.      .-._          ,-,--.     _,---.     ,----.                           _,---.      ,----.  .-._           ,----.                ,---.   ,--.--------.  .=-.-.  _,.---._    .-._                ,--.-.,-.  .-._          _,.---._                 ,-,--. 
	-- /==/,  -   , -\.-.,.---.  .--.'  \    /==/ \  .-._ ,-.'-  _\ .-`.' ,  \ ,-.--` , \  .-.,.---.           _.='.'-,  \  ,-.--` , \/==/ \  .-._ ,-.--` , \  .-.,.---.  .--.'  \ /==/,  -   , -\/==/_ /,-.' , -  `. /==/ \  .-._        /==/- |\  \/==/ \  .-._ ,-.' , -  `.    _..---.  ,-.'-  _\
	-- \==\.-.  - ,-./==/  `   \ \==\-/\ \   |==|, \/ /, /==/_ ,_.'/==/_  _.-'|==|-  _.-` /==/  `   \         /==.'-     / |==|-  _.-`|==|, \/ /, /==|-  _.-` /==/  `   \ \==\-/\ \\==\.-.  - ,-./==|, |/==/_,  ,  - \|==|, \/ /, /       |==|_ `/_ /|==|, \/ /, /==/_,  ,  - \ .' .'.-. \/==/_ ,_.'
	--  `--`\==\- \ |==|-, .=., |/==/-|_\ |  |==|-  \|  |\==\  \  /==/-  '..-.|==|   `.-.|==|-, .=., |       /==/ -   .-'  |==|   `.-.|==|-  \|  ||==|   `.-.|==|-, .=., |/==/-|_\ |`--`\==\- \  |==|  |==|   .=.     |==|-  \|  |        |==| ,   / |==|-  \|  |==|   .=.     /==/- '=' /\==\  \   
	--       \==\_ \|==|   '='  /\==\,   - \ |==| ,  | -| \==\ -\ |==|_ ,    /==/_ ,    /|==|   '='  /       |==|_   /_,-./==/_ ,    /|==| ,  | -/==/_ ,    /|==|   '='  /\==\,   - \    \==\_ \ |==|- |==|_ : ;=:  - |==| ,  | -|        |==|-  .|  |==| ,  | -|==|_ : ;=:  - |==|-,   '  \==\ -\  
	--       |==|- ||==|- ,   .' /==/ -   ,| |==| -   _ | _\==\ ,\|==|   .--'|==|    .-' |==|- ,   .'        |==|  , \_.' )==|    .-' |==| -   _ |==|    .-' |==|- ,   .' /==/ -   ,|    |==|- | |==| ,|==| , '='     |==| -   _ |        |==| _ , \ |==| -   _ |==| , '='     |==|  .=. \ _\==\ ,\ 
	--       |==|, ||==|_  . ,'./==/-  /\ - \|==|  /\ , |/==/\/ _ |==|-  |   |==|_  ,`-._|==|_  . ,'.        \==\-  ,    (|==|_  ,`-._|==|  /\ , |==|_  ,`-._|==|_  . ,'./==/-  /\ - \   |==|, | |==|- |\==\ -    ,_ /|==|  /\ , |        /==/  '\  ||==|  /\ , |\==\ -    ,_ //==/- '=' ,/==/\/ _ |
	--       /==/ -//==/  /\ ,  )==\ _.\=\.-'/==/, | |- |\==\ - , /==/   \   /==/ ,     //==/  /\ ,  )        /==/ _  ,  //==/ ,     //==/, | |- /==/ ,     //==/  /\ ,  )==\ _.\=\.-'   /==/ -/ /==/. / '.='. -   .' /==/, | |- |        \==\ /\=\.'/==/, | |- | '.='. -   .'|==|   -   /\==\ - , /
	--       `--`--``--`-`--`--' `--`        `--`./  `--` `--`---'`--`---'   `--`-----`` `--`-`--`--'         `--`------' `--`-----`` `--`./  `--`--`-----`` `--`-`--`--' `--`           `--`--` `--`-`    `--`--''   `--`./  `--`         `--`      `--`./  `--`   `--`--''  `-._`.___,'  `--`---' 

	DECLARE @senderAccountId BIGINT               = 0
	DECLARE @SenderAccountName NVARCHAR(100)      = 'Sender Name'
	--DECLARE @senderPayeScheme NVARCHAR(16)      = '123/SE12345'

    DECLARE @receiverAccountId BIGINT             = 1
	DECLARE @receiverAccountName NVARCHAR(100)    = 'Receiver Name'
	--DECLARE @receiverPayeScheme NVARCHAR(16)    = '123/RE12345'
	
    DECLARE @toDate DATETIME                      = GETDATE()
	declare @numberOfMonthsToCreate INT           = 25
	declare @defaultMonthlyTransfer DECIMAL(18,4) = 100

	declare @defaultNumberOfPaymentsPerMonth INT              = 1
	
	--  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,
	-- '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P' 

	DECLARE @paymentsByMonth TABLE (monthBeforeToDate INT, amount DECIMAL(18, 4), paymentsToGenerate INT, createMonth DATETIME)

-- generate defaults
insert into @paymentsByMonth
SELECT TOP (@numberOfMonthsToCreate)
			monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
			@defaultMonthlyTransfer,
			@defaultNumberOfPaymentsPerMonth,
			DATEADD(month,/*monthBeforeToDate*/ -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)
FROM sys.all_objects
ORDER BY monthBeforeToDate;

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -7

select * from @paymentsByMonth

--todo: sproc that takes gen source table and generates payments

DECLARE @monthBeforeToDate INT = 1
DECLARE @createDate DATETIME
DECLARE @amount DECIMAL(18, 4)
DECLARE @paymentsToGenerate INT

WHILE (1 = 1) 
BEGIN  

  SELECT TOP 1 @monthBeforeToDate = monthBeforeToDate, @createDate = createMonth, @amount = amount, @paymentsToGenerate = paymentsToGenerate
  FROM @paymentsByMonth
  WHERE monthBeforeToDate < @monthBeforeToDate
  ORDER BY monthBeforeToDate DESC

  IF @@ROWCOUNT = 0 BREAK;

	exec #createPaymentAndTransferForMonth	@senderAccountId,   @senderAccountName,   -- @senderPayeScheme,
											@receiverAccountId, @receiverAccountName, -- @receiverPayeScheme,
											@createDate, @amount, @paymentsToGenerate
END

drop function CalendarPeriodYear
go
drop function CalendarPeriodMonth
go
drop function CollectionPeriodYear
go
drop function CollectionPeriodMonth
go
drop function PeriodEndYear
go
drop function PeriodEndMonth
go
drop function PeriodEnd
go