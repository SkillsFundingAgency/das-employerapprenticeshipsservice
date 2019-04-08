--todo: generate correct period end for payment

-- DELETE THE TEMP STORED PROCEDURES IF THEY EXIST

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
	@numberOfPayments INT
)  
AS  
BEGIN  	

    DECLARE @paymentAmount DECIMAL(18,5) = @totalAmount / @numberOfPayments
	declare @fundingSourceString varchar(25) = cast(@fundingSource as VARCHAR(25))

	declare @name varchar(100)
	declare @uln bigint
	declare @id bigint

	while (@numberOfPayments > 0)
	BEGIN

	  set @numberOfPayments = @numberOfPayments - 1

	  SET @name = (CHAR(ASCII('A') + @numberOfPayments)) + ' Apprentice'
	  SET @uln = 1000000000 + @numberOfPayments
	  SET @id = 1000 + @numberOfPayments

      EXEC #createPayment @accountId, @providerName, @courseName, 1, @name, @ukprn, @uln, @id, @fundingSourceString, @paymentAmount, @periodEndDate
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
    (paymentid, ukprn,uln,accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, 
	collectionperiodid, collectionperiodmonth, collectionperiodyear, 
	evidencesubmittedon, employeraccountversion,apprenticeshipversion, fundingsource, transactiontype,amount,periodend,paymentmetadataid)
    VALUES
    (newid(), @ukprn, @uln, @accountid, @apprenticeshipid, dbo.CalendarPeriodMonth(@deliveryPeriodDate), dbo.CalendarPeriodYear(@deliveryPeriodDate),
	dbo.PeriodEnd(@periodEndDate), dbo.CollectionPeriodMonth(@periodEndDate), dbo.CollectionPeriodYear(@periodEndDate), 
	'2018-06-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount, dbo.PeriodEnd(@periodEndDate), @paymentMetadataId)
END;  
GO 

--CREATE PROCEDURE #createPayment
--(     
--	@accountId BIGINT,
--    @providerName NVARCHAR(MAX),
--    @apprenticeshipCourseName NVARCHAR(MAX),   
--	@apprenticeshipCourseLevel INT,
--	@apprenticeName VARCHAR(MAX), 	
--	@ukprn BIGINT,
--	@uln BIGINT,
--	@apprenticeshipid BIGINT,
--	@fundingSource INT,	
--	@Amount DECIMAL  
--)  
--AS  
--BEGIN  
--	DECLARE @paymentMetadataId bigint

--	INSERT INTO employer_financial.paymentmetadata 
--	(ProviderName, StandardCode, FrameworkCode, ProgrammeType, PathwayCode, ApprenticeshipCourseName, ApprenticeshipCourseStartDate, ApprenticeshipCourseLevel, ApprenticeName, ApprenticeNINumber)
--	VALUES
--	(@providerName,4,null,null,null, @apprenticeshipCourseName,'01/06/2018', @apprenticeshipCourseLevel, @apprenticeName, null)

--	SELECT @paymentMetadataId  = SCOPE_IDENTITY()

--	INSERT INTO employer_financial.payment
--	(paymentid, ukprn,uln,accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, collectionperiodid, collectionperiodmonth, collectionperiodyear, evidencesubmittedon, employeraccountversion,apprenticeshipversion, fundingsource, transactiontype,amount,periodend,paymentmetadataid)
--	VALUES
--	(newid(), @ukprn, @uln, @accountid, @apprenticeshipid, 5, 2018,'1819-R01', 6, 2018, '2018-05-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount,'1819-R01',@paymentMetadataId)
--END;  
--GO  

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
		GETDATE(),
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
	@transactionDate datetime
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
		GETDATE(),
		@transactionDate,
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


--CREATE PROCEDURE #createAccountTransfers
--(     
--	@senderAccountId BIGINT,
--	@senderAccountName NVARCHAR(100),
--	@receiverAccountId BIGINT,
--	@receiverAccountName NVARCHAR(100),
--	@providerName NVARCHAR(MAX),
--	@ukprn BIGINT,
--	@courseName NVARCHAR(MAX),
--	@periodEnd NVARCHAR(20),
--	@totalAmount DECIMAL(18,4)
--)  
--AS  
--BEGIN  	

--    DECLARE @paymentAmount DECIMAL(18,4) = @totalAmount / 3
--	--todo: needs to come in.
--	-- replace this proc with #createPaymentsForMonth, then add transfers to it?
--	-- call processing per month

--	DECLARE @currentDate DATETIME = GETDATE()	

--	DECLARE @periodEndDate DATETIME = DATEADD(month, -1, @currentDate)

--	-- todo: bring over new stuff deom CreatePayments. merge with a is transfer flag on the source generation table??

--	exec #createPeriodEnd @periodEndDate

--	-- Create transfer payments
--	EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Mark Redwood', @ukprn, 1003, 3333, 1, @paymentAmount
--	--EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Sarah Redwood', @ukprn, 2003, 7777, 1, @paymentAmount 
--	--EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Tim Woods', @ukprn, 2004, 8888, 1, @paymentAmount 

--	-- Create transfers
--	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 3333, @courseName, @paymentAmount, @periodEnd, 0, @currentDate
--	--EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 7777, @courseName, @paymentAmount, @periodEnd, 0, @currentDate
--	--EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 8888, @courseName, @paymentAmount, @periodEnd, 0, @currentDate

--END
--GO

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

	exec #createPeriodEnd @periodEndDate

    EXEC #createAccountPayments @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', 10001378, @courseName, /*LevyTransfer*/5, @periodEndDate, @totalPaymentAmount, @numberOfPayments

	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 3333, @courseName, @totalPaymentAmount, @periodEndId, 'Levy', @createDate

	declare @negativeTotalPaymentAmount DECIMAL(18,5) = -@totalPaymentAmount
	EXEC #CreateAccountTransferTransaction @senderAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @negativeTotalPaymentAmount, @createDate
	EXEC #CreateAccountTransferTransaction @receiverAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @totalPaymentAmount, @createDate

	--todo: check what these are doing. are they required? if so will need datageneration edition that accepts date rather than using getdate
	--exec employer_financial.processdeclarationstransactions @senderAccountId, @senderPayeScheme
	--exec employer_financial.processdeclarationstransactions @receiverAccountId, @receiverPayeScheme

    exec #ProcessPaymentDataTransactionsGenerateDataEdition @receiverAccountId, @createDate

COMMIT TRANSACTION
END
GO

BEGIN
	--  ,--.--------.              ,---.      .-._          ,-,--.     _,---.     ,----.                           _,---.      ,----.  .-._           ,----.                ,---.   ,--.--------.  .=-.-.  _,.---._    .-._                ,--.-.,-.  .-._          _,.---._                 ,-,--. 
	-- /==/,  -   , -\.-.,.---.  .--.'  \    /==/ \  .-._ ,-.'-  _\ .-`.' ,  \ ,-.--` , \  .-.,.---.           _.='.'-,  \  ,-.--` , \/==/ \  .-._ ,-.--` , \  .-.,.---.  .--.'  \ /==/,  -   , -\/==/_ /,-.' , -  `. /==/ \  .-._        /==/- |\  \/==/ \  .-._ ,-.' , -  `.    _..---.  ,-.'-  _\
	-- \==\.-.  - ,-./==/  `   \ \==\-/\ \   |==|, \/ /, /==/_ ,_.'/==/_  _.-'|==|-  _.-` /==/  `   \         /==.'-     / |==|-  _.-`|==|, \/ /, /==|-  _.-` /==/  `   \ \==\-/\ \\==\.-.  - ,-./==|, |/==/_,  ,  - \|==|, \/ /, /       |==|_ `/_ /|==|, \/ /, /==/_,  ,  - \ .' .'.-. \/==/_ ,_.'
	--  `--`\==\- \ |==|-, .=., |/==/-|_\ |  |==|-  \|  |\==\  \  /==/-  '..-.|==|   `.-.|==|-, .=., |       /==/ -   .-'  |==|   `.-.|==|-  \|  ||==|   `.-.|==|-, .=., |/==/-|_\ |`--`\==\- \  |==|  |==|   .=.     |==|-  \|  |        |==| ,   / |==|-  \|  |==|   .=.     /==/- '=' /\==\  \   
	--       \==\_ \|==|   '='  /\==\,   - \ |==| ,  | -| \==\ -\ |==|_ ,    /==/_ ,    /|==|   '='  /       |==|_   /_,-./==/_ ,    /|==| ,  | -/==/_ ,    /|==|   '='  /\==\,   - \    \==\_ \ |==|- |==|_ : ;=:  - |==| ,  | -|        |==|-  .|  |==| ,  | -|==|_ : ;=:  - |==|-,   '  \==\ -\  
	--       |==|- ||==|- ,   .' /==/ -   ,| |==| -   _ | _\==\ ,\|==|   .--'|==|    .-' |==|- ,   .'        |==|  , \_.' )==|    .-' |==| -   _ |==|    .-' |==|- ,   .' /==/ -   ,|    |==|- | |==| ,|==| , '='     |==| -   _ |        |==| _ , \ |==| -   _ |==| , '='     |==|  .=. \ _\==\ ,\ 
	--       |==|, ||==|_  . ,'./==/-  /\ - \|==|  /\ , |/==/\/ _ |==|-  |   |==|_  ,`-._|==|_  . ,'.        \==\-  ,    (|==|_  ,`-._|==|  /\ , |==|_  ,`-._|==|_  . ,'./==/-  /\ - \   |==|, | |==|- |\==\ -    ,_ /|==|  /\ , |        /==/  '\  ||==|  /\ , |\==\ -    ,_ //==/- '=' ,/==/\/ _ |
	--       /==/ -//==/  /\ ,  )==\ _.\=\.-'/==/, | |- |\==\ - , /==/   \   /==/ ,     //==/  /\ ,  )        /==/ _  ,  //==/ ,     //==/, | |- /==/ ,     //==/  /\ ,  )==\ _.\=\.-'   /==/ -/ /==/. / '.='. -   .' /==/, | |- |        \==\ /\=\.'/==/, | |- | '.='. -   .'|==|   -   /\==\ - , /
	--       `--`--``--`-`--`--' `--`        `--`./  `--` `--`---'`--`---'   `--`-----`` `--`-`--`--'         `--`------' `--`-----`` `--`./  `--`--`-----`` `--`-`--`--' `--`           `--`--` `--`-`    `--`--''   `--`./  `--`         `--`      `--`./  `--`   `--`--''  `-._`.___,'  `--`---' 

	DECLARE @senderAccountId BIGINT            = 0
	DECLARE @SenderAccountName NVARCHAR(100)   = 'Sender Name'
	--DECLARE @senderPayeScheme NVARCHAR(16)     = '123/SE12345'

    DECLARE @receiverAccountId BIGINT          = 1
	DECLARE @receiverAccountName NVARCHAR(100) = 'Receiver Name'
	--DECLARE @receiverPayeScheme NVARCHAR(16)   = '123/RE12345'
	
    DECLARE @currentDate DATETIME              = GETDATE()
    DECLARE @totalPaymentAmount DECIMAL(18,4)  = 10000
	declare @numberOfPayments INT              = 1
	
	--  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,
	-- '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P' 

	exec #createPaymentAndTransferForMonth	@senderAccountId,   @senderAccountName,   -- @senderPayeScheme,
											@receiverAccountId, @receiverAccountName, -- @receiverPayeScheme,
											@currentDate, @totalPaymentAmount, @numberOfPayments

	--DECLARE @negativePaymentAmount DECIMAL(18,4) = -@totalPaymentAmount

	--EXEC #createAccountTransfers @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', 10001378, 'Accounting',  @periodEnd, @totalPaymentAmount	
	--EXEC #CreateAccountTransferTransaction @senderAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEnd, @totalPaymentAmount, @currentDate
	--EXEC #CreateAccountTransferTransaction @receiverAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEnd, @totalPaymentAmount, @currentDate

	--todo: why process levy decs? this script isn't adding any!
	--exec employer_financial.processdeclarationstransactions @senderAccountId, @senderPayeScheme
	--exec employer_financial.processdeclarationstransactions @receiverAccountId, @receiverPayeScheme
	--exec employer_financial.processpaymentdatatransactions @receiverAccountId
END
GO

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