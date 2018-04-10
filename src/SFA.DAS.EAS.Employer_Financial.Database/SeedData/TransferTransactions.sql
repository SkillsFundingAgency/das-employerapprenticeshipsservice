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
	select 1 FROM [employer_financial]‌.[periodend] 
	WHERE periodendid = '1718-R06'
)
BEGIN        
	insert into employer_financial.periodend (periodendid, calendarperiodmonth, calendarperiodyear, accountdatavalidat, commitmentdatavalidat, completiondatetime, paymentsforperiod)
	values
	('1718-R06',2,2018,'2018-02-04 00:00:00.000','2018-02-04 09:07:34.457','2018-02-04 10:50:27.760','https://pp-payments.apprenticeships.sfa.bis.gov.uk/api/payments?periodId=1617-R10')
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
	(@providerName,4,null,null,null, @apprenticeshipCourseName,'01/03/2018', @apprenticeshipCourseLevel, @apprenticeName, null)

	SELECT @paymentMetadataId  = SCOPE_IDENTITY()

	INSERT INTO employer_financial.payment
	(paymentid, ukprn,uln,accountid, apprenticeshipid, deliveryperiodmonth, deliveryperiodyear, collectionperiodid, collectionperiodmonth, collectionperiodyear, evidencesubmittedon, employeraccountversion,apprenticeshipversion, fundingsource, transactiontype,amount,periodend,paymentmetadataid)
	VALUES
	(newid(), @ukprn, @uln, @accountid, @apprenticeshipid, 2, 2018,'1718-R06', 3, 2018, '2018-02-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount,'1718-R06',@paymentMetadataId)
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
	INSERT INTO [employer_financial]‌.[AccountTransfers] 
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
		TransferDate, 
		CreatedDate
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
		GETDATE()
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
	INSERT INTO [employer_financial]‌.[TransactionLine]
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

CREATE PROCEDURE #createCoursePaymentsForAccount
(     
	@accountId BIGINT,
	@providerName NVARCHAR(MAX),
	@ukprn BIGINT,
	@apprenticeshipCourseName NVARCHAR(MAX)	
)  
AS  
BEGIN  	
	-- Levy Funded only
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'Bob Green', @ukprn, 1001, 1111, 1, 100		
	
    -- Levy with SFA & employer co-funding
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'Jane Doe', @ukprn, 1002, 2222, 1, 200
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'Jane Doe', @ukprn, 1002, 2222, 2, 450
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'Jane Doe', @ukprn, 1002, 2222, 3, 50
			
	-- Levy & SFA co-funding
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 1, 'Mark Redwood', @ukprn, 1003, 3333, 1, 100 
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 1, 'Mark Redwood', @ukprn, 1003, 3333, 2, 45
		
	-- Levy & employer co-funding
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Sarah Woods', @ukprn, 1004, 3333, 1, 200 
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Sarah Woods', @ukprn, 1004, 3333, 3, 80
		
	-- SFA & employer co-funding only
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Tom Tailor', @ukprn, 1005, 4444, 2, 900
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Tom Tailor', @ukprn, 1005, 4444, 3, 100

	-- Levy Funded only with refund
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'Mary Green', @ukprn, 2001, 5555, 1, 100
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'Mary Green', @ukprn, 2001, 5555, 1, -50	
	
    -- Levy with SFA & employer co-funding with refunds
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'John Doe', @ukprn, 2002, 6666, 1, 200
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'John Doe', @ukprn, 2002, 6666, 1, -100
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'John Doe', @ukprn, 2002, 6666, 2, 450
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'John Doe', @ukprn, 2002, 6666, 2, -250
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'John Doe', @ukprn, 2002, 6666, 3, 50
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 2, 'John Doe', @ukprn, 2002, 6666, 3, -25
			
	-- Levy & SFA co-funding with refunds
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 1, 'Sarah Redwood', @ukprn, 2003, 7777, 1, 100 
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 1, 'Sarah Redwood', @ukprn, 2003, 7777, 1, -50 
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 1, 'Sarah Redwood', @ukprn, 2003, 7777, 2, 45
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 1, 'Sarah Redwood', @ukprn, 2003, 7777, 2, -25
		
	-- Levy & employer co-funding with refunds
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Tim Woods', @ukprn, 2004, 8888, 1, 200 
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Tim Woods', @ukprn, 2004, 8888, 1, -100 
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Tim Woods', @ukprn, 2004, 8888, 3, 80
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Tim Woods', @ukprn, 2004, 8888, 3, -40
		
	-- SFA & employer co-funding only with refunds
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Anne Tailor', @ukprn, 2005, 9999, 2, 900
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Anne Tailor', @ukprn, 2005, 9999, 2, 900
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Anne Tailor', @ukprn, 2005, 9999, 3, 100
	EXEC #createPayment @accountId, @providerName, @apprenticeshipCourseName, 3, 'Anne Tailor', @ukprn, 2005, 9999, 3, 100
END;  
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
	@amount DECIMAL(18,4)
)  
AS  
BEGIN  	

	DECLARE @currentDate DATETIME
	SET @currentDate = GETDATE()

	-- Create transfer payments
	EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Mark Redwood', @ukprn, 1003, 3333, 1, @amount
	EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Sarah Redwood', @ukprn, 2003, 7777, 1, @amount 
	EXEC #createPayment @receiverAccountId, @providerName, @courseName, 4, 'Tim Woods', @ukprn, 2004, 8888, 1, @amount 

	-- Create transfers
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 3333, @courseName, @amount, @periodEnd, 0, @currentDate
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 7777, @courseName, @amount, @periodEnd, 0, @currentDate
	EXEC #createTransfer @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 8888, @courseName, @amount, @periodEnd, 0, @currentDate

