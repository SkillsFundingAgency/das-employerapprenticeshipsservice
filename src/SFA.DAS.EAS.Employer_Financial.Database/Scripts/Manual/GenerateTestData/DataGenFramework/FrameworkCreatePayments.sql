
DECLARE @accountId bigint                          = 1
DECLARE @accountName nvarchar(100)                 = 'Insert Name Here'
DECLARE @toDate datetime                           = GETDATE()
DECLARE @numberOfMonthsToCreate int                = 25
DECLARE @defaultMonthlyTotalPayments decimal(18,5) = 100
DECLARE @defaultPaymentsPerMonth int               = 3

--todo: wrap this in a stored proc with a dynamic sql 'callback' for altering the defaults?

DECLARE @paymentsByMonth DataGen.PaymentGenerationSourceTable

INSERT @paymentsByMonth
SELECT * FROM DataGen.GenerateSourceTable(@toDate, @numberOfMonthsToCreate, @defaultMonthlyTotalPayments, @defaultPaymentsPerMonth)

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 WHERE monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 WHERE monthBeforeToDate = -7

SELECT * FROM @paymentsByMonth

EXEC DataGen.CreatePaymentsForMonths @accountId, @accountName, @paymentsByMonth
