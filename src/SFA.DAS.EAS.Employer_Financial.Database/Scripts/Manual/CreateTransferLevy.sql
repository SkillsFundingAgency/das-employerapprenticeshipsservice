BEGIN TRANSACTION

DECLARE @accountId BIGINT
DECLARE @payeScheme NVARCHAR(50)
DECLARE @yearlyLevy DECIMAL(18, 4)
DECLARE @currentYearLevy DECIMAL(18, 4)

SET @accountId = 0
SET @payeScheme = 'XXX'
SET @yearlyLevy = 120000
SET @currentYearLevy = 10000

DECLARE @montlyLevy DECIMAL(18, 4) = @yearlyLevy / 12
DECLARE @currentYear INT = DATEPART(year, GETDATE())
DECLARE @currentMonth INT = DATEPART(month, GETDATE())
DECLARE @currentDay INT = DATEPART(day, GETDATE())

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

INSERT INTO employer_financial.levydeclaration (AccountId,empref,levydueytd,levyallowanceforyear,submissiondate,submissionid,payrollyear,payrollmonth,createddate,hmrcsubmissionid)
VALUES
(@accountId, @payeScheme, @montlyLevy,	    1500.0000, @previousLevyYearStart + '-05-18 07:12:28.060',  @maxSubmissionId + 1, @payrollYear, 1, @previousLevyYearStart + '-05-20 07:12:28.060',  @maxSubmissionId + 1),
(@accountId, @payeScheme, @montlyLevy * 2,  1500.0000, @previousLevyYearStart + '-06-18 07:12:28.060',  @maxSubmissionId + 2, @payrollYear, 2, @previousLevyYearStart +'-06-20 07:12:28.060',  @maxSubmissionId + 2),
(@accountId, @payeScheme, @montlyLevy * 3,  1500.0000, @previousLevyYearStart + '-07-18 07:12:28.060',  @maxSubmissionId + 3, @payrollYear, 3, @previousLevyYearStart +'-07-20 07:12:28.060',  @maxSubmissionId + 3),
(@accountId, @payeScheme, @montlyLevy * 4,  1500.0000, @previousLevyYearStart + '-08-18 07:12:28.060',  @maxSubmissionId + 4, @payrollYear, 4, @previousLevyYearStart +'-08-20 07:12:28.060',  @maxSubmissionId + 4),
(@accountId, @payeScheme, @montlyLevy * 5,  1500.0000, @previousLevyYearStart + '-09-18 07:12:28.060',  @maxSubmissionId + 5, @payrollYear, 5, @previousLevyYearStart +'-09-20 07:12:28.060',  @maxSubmissionId + 5),
(@accountId, @payeScheme, @montlyLevy * 6,  1500.0000, @previousLevyYearStart + '-10-18 07:12:28.060',  @maxSubmissionId + 6, @payrollYear, 6, @previousLevyYearStart +'-10-20 07:12:28.060',  @maxSubmissionId + 6),
(@accountId, @payeScheme, @montlyLevy * 7,  1500.0000, @previousLevyYearStart + '-11-18 07:12:28.060',  @maxSubmissionId + 7, @payrollYear, 7, @previousLevyYearStart +'-11-20 07:12:28.060',  @maxSubmissionId + 7),
(@accountId, @payeScheme, @montlyLevy * 8,  1500.0000, @previousLevyYearStart + '-12-18 07:12:28.060',  @maxSubmissionId + 8, @payrollYear, 8, @previousLevyYearStart +'-12-20 07:12:28.060',  @maxSubmissionId + 8),
(@accountId, @payeScheme, @montlyLevy * 9,  1500.0000, @previousLevyYearEnd + '-01-18 07:12:28.060',  @maxSubmissionId + 9, @payrollYear, 9, @previousLevyYearEnd + '-01-20 07:12:28.060',  @maxSubmissionId + 9),
(@accountId, @payeScheme, @montlyLevy * 10, 1500.0000, @previousLevyYearEnd + '-02-18 07:12:28.060',  @maxSubmissionId + 10, @payrollYear, 10, @previousLevyYearEnd + '-02-20 07:12:28.060',  @maxSubmissionId + 10),
(@accountId, @payeScheme, @montlyLevy * 11, 1500.0000, @previousLevyYearEnd + '-03-18 07:12:28.060',  @maxSubmissionId + 11, @payrollYear, 11, @previousLevyYearEnd + '-03-20 07:12:28.060',  @maxSubmissionId + 11),
(@accountId, @payeScheme, @montlyLevy * 12, 1500.0000, @previousLevyYearEnd + '-04-18 07:12:28.060',  @maxSubmissionId + 12, @payrollYear, 12, @previousLevyYearEnd + '-04-20 07:12:28.060',  @maxSubmissionId + 12)

IF(@currentMonth > 4)
BEGIN
    DECLARE @nextPayrollYear VARCHAR(5) = (SELECT RIGHT(CONVERT(VARCHAR(5), @previousLevyYearEnd, 1), 2)) + '-' + (SELECT RIGHT(CONVERT(VARCHAR(4), @previousLevyYearEnd + 1, 1), 2))
    
    INSERT INTO employer_financial.levydeclaration (AccountId,empref,levydueytd,levyallowanceforyear,submissiondate,submissionid,payrollyear,payrollmonth,createddate,hmrcsubmissionid)
    VALUES (@accountId, @payeScheme, @currentYearLevy, 1500.0000, @previousLevyYearEnd + '-05-18 07:12:28.060',  @maxSubmissionId + 13, @nextPayrollYear, 1, @previousLevyYearEnd + '-05-20 07:12:28.060',  @maxSubmissionId + 13)
END

EXEC employer_financial.processdeclarationstransactions @accountId, @payeScheme

COMMIT TRANSACTION