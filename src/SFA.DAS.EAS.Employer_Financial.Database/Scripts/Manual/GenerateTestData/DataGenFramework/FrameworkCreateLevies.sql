
DECLARE @accountId bigint                = 1
DECLARE @payeScheme nvarchar(50)         = '222/ZZ00002'
DECLARE @monthlyLevy decimal(18, 4)      = 1000
-- last levy will be created in this month (last payroll month will be 1 month before)
DECLARE @toDate datetime2                = GETDATE()
DECLARE @numberOfMonthsToCreate int      = 25

BEGIN TRANSACTION CreateLevy

DECLARE @levyDecByMonth DataGen.LevyGenerationSourceTable

INSERT @levyDecByMonth
SELECT * FROM DataGen.GenerateLevySourceTable(@toDate, @numberOfMonthsToCreate, @monthlyLevy)

--*** Levy Adjustments ***
--declare @adjustmentAmount DECIMAL(18, 4) = 1000
--update @levyDecByMonth set amount = amount-@monthlyLevy-@adjustmentAmount where payrollYear = '18-19' and payrollMonth >= 6

SELECT * FROM @levyDecByMonth

--- Generate english fraction rows to cover the levy decs we're about to generate
EXEC DataGen.CreateEnglishFractions @payeScheme, @levyDecByMonth

EXEC DataGen.CreateLevyDecs @accountId, @payeScheme, @toDate, @levyDecByMonth

COMMIT TRANSACTION CreateLevy
