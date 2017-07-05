CREATE FUNCTION [employer_financial].[CalculateSubmissionCutoffDate]
(
	@payrollMonth INT
)
RETURNS DATE
AS
BEGIN
	RETURN DATEADD(month, 4, DATEFROMPARTS(DatePart(yyyy,GETDATE()), @payrollMonth, 20))
END
