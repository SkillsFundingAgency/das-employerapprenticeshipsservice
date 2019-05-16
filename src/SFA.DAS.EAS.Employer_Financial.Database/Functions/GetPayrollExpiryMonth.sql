CREATE FUNCTION [employer_financial].[GetPayrollExpiryMonth]
(
	@currentDate DATETIME = NULL,
	@expiryPeriod INT = NULL
)
RETURNS @monthAfterPayrollExpiry TABLE( monthPortion INT, yearPortion INT  )
AS
BEGIN
	--conversion of inputs
	DECLARE @currentPayrollStartYear INT = DATEPART(YEAR, @currentDate)
	DECLARE @currentPayrollMonth INT = DATEPART(MONTH, @currentDate)
	DECLARE @expiryPeriodYearsPortion INT = FLOOR(@expiryPeriod / 12)
	DECLARE @expiryPeriodMonthsPortion INT = @expiryPeriod % 12

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
	IF(@expiryPayrollMonth < -1)
	BEGIN
		SET @expiryPayrollMonth = @expiryPayrollMonth + 12
		SET @expiryPayrollStartYear = @expiryPayrollStartYear - 1
	END
	INSERT INTO @monthAfterPayrollExpiry VALUES(@expiryPayrollMonth, @expiryPayrollStartYear)
	RETURN
END