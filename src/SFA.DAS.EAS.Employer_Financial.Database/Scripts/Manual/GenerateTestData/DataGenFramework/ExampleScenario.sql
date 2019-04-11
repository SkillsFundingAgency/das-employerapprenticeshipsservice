
DECLARE @accountId BIGINT                          = 1
declare @accountName NVARCHAR(100)                 = 'Insert Name Here'
DECLARE @toDate DATETIME2                          = GETDATE()
DECLARE @numberOfMonthsToCreate INT                = 25
DECLARE @payeScheme NVARCHAR(50)                   = '222/ZZ00002'
DECLARE @monthlyLevy DECIMAL(18, 4)                = 1000
declare @defaultMonthlyTotalPayments DECIMAL(18,5) = 100
declare @defaultPaymentsPerMonth int               = 3

BEGIN TRANSACTION ScenarioExample

-- levy source table and overrides

DECLARE @levyDecByMonth DataGen.LevyGenerationSourceTable

insert @levyDecByMonth
select * from DataGen.GenerateLevySourceTable(@toDate, @numberOfMonthsToCreate, @monthlyLevy)

--** Levy Adjustments ***
-- reduce levy declared by this amount for the given payroll year and month.
-- if the month reduction > the levy declared in that month (unless explicitely changed, @monthlyLevy), then it will generate a levy adjustment
--declare @adjustmentAmount DECIMAL(18, 4) = 1000
--update @levyDecByMonth set amount = amount-@monthlyLevy-@adjustmentAmount where payrollYear = '18-19' and payrollMonth >= 6

select * from @levyDecByMonth

-- payment source table and overrides

declare @paymentsByMonth DataGen.PaymentGenerationSourceTable

insert @paymentsByMonth
select * from DataGen.GenerateSourceTable(@toDate, @numberOfMonthsToCreate, @defaultMonthlyTotalPayments, @defaultPaymentsPerMonth)

-- override defaults here...
-- e.g. to create refunds set the amount -ve
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -1
--UPDATE @paymentsByMonth SET amount = -500, paymentsToGenerate = 1 where monthBeforeToDate = -7

select * from @paymentsByMonth

-- generate scenario from source table and parameters

--- Generate english fraction rows to cover the levy decs we're about to generate
exec DataGen.CreateEnglishFractions @payeScheme, @levyDecByMonth

exec DataGen.CreateLevyDecs @accountId, @payeScheme, @toDate, @levyDecByMonth

exec DataGen.CreatePaymentsForMonths @accountId, @accountName, @paymentsByMonth

COMMIT TRANSACTION ScenarioExample
