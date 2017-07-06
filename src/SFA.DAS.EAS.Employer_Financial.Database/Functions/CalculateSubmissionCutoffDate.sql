CREATE FUNCTION [employer_financial].[CalculateSubmissionCutoffDate]
(
	@payrollMonth INT,
	@payrollYear NVARCHAR(10)
)
RETURNS DATE
AS
BEGIN
	DECLARE @year INT
	SET @year = 2000 + CAST(LEFT(@payrollYear, 2) AS INT)
	RETURN DATEADD(month, 4, DATEFROMPARTS(@year, @payrollMonth, 20))
END
