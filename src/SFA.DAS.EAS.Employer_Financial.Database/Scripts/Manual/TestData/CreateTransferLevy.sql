--todo: convert in sp, and have scripts to run sps to set up levy & payments for a certain scenario
-- could possibly be reused by specflow (or it could use c sharp to programatically set <- probably better than this approach!)

BEGIN TRANSACTION

DECLARE @accountId BIGINT                = 1
DECLARE @payeScheme NVARCHAR(50)         = '222/ZZ00002'
DECLARE @monthlyLevy DECIMAL(18, 4)      = 1000
DECLARE @currentYearLevy DECIMAL(18, 4)  = 10000
DECLARE @toDate DATETIME2                = GETDATE()
DECLARE @numberOfMonthsToCreate INT      = 25

DECLARE @levyDecByMonth TABLE (monthBeforeToDate INT, amount DECIMAL(18, 4))

-- generate same levy per month
insert into @levyDecByMonth
--SELECT TOP (@numberOfMonthsToCreate) monthBeforeToDate = -1+ROW_NUMBER() OVER (ORDER BY [object_id]), 1000 FROM sys.all_objects ORDER BY monthBeforeToDate;
SELECT TOP (@numberOfMonthsToCreate) monthBeforeToDate = -@numberOfMonthsToCreate+ROW_NUMBER() OVER (ORDER BY [object_id]), 1000 FROM sys.all_objects ORDER BY monthBeforeToDate;

-- to have different levy per month, set individual rows
--update @levyDecByMonth set amount = 10 where monthBeforeToDate = 0
--update @levyDecByMonth set amount = 10 where monthBeforeToDate = -1

--select * from @levyDecByMonth

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

--todo payroll year
--todo use monthBeforeToDate, rather than row_number?
INSERT INTO employer_financial.levydeclaration (AccountId,empref,levydueytd,levyallowanceforyear,submissiondate,submissionid,payrollyear,payrollmonth,createddate,hmrcsubmissionid)
select @accountId, @payeScheme, amount, 1500.0000, DATEADD(month, monthBeforeToDate, @baselineSubmissionDate), @maxSubmissionId + row_number() over (order by (select NULL)), @payrollYear, 1, DATEADD(month, monthBeforeToDate, @baselineCreatedDate), @maxSubmissionId + row_number() over (order by (select NULL))
from @levyDecByMonth

--VALUES
--(@accountId, @payeScheme, @monthlyLevy,	    1500.0000, @previousLevyYearStart + '-05-18 07:12:28.060',  @maxSubmissionId + 1, @payrollYear, 1, @previousLevyYearStart + '-05-20 07:12:28.060',  @maxSubmissionId + 1),
--(@accountId, @payeScheme, @monthlyLevy * 2,  1500.0000, @previousLevyYearStart + '-06-18 07:12:28.060',  @maxSubmissionId + 2, @payrollYear, 2, @previousLevyYearStart +'-06-20 07:12:28.060',  @maxSubmissionId + 2),
--(@accountId, @payeScheme, @monthlyLevy * 3,  1500.0000, @previousLevyYearStart + '-07-18 07:12:28.060',  @maxSubmissionId + 3, @payrollYear, 3, @previousLevyYearStart +'-07-20 07:12:28.060',  @maxSubmissionId + 3),
--(@accountId, @payeScheme, @monthlyLevy * 4,  1500.0000, @previousLevyYearStart + '-08-18 07:12:28.060',  @maxSubmissionId + 4, @payrollYear, 4, @previousLevyYearStart +'-08-20 07:12:28.060',  @maxSubmissionId + 4),
--(@accountId, @payeScheme, @monthlyLevy * 5,  1500.0000, @previousLevyYearStart + '-09-18 07:12:28.060',  @maxSubmissionId + 5, @payrollYear, 5, @previousLevyYearStart +'-09-20 07:12:28.060',  @maxSubmissionId + 5),
--(@accountId, @payeScheme, @monthlyLevy * 6,  1500.0000, @previousLevyYearStart + '-10-18 07:12:28.060',  @maxSubmissionId + 6, @payrollYear, 6, @previousLevyYearStart +'-10-20 07:12:28.060',  @maxSubmissionId + 6),
--(@accountId, @payeScheme, @monthlyLevy * 7,  1500.0000, @previousLevyYearStart + '-11-18 07:12:28.060',  @maxSubmissionId + 7, @payrollYear, 7, @previousLevyYearStart +'-11-20 07:12:28.060',  @maxSubmissionId + 7),
--(@accountId, @payeScheme, @monthlyLevy * 8,  1500.0000, @previousLevyYearStart + '-12-18 07:12:28.060',  @maxSubmissionId + 8, @payrollYear, 8, @previousLevyYearStart +'-12-20 07:12:28.060',  @maxSubmissionId + 8),
--(@accountId, @payeScheme, @monthlyLevy * 9,  1500.0000, @previousLevyYearEnd + '-01-18 07:12:28.060',  @maxSubmissionId + 9, @payrollYear, 9, @previousLevyYearEnd + '-01-20 07:12:28.060',  @maxSubmissionId + 9),
--(@accountId, @payeScheme, @monthlyLevy * 10, 1500.0000, @previousLevyYearEnd + '-02-18 07:12:28.060',  @maxSubmissionId + 10, @payrollYear, 10, @previousLevyYearEnd + '-02-20 07:12:28.060',  @maxSubmissionId + 10),
--(@accountId, @payeScheme, @monthlyLevy * 11, 1500.0000, @previousLevyYearEnd + '-03-18 07:12:28.060',  @maxSubmissionId + 11, @payrollYear, 11, @previousLevyYearEnd + '-03-20 07:12:28.060',  @maxSubmissionId + 11),
--(@accountId, @payeScheme, @monthlyLevy * 12, 1500.0000, @previousLevyYearEnd + '-04-18 07:12:28.060',  @maxSubmissionId + 12, @payrollYear, 12, @previousLevyYearEnd + '-04-20 07:12:28.060',  @maxSubmissionId + 12)

--IF(@currentMonth > 4)
--BEGIN
--    DECLARE @nextPayrollYear VARCHAR(5) = (SELECT RIGHT(CONVERT(VARCHAR(5), @previousLevyYearEnd, 1), 2)) + '-' + (SELECT RIGHT(CONVERT(VARCHAR(4), @previousLevyYearEnd + 1, 1), 2))
    
--    INSERT INTO employer_financial.levydeclaration (AccountId,empref,levydueytd,levyallowanceforyear,submissiondate,submissionid,payrollyear,payrollmonth,createddate,hmrcsubmissionid)
--    VALUES (@accountId, @payeScheme, @currentYearLevy, 1500.0000, @previousLevyYearEnd + '-05-18 07:12:28.060',  @maxSubmissionId + 13, @nextPayrollYear, 1, @previousLevyYearEnd + '-05-20 07:12:28.060',  @maxSubmissionId + 13)
--END

EXEC employer_financial.processdeclarationstransactions @accountId, @payeScheme

COMMIT TRANSACTION