END
GO

BEGIN
	DECLARE @senderAccountId BIGINT
	DECLARE @SenderAccountName NVARCHAR(100)
	DECLARE @receiverAccountId BIGINT
	DECLARE @receiverAccountName NVARCHAR(100)
	DECLARE @senderPayeScheme NVARCHAR(16)
	DECLARE @receiverPayeScheme NVARCHAR(16)
	DECLARE @levyAmount DECIMAL
	DECLARE @currentDate DATETIME

	-- EDIT THESE VALUES TO LOCAL VALUES IN DATABASE
	SET @levyAmount = 20000.0000
	SET @senderAccountId  = '<sender account id>'
	SET @senderAccountName  = '<sender name>'
	SET @receiverAccountId  = '<receiver account id>'
	SET @receiverAccountName  = '<receiver name>'
	SET @senderPayeScheme = '<sender PAYE scheme>'
	SET @receiverPayeScheme = '<receiver PAYE scheme>'
	SET @currentDate = GETDATE()
	----------------------------------------------------------

	INSERT INTO employer_financial.levydeclaration (AccountId,empref,levydueytd,levyallowanceforyear,submissiondate,submissionid,payrollyear,payrollmonth,createddate,hmrcsubmissionid)
	values
	(@senderAccountId, @senderPayeScheme, @levyAmount, 1500.0000, '2018-02-18 07:12:28.060', 1234567890, '17-18', 10, '2018-02-02 07:12:28.060', 123456)
	
	
	EXEC #createCoursePaymentsForAccount @senderAccountId, 'CHESTERFIELD COLLEGE', 10001378, 'Chemical Engineering'
	EXEC #createCoursePaymentsForAccount @senderAccountId, 'LEAD EDGE LTD', 10034146, 'Chemical Engineering'
	EXEC #createCoursePaymentsForAccount @senderAccountId, 'CHOICE TRAINING LTD', 10037909, 'Social Care'

	EXEC #createAccountTransfers @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', 10001378, 'Accounting', '1718-R06', 300
	EXEC #createAccountTransfers @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', 10001378, 'Traffic Management', '1718-R06', 500
	EXEC #createAccountTransfers @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, 'CHESTERFIELD COLLEGE', 10001378, 'Environmental Protection', '1718-R06', 650

	EXEC #CreateAccountTransferTransaction @senderAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, '1718-R06', -4350, 4, @currentDate
	EXEC #CreateAccountTransferTransaction @receiverAccountId, @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, '1718-R06', 4350, 4, @currentDate

	exec employer_financial.processdeclarationstransactions @senderAccountId, @senderPayeScheme
	exec employer_financial.processdeclarationstransactions @receiverAccountId, @receiverPayeScheme
	exec employer_financial.processpaymentdatatransactions @senderAccountId
END
GO