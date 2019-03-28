--todo: convert in sp, and have scripts to run sps to set up levy & payments for a certain scenario
-- could possibly be reused by specflow
-- (or we could create a c sharp console app to set up the data <- probably better than this approach, and could reuse code in specflow tests!)
-- add functions to db, so can have params at top?
-- generate correct number of english fractions
-- levydeclared in transactionline are wrong: why? need to reset ytd in payroll month 1
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

DECLARE @accountId BIGINT                = 1
DECLARE @payeScheme NVARCHAR(50)         = '222/ZZ00002'
DECLARE @monthlyLevy DECIMAL(18, 4)      = 1000
DECLARE @currentYearLevy DECIMAL(18, 4)  = 10000
-- last levy will be created in this month (last payroll month will be 1 month before)
DECLARE @toDate DATETIME2                = GETDATE() -- DATEADD(month,2,GETDATE())
DECLARE @numberOfMonthsToCreate INT      = 25

DECLARE @levyDecByMonth TABLE (monthBeforeToDate INT, amount DECIMAL(18, 4), createMonth DATETIME, payrollMonth DATETIME, payrollYear VARCHAR(5))

declare @firstPayrollMonth datetime = DATEADD(month,-@numberOfMonthsToCreate+1-1,@toDate)
select @firstPayrollMonth

declare @firstPayrollYear VARCHAR(5) = dbo.PayrollYear(@firstPayrollMonth)
select @firstPayrollYear

-- generate same levy per month
insert into @levyDecByMonth
--SELECT TOP (@numberOfMonthsToCreate) monthBeforeToDate = -1+ROW_NUMBER() OVER (ORDER BY [object_id]), 1000 FROM sys.all_objects ORDER BY monthBeforeToDate;
SELECT TOP (@numberOfMonthsToCreate)
			monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 
			(case
			when dbo.PayrollYear(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate)) = @firstPayrollYear 
				THEN @monthlyLevy*row_number() over (order by (select NULL))
			ELSE
				@monthlyLevy*dbo.PayrollMonth(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate))
			END),
			DATEADD(month,/*monthBeforeToDate*/ -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate),
			DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate),
			dbo.PayrollYear(DATEADD(month,/*monthBeforeToDate*/ -1-@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]),@toDate))
FROM sys.all_objects
ORDER BY monthBeforeToDate;

-- to have different levy per month, set individual rows
--update @levyDecByMonth set amount = 10 where monthBeforeToDate = 0
--update @levyDecByMonth set amount = 10 where monthBeforeToDate = -1

select * from @levyDecByMonth

DECLARE @currentYear INT = DATEPART(year, @toDate)
DECLARE @currentMonth INT = DATEPART(month, @toDate)
DECLARE @currentDay INT = DATEPART(day, @toDate)

DECLARE @previousLevyYearStart VARCHAR(4)
DECLARE @previousLevyYearEnd VARCHAR(4)

IF(@currentMonth = 4 AND @currentDay >= 20 OR @currentMonth > 4)
	BEGIN
    SET @previousLevyYearStart = @currentYear - 1
    SET @previousLevyYearEnd = @currentYear
	END	
ELSE
	BEGIN
    SET @previousLevyYearStart = @currentYear - 2
    SET @previousLevyYearEnd = @currentYear - 1
	END

DECLARE @payrollYear VARCHAR(5) = (SELECT RIGHT(CONVERT(VARCHAR(5), @previousLevyYearStart, 1), 2)) + '-' + (SELECT RIGHT(CONVERT(VARCHAR(4), @previousLevyYearEnd, 1), 2))

INSERT INTO employer_financial.EnglishFraction (DateCalculated, Amount, EmpRef, DateCreated)
VALUES
(@previousLevyYearStart + '-06-10 07:12:28.060', 1.000, @payeScheme, @previousLevyYearStart + '-06-12 07:12:28.060'),
(@previousLevyYearStart + '-09-10 07:12:28.060', 1.000, @payeScheme, @previousLevyYearStart + '-09-12 07:12:28.060'),
(@previousLevyYearStart + '-12-10 07:12:28.060', 1.000, @payeScheme, @previousLevyYearStart + '-06-12 07:12:28.060'),
(@previousLevyYearEnd + '-03-10 07:12:28.060', 1.000, @payeScheme, @previousLevyYearEnd + '-03-12 07:12:28.060')

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

EXEC employer_financial.processdeclarationstransactions @accountId, @payeScheme

go

COMMIT TRANSACTION CreateLevy

drop FUNCTION PayrollYear
go

drop FUNCTION PayrollMonth
go

