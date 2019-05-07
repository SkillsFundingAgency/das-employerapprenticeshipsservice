-- Instructions: Find clearly signposted 'Levy Generation Knobs' section below to change generation variable defaults

CREATE FUNCTION PayrollMonth (@date datetime)  
RETURNS int 
AS  
BEGIN  
  DECLARE @month int = DATEPART(month,@date)

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
  DECLARE @month int = DATEPART(month,@date)
  DECLARE @year int = DATEPART(year,@date)

  IF @month < 4
	SET @year = @year - 1
	
  DECLARE @payrollYear VARCHAR(5) = (SELECT RIGHT(CONVERT(VARCHAR(5), @year, 1), 2)) + '-' + (SELECT RIGHT(CONVERT(VARCHAR(4), @year+1, 1), 2))
  RETURN @payrollYear
END; 
GO

BEGIN TRANSACTION CreateLevy
GO

--  __                                   ____                                          __                            __  __                  __                
-- /\ \                                 /\  _`\                                       /\ \__  __                    /\ \/\ \                /\ \               
-- \ \ \         __   __  __   __  __   \ \ \L\_\     __    ___      __   _ __    __  \ \ ,_\/\_\    ___     ___    \ \ \/'/'    ___     ___\ \ \____    ____  
--  \ \ \  __  /'__`\/\ \/\ \ /\ \/\ \   \ \ \L_L   /'__`\/' _ `\  /'__`\/\`'__\/'__`\ \ \ \/\/\ \  / __`\ /' _ `\   \ \ , <   /' _ `\  / __`\ \ '__`\  /',__\ 
--   \ \ \L\ \/\  __/\ \ \_/ |\ \ \_\ \   \ \ \/, \/\  __//\ \/\ \/\  __/\ \ \//\ \L\.\_\ \ \_\ \ \/\ \L\ \/\ \/\ \   \ \ \\`\ /\ \/\ \/\ \L\ \ \ \L\ \/\__, `\
--    \ \____/\ \____\\ \___/  \/`____ \   \ \____/\ \____\ \_\ \_\ \____\\ \_\\ \__/.\_\\ \__\\ \_\ \____/\ \_\ \_\   \ \_\ \_\ \_\ \_\ \____/\ \_,__/\/\____/
--     \/___/  \/____/ \/__/    `/___/> \   \/___/  \/____/\/_/\/_/\/____/ \/_/ \/__/\/_/ \/__/ \/_/\/___/  \/_/\/_/    \/_/\/_/\/_/\/_/\/___/  \/___/  \/___/ 
--                                 /\___/                                                                                                                      
--                                 \/__/                                                                                                                       

DECLARE @accountId bigint                = 1
DECLARE @payeScheme nvarchar(50)         = '222/ZZ00002'
DECLARE @monthlyLevy decimal(18, 4)      = 1000
-- last levy will be created in this month (last payroll month will be 1 month before)
DECLARE @toDate datetime2                = GETDATE()
DECLARE @numberOfMonthsToCreate int      = 25
                                                                                                                                                                           
--  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______  _______ 
-- /\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\/\______\
-- \/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/\/______/

DECLARE @levyDecByMonth TABLE (monthBeforeToDate int, amount decimal(18, 4), createMonth datetime, payrollYear varchar(5), payrollMonth int)

DECLARE @firstPayrollMonth datetime = DATEADD(month,-@numberOfMonthsToCreate+1-1,@toDate)
DECLARE @firstPayrollYear VARCHAR(5) = dbo.PayrollYear(@firstPayrollMonth)

DECLARE @monthNumber INT = 1

-- generates same levy per month
WHILE @monthNumber <= @numberOfMonthsToCreate
BEGIN
	INSERT INTO @levyDecByMonth (monthBeforeToDate, amount, createMonth, payrollYear, payrollMonth)
	VALUES (
			-@numberOfMonthsToCreate+@monthNumber, 
			(CASE
			WHEN dbo.PayrollYear(DATEADD(month, -1-@numberOfMonthsToCreate+@monthNumber, @toDate)) = @firstPayrollYear 
				THEN @monthlyLevy*@monthNumber
				ELSE @monthlyLevy*dbo.PayrollMonth(DATEADD(month, -1-@numberOfMonthsToCreate+@monthNumber, @toDate))
			END),
			DATEADD(month, -@numberOfMonthsToCreate+@monthNumber, @toDate),
			dbo.PayrollYear(DATEADD(month, -1-@numberOfMonthsToCreate+@monthNumber, @toDate)),
			dbo.PayrollMonth(DATEADD(month, -1-@numberOfMonthsToCreate+@monthNumber, @toDate)))

    SET @monthNumber = @monthNumber + 1
END

--*** Levy Adjustments ***
--declare @adjustmentAmount DECIMAL(18, 4) = 1000
--update @levyDecByMonth set amount = amount-@monthlyLevy-@adjustmentAmount where payrollYear = '18-19' and payrollMonth >= 6

SELECT * FROM @levyDecByMonth

---
--- Generate english fraction rows to cover the levy decs we're about to generate
---

-- engligh fractions usually generated on 1, 4, 7, 10 (not 3, 6, 9, 12), but can be generated anytime
DECLARE @englishFractionMonths TABLE (dateCalculated datetime)

-- first quarter before (or on) first month
INSERT @englishFractionMonths
SELECT TOP 1 DATEADD(month,-DATEPART(month,createMonth)%3, createMonth) FROM @levyDecByMonth ORDER BY createMonth

-- rest of the quarters
INSERT @englishFractionMonths
SELECT createMonth FROM (SELECT createMonth FROM @levyDecByMonth EXCEPT SELECT TOP 1 createMonth FROM @levyDecByMonth ORDER BY createMonth) x WHERE DATEPART(month,createMonth)%3 = 0

-- only insert english fraction rows that don't already exist (and add english fraction calcs on consistent day of the month)
DECLARE @newEnglishFractionMonths TABLE (dateCalculated datetime)

INSERT @newEnglishFractionMonths
SELECT DATEFROMPARTS(DATEPART(year,dateCalculated), DATEPART(month,dateCalculated), 7) FROM @englishFractionMonths
EXCEPT SELECT dateCalculated FROM employer_financial.EnglishFraction WHERE EmpRef = @payeScheme

INSERT employer_financial.EnglishFraction (DateCalculated, Amount, EmpRef, DateCreated)
SELECT dateCalculated, 1.0, @payeScheme, dateCalculated FROM @newEnglishFractionMonths

---
--- Generate levy decs
---

DECLARE @maxSubmissionId bigint = ISNULL((SELECT MAX(SubmissionId) FROM employer_financial.levydeclaration),0)

DECLARE @baselineSubmissionDate datetime = DATEFROMPARTS(year(@toDate), month(@toDate), 18)
DECLARE @baselineCreatedDate datetime = DATEFROMPARTS(year(@toDate), month(@toDate), 20)
DECLARE @baselinePayrollDate datetime = DATEADD(month, -1, @toDate)

--todo use monthBeforeToDate, rather than row_number?
INSERT INTO employer_financial.levydeclaration (AccountId,empref,levydueytd,levyallowanceforyear,submissiondate,submissionid,payrollyear,payrollmonth,createddate,hmrcsubmissionid)
SELECT @accountId, @payeScheme, 
	amount,
	1500.0000, 
	DATEADD(month, monthBeforeToDate, @baselineSubmissionDate), 
	@maxSubmissionId + ROW_NUMBER() OVER (ORDER BY (SELECT NULL)), 
	dbo.PayrollYear(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)), 
	dbo.PayrollMonth(DATEADD(month, monthBeforeToDate, @baselinePayrollDate)), 
	DATEADD(month, monthBeforeToDate, @baselineCreatedDate), 
	@maxSubmissionId + ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
FROM @levyDecByMonth

---
--- Process the levy decs into transaction lines
---

EXEC employer_financial.processdeclarationstransactions @accountId, @payeScheme
GO

COMMIT TRANSACTION CreateLevy

DROP FUNCTION PayrollYear
GO

DROP FUNCTION PayrollMonth
GO

