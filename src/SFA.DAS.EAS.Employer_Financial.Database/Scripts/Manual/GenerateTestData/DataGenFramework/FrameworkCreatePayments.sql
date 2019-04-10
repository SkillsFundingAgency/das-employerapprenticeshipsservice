
declare @accountId BIGINT                          = 1
declare @accountName NVARCHAR(100)                 = 'Insert Name Here'
declare @toDate DATETIME                           = GETDATE()
declare @numberOfMonthsToCreate INT                = 25
declare @defaultMonthlyTotalPayments DECIMAL(18,5) = 100
declare @defaultPaymentsPerMonth int               = 3

--todo: wrap this in a stored proc with a dynamic sql 'callback' for altering the defaults?

declare @paymentsByMonth DataGen.PaymentGenerationSourceTable

insert @paymentsByMonth
select * from DataGen.GenerateSourceTable(@toDate, @numberOfMonthsToCreate, @defaultMonthlyTotalPayments, @defaultPaymentsPerMonth)

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -7

select * from @paymentsByMonth

exec DataGen.CreatePaymentsForMonths @accountId, @accountName, @paymentsByMonth
