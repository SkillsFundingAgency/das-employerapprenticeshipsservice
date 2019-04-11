
DECLARE @accountId BIGINT                = 1
DECLARE @payeScheme NVARCHAR(50)         = '222/ZZ00002'
DECLARE @monthlyLevy DECIMAL(18, 4)      = 1000
-- last levy will be created in this month (last payroll month will be 1 month before)
DECLARE @toDate DATETIME2                = GETDATE()
DECLARE @numberOfMonthsToCreate INT      = 25

BEGIN TRANSACTION CreateLevy

DECLARE @levyDecByMonth DataGen.LevyGenerationSourceTable

insert @levyDecByMonth
select * from DataGen.GenerateLevySourceTable(@toDate, @numberOfMonthsToCreate, @monthlyLevy)

--*** Levy Adjustments ***
--declare @adjustmentAmount DECIMAL(18, 4) = 1000
--update @levyDecByMonth set amount = amount-@monthlyLevy-@adjustmentAmount where payrollYear = '18-19' and payrollMonth >= 6

select * from @levyDecByMonth

--- Generate english fraction rows to cover the levy decs we're about to generate
exec DataGen.CreateEnglishFractions @payeScheme, @levyDecByMonth

exec DataGen.CreateLevyDecs @accountId, @payeScheme, @toDate, @levyDecByMonth

COMMIT TRANSACTION CreateLevy
