/*
	Returns the start and end dates for the financial year prior to the specified date.
	For example, 
		if @asOfDate is 10-Jul-2018 then previous financial year will be 20-Apr-2017 to 20-Apr-2018.
		if @asOfDate is 10-Apr-2018 then previous financial year will be 20-Apr-2016 to 20-Apr-2017.
*/
CREATE FUNCTION [employer_financial].[GetPreviousFinancialYearDates] 
(
	@asOfDate DateTime = null
)
RETURNS @t TABLE (YearStart DateTime, YearEnd DateTime)
AS
BEGIN
	DECLARE @asOfYear as int;
	SELECT @asOfYear = DATEPART(year, IsNull(@asOfDate, GETDATE()));

	IF(DATEPART(month, @asOfDate) > 4)
		INSERT INTO @t	Values (DATEFROMPARTS(@asOfYear - 1, 4, 20), DATEFROMPARTS(@asOfYear, 4, 20))
	ELSE
		INSERT INTO @t	Values (DATEFROMPARTS(@asOfYear - 2, 4, 20), DATEFROMPARTS(@asOfYear - 1, 4, 20))

	RETURN
END;
