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
    @Amount DECIMAL,
    @periodEnd VARCHAR(25)  
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
    (newid(), @ukprn, @uln, @accountid, @apprenticeshipid, 5, 2018, @periodend, 6, 2018, '2018-06-03 16:24:22.340', 20170504, 69985, @fundingsource, 1, @Amount, @periodEnd, @paymentMetadataId)
END;  
GO  

CREATE PROCEDURE #createAccountPayments
(    	
    @accountId BIGINT,
    @accountName NVARCHAR(100),
    @providerName NVARCHAR(MAX),
    @ukprn BIGINT,
    @courseName NVARCHAR(MAX),
    @periodEnd NVARCHAR(25),
    @totalAmount DECIMAL(18,5)
)  
AS  
BEGIN  	

    DECLARE @paymentAmount DECIMAL(18,5) = @totalAmount / 3
    DECLARE @currentDate DATETIME = GETDATE()	

    -- Create transfer payments
    EXEC #createPayment @accountId, @providerName, @courseName, 1, 'Mark Redwood', @ukprn, 1003, 3333, 1, @paymentAmount, @periodEnd
    EXEC #createPayment @accountId, @providerName, @courseName, 1, 'Sarah Redwood', @ukprn, 2003, 7777, 1, @paymentAmount, @periodEnd 
    EXEC #createPayment @accountId, @providerName, @courseName, 1, 'Tim Woods', @ukprn, 2004, 8888, 1, @paymentAmount, @periodEnd 	
END
GO

BEGIN TRANSACTION
    
    DECLARE @accountId BIGINT
    DECLARE @accountName NVARCHAR(100)	
    DECLARE @payeScheme NVARCHAR(16)
    DECLARE @currentDate DATETIME
    DECLARE @periodEnd VARCHAR(20)
    DECLARE @totalPaymentAmount DECIMAL(18,5)	

    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ------ EDIT THE VALUES BELOW TO AFFECT THE TRANSFER PAYMENTS ---------------------------------------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    SET @accountId  = 0
    SET @accountName  = 'XXX'	
    SET @payeScheme = 'XXX'
    
    SET @currentDate = GETDATE()
    SET @periodEnd = '1819-R01'
    SET @totalPaymentAmount = 10000
    
    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    ------ END OF SCRIPT VARIABLES  --------------------------------------------------------------------------------------------------------------------------------------------
    ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    EXEC #createAccountPayments @accountId, @accountName, 'CHESTERFIELD COLLEGE', 10001378, 'Accounting',  @periodEnd, @totalPaymentAmount	
        
    exec employer_financial.processpaymentdatatransactions @accountId

COMMIT TRANSACTION
GO