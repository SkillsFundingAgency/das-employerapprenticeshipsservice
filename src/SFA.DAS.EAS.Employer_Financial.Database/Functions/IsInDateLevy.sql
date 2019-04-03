CREATE FUNCTION [employer_financial].[IsInDateLevy]
(
	@currentDate DATETIME = NULL,
	@expiryPeriod INT = NULL,
	@payrollYear nvarchar(5) = NULL,
	@payrollMonth int = NULL
)
RETURNS bit
AS
BEGIN
	DECLARE @monthOfPayrollExpiry TABLE(monthPortion int, yearPotion int)

	INSERT INTO @monthOfPayrollExpiry SELECT * FROM [employer_financial].[GetPayrollExpiryMonth](@currentDate, @expiryPeriod)

	DECLARE @expiryPayrollMonth INT = (SELECT monthPortion FROM @monthOfPayrollExpiry)
	DECLARE @expiryPayrollStartYear INT = (SELECT yearPotion FROM @monthOfPayrollExpiry)

	--actual checks
	DECLARE @payrollStartYear AS INT = CAST(LEFT(@payrollYear, 2) AS INT)

	--year is later, definitely in date
	IF(@payrollStartYear > @expiryPayrollStartYear) RETURN 1

	--year is same, month is later, in date
	IF(@payrollStartYear = @expiryPayrollStartYear AND @payrollMonth > @expiryPayrollMonth) RETURN 1

	--otherwise out of date
	RETURN 0
END