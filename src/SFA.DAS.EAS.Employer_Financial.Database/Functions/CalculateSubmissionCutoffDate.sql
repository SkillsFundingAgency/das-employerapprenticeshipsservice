CREATE FUNCTION [employer_financial].[CalculateSubmissionCutoffDate]
(
	@PayrollMonth INT,
	@PayrollYear NVARCHAR(10)
)
RETURNS DATE
AS
BEGIN
	DECLARE @year INT
	SET @year = 2000 + CAST(LEFT(@PayrollYear, 2) AS INT)
	RETURN DATEADD(month, 4, DATEFROMPARTS(@year, @PayrollMonth, 20))
END
