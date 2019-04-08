-- Instructions: Find clearly signposted 'Levy Gereration Knobs' section below to change generation variable defaults

--todo: 
-- levy adjustments
-- convert to sp, and have scripts to run sps to set up levy & payments for a certain scenario
-- ^^^ have script to AddTestDataGeneration sprocs & functions, and RemoveTestDataGeneration, then call sprocs to generate and/or have scenarios that call sprocs
-- add functions to db, so can have params at top?
-- if exists functions
-- use DATE, rather than datetime when we only care about year/month
-- use dates generated into @levyDecByMonth table, rather than recalculatin

CREATE FUNCTION PayrollMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)

  SET @month = @month - 3
  IF @month < 1
	SET @month = @month + 12

  RETURN(@month);  
END; 
GO

CREATE FUNCTION PayrollYear (@date datetime)  
RETURNS VARCHAR(5)
AS  
BEGIN  
  declare @month int = DATEPART(month,@date)
  declare @year int = DATEPART(year,@date)

  if @month < 4
	SET @year = @year - 1
	
  DECLARE @payrollYear VARCHAR(5) = (SELECT RIGHT(CONVERT(VARCHAR(5), @year, 1), 2)) + '-' + (SELECT RIGHT(CONVERT(VARCHAR(4), @year+1, 1), 2))
  return @payrollYear
END; 
GO

BEGIN TRANSACTION CreateLevy
go

--  __                                   ____                                          __                            __  __                  __                
-- /\ \                                 /\  _`\                                       /\ \__  __                    /\ \/\ \                /\ \               
-- \ \ \         __   __  __   __  __   \ \ \L\_\     __    ___      __   _ __    __  \ \ ,_\/\_\    ___     ___    \ \ \/'/'    ___     ___\ \ \____    ____  
--  \ \ \  __  /'__`\/\ \/\ \ /\ \/\ \   \ \ \L_L   /'__`\/' _ `\  /'__`\/\`'__\/'__`\ \ \ \/\/\ \  / __`\ /' _ `\   \ \ , <   /' _ `\  / __`\ \ '__`\  /',__\ 
--   \ \ \L\ \/\  __/\ \ \_/ |\ \ \_\ \   \ \ \/, \/\  __//\ \/\ \/\  __/\ \ \//\ \L\.\_\ \ \_\ \ \/\ \L\ \/\ \/\ \   \ \ \\`\ /\ \/\ \/\ \L\ \ \ \L\ \/\__, `\
--    \ \____/\ \____\\ \___/  \/`____ \   \ \____/\ \____\ \_\ \_\ \____\\ \_\\ \__/.\_\\ \__\\ \_\ \____/\ \_\ \_\   \ \_\ \_\ \_\ \_\ \____/\ \_,__/\/\____/
--     \/___/  \/____/ \/__/    `/___/> \   \/___/  \/____/\/_/\/_/\/____/ \/_/ \/__/\/_/ \/__/ \/_/\/___/  \/_/\/_/    \/_/\/_/\/_/\/_/\/___/  \/___/  \/___/ 
--                                 /\___/                                                                                                                      
--                                 \/__/                                                                                                                       

DECLARE @accountId BIGINT                = 1
DECLARE @payeScheme NVARCHAR(50)         = '222/ZZ00002'
DECLARE @monthlyLevy DECIMAL(18, 4)      = 1000
-- last levy will be created in this month (last payroll month will be 1 month before)
DECLARE @toDate DATETIME2                = GETDATE()
DECLARE @numberOfMonthsToCreate INT      = 25
                                                                                                                                                                           
--  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______ 
-- /\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\
-- \/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/

DECLARE @levyDecByMonth TABLE (monthBeforeToDate INT, amount DECIMAL(18, 4), createMonth DATETIME, payrollYear VARCHAR(5), payrollMonth int)

declare @firstPayrollMonth datetime = DATEADD(month,-@numberOfMonthsToCreate+1-1,@toDate)
declare @firstPayrollYear VARCHAR(5) = dbo.PayrollYear(@firstPayrollMonth)

-- generates same levy per month
insert into @levyDecByMonth
SELECT TOP (@numberOfMonthsToCreate)
			monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
			(case
			when dbo.PayrollYear(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)) = @firstPayrollYear 
				THEN @monthlyLevy*row_number() over (order by (select NULL))
			ELSE
				@monthlyLevy*dbo.PayrollMonth(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate))
			END),
			DATEADD(month,/*monthBeforeToDate*/ -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate),
			dbo.PayrollYear(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)),
			dbo.PayrollMonth(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate))
FROM sys.all_objects
ORDER BY monthBeforeToDate;

--** Levy Adjustments ***
-- reduce levy declared by this amount for the given payroll year and month.
-- if the month reduction > the levy declared in that month (unless explicitely changed, @monthlyLevy), then it will generate a levy adjustment
--update @levyDecByMonth set amount = amount-2000 where payrollYear = '18-19' and payrollMonth >= 6

select * from @levyDecByMonth

---
--- Generate english fraction rows to cover the levy decs we're about to generate
---

-- engligh fractions usually generated on 1, 4, 7, 10 (not 3, 6, 9, 12), but can be generated anytime
declare @englishFractionMonths table (dateCalculated DATETIME)

-- first quarter before (or on) first month
insert @englishFractionMonths
select top 1 dateadd(month,-datepart(month,createMonth)%3, createMonth) from @levyDecByMonth order by createMonth

-- rest of the quarters
insert @englishFractionMonths
select createMonth from (select createMonth from @levyDecByMonth except select top 1 createMonth from @levyDecByMonth order by createMonth) x where datepart(month,createMonth)%3 = 0

-- only insert english fraction rows that don't already exist (and add english fraction calcs on consistent day of the month)
declare @newEnglishFractionMonths table (dateCalculated DATETIME)

insert @newEnglishFractionMonths
select datefromparts(datepart(year,dateCalculated), datepart(month,dateCalculated), 7) from @englishFractionMonths
except select dateCalculated from employer_financial.EnglishFraction where EmpRef = @payeScheme

INSERT employer_financial.EnglishFraction (DateCalculated, Amount, EmpRef, DateCreated)
select dateCalculated, 1.0, @payeScheme, dateCalculated from @newEnglishFractionMonths

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
	dbo.PayrollYear(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)), 
	dbo.PayrollMonth(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)), 
	DATEADD(month, monthBeforeToDate, @baselineCreatedDate), 
	@maxSubmissionId + row_number() over (order by (select NULL))
from @levyDecByMonth

---
--- Process the levy decs into transaction lines
---

EXEC employer_financial.processdeclarationstransactions @accountId, @payeScheme

go

COMMIT TRANSACTION CreateLevy

drop FUNCTION PayrollYear
go

drop FUNCTION PayrollMonth
go

