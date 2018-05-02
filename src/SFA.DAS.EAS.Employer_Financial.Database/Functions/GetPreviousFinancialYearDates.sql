CREATE FUNCTION [employer_financial].[GetPreviousFinancialYearDates] 
(
	@asOfDate DATETIME = NULL
)
RETURNS @t TABLE (YearStart DATETIME, YearEnd DATETIME)
AS
BEGIN
	SET @asOfDate = ISNULL(@asOfDate, GETDATE())

	DECLARE @asOfYear INT = DATEPART(year, @asOfDate)
	DECLARE @asOfMonth INT = DATEPART(month, @asOfDate)
	DECLARE @asOfDay INT = DATEPART(day, @asOfDate)

	IF(@asOfMonth = 4 AND @asOfDay >= 20 OR @asOfMonth > 4)
		INSERT INTO @t VALUES (DATEFROMPARTS(@asOfYear - 1, 4, 20), DATEFROMPARTS(@asOfYear, 4, 20))
	ELSE
		INSERT INTO @t VALUES (DATEFROMPARTS(@asOfYear - 2, 4, 20), DATEFROMPARTS(@asOfYear - 1, 4, 20))
	RETURN
END