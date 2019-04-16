
DECLARE @accountId bigint                             = 1
DECLARE @accountName nvarchar(100)                    = 'Insert Name Here'
DECLARE @toDate datetime2                             = GETDATE()
DECLARE @numberOfMonthsToCreate int                   = 25
DECLARE @payeScheme nvarchar(50)                      = '222/ZZ00002'
DECLARE @monthlyLevy decimal(18, 4)                   = 1000
DECLARE @defaultMonthlyTotalPayments decimal(18,5)    = 100
DECLARE @defaultPaymentsPerMonth int                  = 3

DECLARE @senderAccountId bigint                       = @accountId
DECLARE @senderAccountName nvarchar(100)              = @accountName
DECLARE @receiverAccountId bigint                     = 2
DECLARE @receiverAccountName nvarchar(100)            = 'Receiver Name'
DECLARE @defaultMonthlyTransfer decimal(18,4)         = 100
DECLARE @defaultNumberOfTransferPaymentsPerMonth int  = 1

BEGIN TRANSACTION ScenarioExample

-- levy source table and overrides

DECLARE @levyDecByMonth DataGen.LevyGenerationSourceTable

INSERT @levyDecByMonth
SELECT * FROM DataGen.GenerateLevySourceTable(@toDate, @numberOfMonthsToCreate, @monthlyLevy)

--*** Levy Adjustments ***
--DECLARE @adjustmentAmount decimal(18, 4) = 1000
--UPDATE @levyDecByMonth SET amount = amount-@monthlyLevy-@adjustmentAmount WHERE payrollYear = '18-19' and payrollMonth >= 6

SELECT * FROM @levyDecByMonth

-- payment source table and overrides

DECLARE @paymentsByMonth DataGen.PaymentGenerationSourceTable

INSERT @paymentsByMonth
SELECT * FROM DataGen.GenerateSourceTable(@toDate, @numberOfMonthsToCreate, @defaultMonthlyTotalPayments, @defaultPaymentsPerMonth)

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 WHERE monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 WHERE monthBeforeToDate = -7

SELECT * FROM @paymentsByMonth

-- transfer source table

DECLARE @transfersByMonth DataGen.PaymentGenerationSourceTable

INSERT @transfersByMonth
SELECT * FROM DataGen.GenerateSourceTable(@toDate, @numberOfMonthsToCreate, @defaultMonthlyTransfer, @defaultNumberOfTransferPaymentsPerMonth)

SELECT * FROM @transfersByMonth

-- generate scenario from source table and parameters

--- Generate english fraction rows to cover the levy decs we're about to generate
EXEC DataGen.CreateEnglishFractions @payeScheme, @levyDecByMonth

EXEC DataGen.CreateLevyDecs @accountId, @payeScheme, @toDate, @levyDecByMonth

EXEC DataGen.CreatePaymentsForMonths @accountId, @accountName, @paymentsByMonth

EXEC DataGen.CreateTransfersForMonths @senderAccountId, @senderAccountName, @receiverAccountId, @receiverAccountName, @transfersByMonth

COMMIT TRANSACTION ScenarioExample
