-- Instructions: See 'Transfer Generation Knobs' section towards the end of this file for the control knobs

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

  DECLARE @periodEndMonth varchar(5) = CONVERT(varchar(5), @month)
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

  IF @month < 8
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

-- Add period end if its not already there
CREATE OR ALTER PROCEDURE #createPeriodEnd
(
	@periodEndDate datetime
)
AS
BEGIN
	DECLARE @periodEndId varchar(8) = dbo.PeriodEnd(@periodEndDate)

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

CREATE OR ALTER PROCEDURE #createAccountPayments
(    	
    @accountId bigint,
    @accountName nvarchar(100),
    @providerName nvarchar(max),
    @ukprn bigint,
    @courseName nvarchar(max),
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

      EXEC #createPayment @accountId, @providerName, @courseName, 1, @name, @ukprn, @uln, @apprenticeshipId, @fundingSourceString, @paymentAmount, @periodEndDate
	END
END
GO


CREATE OR ALTER PROCEDURE #createPayment
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
	--declare @evidenceSubmittedOn datetime = DATEADD(month, 1, @deliveryPeriodDate)

    INSERT INTO employer_financial.payment
    (paymentid, ukprn, uln, accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, 
	collectionperiodid, collectionperiodmonth, collectionperiodyear, 
	evidencesubmittedon, employeraccountversion, apprenticeshipversion, fundingsource, transactiontype, amount, periodend, paymentmetadataid)
    VALUES
    (NEWID(), @ukprn, @uln, @accountid, @apprenticeshipid, dbo.CalendarPeriodMonth(@deliveryPeriodDate), dbo.CalendarPeriodYear(@deliveryPeriodDate),
	dbo.PeriodEnd(@periodEndDate), dbo.CollectionPeriodMonth(@periodEndDate), dbo.CollectionPeriodYear(@periodEndDate), 
	'2018-06-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount, dbo.PeriodEnd(@periodEndDate), @paymentMetadataId)
END;  
GO 

CREATE OR ALTER PROCEDURE #createTransfer
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

CREATE OR ALTER PROCEDURE #CreateAccountTransferTransaction
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

CREATE OR ALTER PROCEDURE #ProcessPaymentDataTransactionsGenerateDataEdition
	@AccountId bigint,
	@DateCreated datetime
AS

--- Process Levy Payments ---
INSERT INTO [employer_financial].[TransactionLine]
SELECT mainUpdate.* FROM
    (
    SELECT 
            x.AccountId as AccountId,
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
			0 as EnglishFraction,
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

CREATE OR ALTER PROCEDURE #createPaymentAndTransferForMonth
(    	
	@senderAccountId bigint,
	@senderAccountName nvarchar(100),
	--@senderPayeScheme nvarchar(16),
	@receiverAccountId bigint,
	@receiverAccountName nvarchar(100),
	--@receiverPayeScheme nvarchar(16),
	@createDate datetime,
	@totalPaymentAmount decimal(18,5),
	@numberOfPayments int
)  
AS  
BEGIN  
BEGIN TRANSACTION

    DECLARE @periodEndDate datetime = DATEADD(month, -1, @createDate)
	DECLARE @periodEndId varchar(8) = dbo.PeriodEnd(@periodEndDate)
	DECLARE @courseName nvarchar(max) = 'Plate Spinning'
	DECLARE @ukprn bigint = 10001378
	DECLARE @apprenticeshipId bigint

	EXEC #createPeriodEnd @periodEndDate

	--todo: we currently make sure that we always use an unique apprenticeshipid, so we don't fall over the unique index [IX_PeriodEndAccountTransfer]
	-- ^^ this might be good enough, but in reality, each month would have transfers for the same apprenticeshipid
	-- so it would be better to generate the first one outside of this sproc and reuse it for each month (once we generate >1 transfer at a time)
	-- or else have it as a user param (+ve can generate correct data, -ve requires manual work and greater understanding)
    EXEC #createAccountPayments @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', @ukprn, @courseName, /*LevyTransfer*/5, @periodEndDate, @totalPaymentAmount, @numberOfPayments,
								@firstApprenticeshipId = @apprenticeshipId OUTPUT 

	--note: employer finance inserts the first apprenticeshipId from the set of transfers (and the others are ignored for the accounttransfer row)
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @apprenticeshipId, @courseName, @totalPaymentAmount, @periodEndId, 'Levy', @createDate

	DECLARE @negativeTotalPaymentAmount decimal(18,5) = -@totalPaymentAmount
	EXEC #CreateAccountTransferTransaction @senderAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @negativeTotalPaymentAmount, @createDate
	EXEC #CreateAccountTransferTransaction @receiverAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEndId, @totalPaymentAmount, @createDate

	--todo: check what these are doing. are they required? if so will need datageneration edition that accepts date rather than using getdate
	--exec employer_financial.processdeclarationstransactions @senderAccountId, @senderPayeScheme
	--exec employer_financial.processdeclarationstransactions @receiverAccountId, @receiverPayeScheme

	-- #ProcessPaymentDataTransactionsGenerateDataEdition doesn't create a new payment transactionline where one already exists
	-- so we remove any current payment transactionline first, so that payments can be additively generated in a month
	DELETE [employer_financial].[TransactionLine] WHERE AccountId = @receiverAccountId AND Ukprn = @ukprn AND PeriodEnd = @periodEndId AND TransactionType = 3

    EXEC #ProcessPaymentDataTransactionsGenerateDataEdition @receiverAccountId, @createDate

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

	DECLARE @senderAccountId bigint               = 0
	DECLARE @SenderAccountName nvarchar(100)      = 'Sender Name'
	--DECLARE @senderPayeScheme nvarchar(16)      = '123/SE12345'

    DECLARE @receiverAccountId bigint             = 1
	DECLARE @receiverAccountName nvarchar(100)    = 'Receiver Name'
	--DECLARE @receiverPayeScheme nvarchar(16)    = '123/RE12345'
	
    DECLARE @toDate datetime                      = GETDATE()
	declare @numberOfMonthsToCreate int           = 25
	declare @defaultMonthlyTransfer decimal(18,4) = 100

	declare @defaultNumberOfPaymentsPerMonth int  = 1
	
	--  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,  ,d88b.    ,
	-- '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P'  '    `Y88P' 

	DECLARE @paymentsByMonth TABLE (monthBeforeToDate int, amount decimal(18, 4), paymentsToGenerate int, createMonth datetime)

-- generate defaults
INSERT INTO @paymentsByMonth
SELECT TOP (@numberOfMonthsToCreate)
			monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
			@defaultMonthlyTransfer,
			@defaultNumberOfPaymentsPerMonth,
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

  EXEC #createPaymentAndTransferForMonth	@senderAccountId,   @senderAccountName,   -- @senderPayeScheme,
											@receiverAccountId, @receiverAccountName, -- @receiverPayeScheme,
											@createDate, @amount, @paymentsToGenerate
END

DROP FUNCTION CalendarPeriodYear
GO
DROP FUNCTION CalendarPeriodMonth
GO
DROP FUNCTION CollectionPeriodYear
GO
DROP FUNCTION CollectionPeriodMonth
GO
DROP FUNCTION PeriodEndYear
GO
DROP FUNCTION PeriodEndMonth
GO
DROP FUNCTION PeriodEnd
GO