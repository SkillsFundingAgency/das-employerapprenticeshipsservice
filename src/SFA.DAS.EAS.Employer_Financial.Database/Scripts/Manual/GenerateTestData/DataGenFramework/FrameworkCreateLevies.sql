
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

--** Levy Adjustments ***
-- reduce levy declared by this amount for the given payroll year and month.
-- if the month reduction > the levy declared in that month (unless explicitely changed, @monthlyLevy), then it will generate a levy adjustment
--declare @adjustmentAmount DECIMAL(18, 4) = 1000
--update @levyDecByMonth set amount = amount-@monthlyLevy-@adjustmentAmount where payrollYear = '18-19' and payrollMonth >= 6

select * from @levyDecByMonth

--- Generate english fraction rows to cover the levy decs we're about to generate
exec DataGen.CreateEnglishFractions @payeScheme, @levyDecByMonth

---
--- Generate levy decs
---

DECLARE @maxSubmissionId BIGINT = ISNULL((SELECT MAX(SubmissionId) FROM employer_financial.levydeclaration),0)

declare @baselineSubmissionDate datetime = datefromparts(year(@toDate), month(@toDate), 18)
declare @baselineCreatedDate datetime = datefromparts(year(@toDate), month(@toDate), 20)
declare @baselinePayrollDate datetime = DATEADD(month, -1, @toDate)

--todo use monthBeforeToDate, rather than row_number?
INSERT INTO employer_financial.levydeclaration (AccountId,empref,levydueytd,levyallowanceforyear,submissiondate,submissionid,payrollyear,payrollmonth,createddate,hmrcsubmissionid)
select @accountId, @payeScheme, 
	amount,
	1500.0000, 
	DATEADD(month, monthBeforeToDate, @baselineSubmissionDate), 
	@maxSubmissionId + row_number() over (order by (select NULL)), 
	DataGen.PayrollYear(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)), 
	DataGen.PayrollMonth(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)), 
	DATEADD(month, monthBeforeToDate, @baselineCreatedDate), 
	@maxSubmissionId + row_number() over (order by (select NULL))
from @levyDecByMonth

---
--- Process the levy decs into transaction lines
---

EXEC employer_financial.processdeclarationstransactions @accountId, @payeScheme

COMMIT TRANSACTION CreateLevy
