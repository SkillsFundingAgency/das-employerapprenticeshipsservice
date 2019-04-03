CREATE FUNCTION [employer_financial].[GetPayrollExpiryMonth]
(
	@currentDate DATETIME = NULL,
	@expiryPeriod INT = NULL
)
RETURNS @monthOfPayrollExpiry TABLE( monthPortion int, yearPortion int  )
AS
BEGIN
	--conversion of inputs
	DECLARE @currentPayrollStartYear int = DATEPART(year, @currentDate)
	DECLARE @currentPayrollMonth int = DATEPART(month, @currentDate)
	DECLARE @expiryPeriodYearsPortion int = FLOOR(@expiryPeriod / 12)
	DECLARE @expiryPeriodMonthsPortion int = @expiryPeriod % 12

	SET @currentPayrollMonth = @currentPayrollMonth - 4
	IF(@currentPayrollMonth <= 0)
	BEGIN
		SET @currentPayrollMonth = @currentPayrollMonth + 12
		SET @currentPayrollStartYear = @currentPayrollStartYear - 1
	END		

	SET @currentPayrollStartYear = @currentPayrollStartYear - 2000

	DECLARE @expiryPayrollStartYear INT = @currentPayrollStartYear - @expiryPeriodYearsPortion

	--calculate month
	DECLARE @expiryPayrollMonth INT = @currentPayrollMonth - @expiryPeriodMonthsPortion

	--if it's last year adjust it and the year to represent the correct month accordingly
	if(@expiryPayrollMonth < -1)
	BEGIN
		SET @expiryPayrollMonth = @expiryPayrollMonth + 12
		SET @expiryPayrollStartYear = @expiryPayrollStartYear - 1
	END
	INSERT INTO @monthOfPayrollExpiry VALUES(@expiryPayrollMonth, @expiryPayrollStartYear)
	RETURN
END