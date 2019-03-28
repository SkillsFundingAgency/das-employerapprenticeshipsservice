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

IF OBJECT_ID('tempdb..#createTransfer') IS NOT NULL
BEGIN
    DROP PROC #createTransfer
END
GO

IF OBJECT_ID('tempdb..#createAccountTransfers') IS NOT NULL
BEGIN
    DROP PROC #createAccountTransfers
END
GO

IF OBJECT_ID('tempdb..#CreateAccountTransferTransaction') IS NOT NULL
BEGIN
    DROP PROC #CreateAccountTransferTransaction
END
GO
-- Add period end if its not already there
IF NOT EXISTS
(
	select 1 FROM [employer_financial].[periodend] 
	WHERE periodendid = '1819-R01'
)
BEGIN        
	insert into employer_financial.periodend (periodendid, calendarperiodmonth, calendarperiodyear, accountdatavalidat, commitmentdatavalidat, completiondatetime, paymentsforperiod)
	values
	('1819-R01',5,2018,'2018-05-04 00:00:00.000','2018-05-04 09:07:34.457','2018-05-04 10:50:27.760','https://pp-payments.apprenticeships.sfa.bis.gov.uk/api/payments?periodId=1617-R10')
END
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
	@Amount DECIMAL  
)  
AS  
BEGIN  
	DECLARE @paymentMetadataId bigint

	INSERT INTO employer_financial.paymentmetadata 
	(ProviderName, StandardCode, FrameworkCode, ProgrammeType, PathwayCode, ApprenticeshipCourseName, ApprenticeshipCourseStartDate, ApprenticeshipCourseLevel, ApprenticeName, ApprenticeNINumber)
	VALUES
	(@providerName,4,null,null,null, @apprenticeshipCourseName,'01/06/2018', @apprenticeshipCourseLevel, @apprenticeName, null)

	SELECT @paymentMetadataId  = SCOPE_IDENTITY()

	INSERT INTO employer_financial.payment
	(paymentid, ukprn,uln,accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, collectionperiodid, collectionperiodmonth, collectionperiodyear, evidencesubmittedon, employeraccountversion,apprenticeshipversion, fundingsource, transactiontype,amount,periodend,paymentmetadataid)
	VALUES
	(newid(), @ukprn, @uln, @accountid, @apprenticeshipid, 5, 2018,'1819-R01', 6, 2018, '2018-05-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount,'1819-R01',@paymentMetadataId)
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
	@type smallint,
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
	@transactionType smallint,
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
		@transactionType,
		@amount,
		@periodEnd,
		@senderAccountId,
		@receiverAccountId,
		@receiversAccountName,
		@senderAccountName
	)
END
GO


CREATE PROCEDURE #createAccountTransfers
(     
	@senderAccountId BIGINT,
	@senderAccountName NVARCHAR(100),
	@receiverAccountId BIGINT,
	@receiverAccountName NVARCHAR(100),
	@providerName NVARCHAR(MAX),
	@ukprn BIGINT,
	@courseName NVARCHAR(MAX),
	@periodEnd NVARCHAR(20),
	@totalAmount DECIMAL(18,4)
)  
AS  
BEGIN  	

    DECLARE @paymentAmount DECIMAL(18,4) = @totalAmount / 3
	DECLARE @currentDate DATETIME = GETDATE()	

	-- Create transfer payments
	EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Mark Redwood', @ukprn, 1003, 3333, 1, @paymentAmount
	EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Sarah Redwood', @ukprn, 2003, 7777, 1, @paymentAmount 
	EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Tim Woods', @ukprn, 2004, 8888, 1, @paymentAmount 

	-- Create transfers
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 3333, @courseName, @paymentAmount, @periodEnd, 0, @currentDate
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 7777, @courseName, @paymentAmount, @periodEnd, 0, @currentDate
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 8888, @courseName, @paymentAmount, @periodEnd, 0, @currentDate

END
GO

BEGIN
	DECLARE @senderAccountId BIGINT
	DECLARE @SenderAccountName NVARCHAR(100)
	DECLARE @receiverAccountId BIGINT
	DECLARE @receiverAccountName NVARCHAR(100)
	DECLARE @senderPayeScheme NVARCHAR(16)
	DECLARE @receiverPayeScheme NVARCHAR(16)
	DECLARE @currentDate DATETIME
    DECLARE @periodEnd VARCHAR(20)
    DECLARE @totalPaymentAmount DECIMAL(18,4)	

    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ------ EDIT THE VALUES BELOW TO AFFECT THE TRANSFER PAYMENTS ---------------------------------------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	SET @senderAccountId  = 0
	SET @senderAccountName  = 'XXX'
	SET @senderPayeScheme = 'XXX'

    SET @receiverAccountId  = 1
	SET @receiverAccountName  = 'XXX'	
	SET @receiverPayeScheme = 'XXX'
	
    SET @currentDate = GETDATE()
    SET @periodEnd = '1819-R01'
    SET @totalPaymentAmount = 10000
	
    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ------ END OF SCRIPT VARIABLES  --------------------------------------------------------------------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	DECLARE @negativePaymentAmount DECIMAL(18,4) = @totalPaymentAmount * -1

	EXEC #createAccountTransfers @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', 10001378, 'Accounting',  @periodEnd, @totalPaymentAmount	
	EXEC #CreateAccountTransferTransaction @senderAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEnd, @negativePaymentAmount, 4, @currentDate
	EXEC #CreateAccountTransferTransaction @receiverAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @periodEnd, @totalPaymentAmount, 4, @currentDate

	exec employer_financial.processdeclarationstransactions @senderAccountId, @senderPayeScheme
	exec employer_financial.processdeclarationstransactions @receiverAccountId, @receiverPayeScheme
	exec employer_financial.processpaymentdatatransactions @receiverAccountId
END
GO