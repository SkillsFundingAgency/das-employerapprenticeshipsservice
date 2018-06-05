CREATE PROCEDURE [employer_financial].[GetLatestPeriodEnd]
	
AS
SELECT top 1 
      [PeriodEndId] 
      ,[CalendarPeriodMonth]
      ,[CalendarPeriodYear]
      ,[AccountDataValidAt]
      ,[CommitmentDataValidAt]
      ,[CompletionDateTime]
      ,[PaymentsForPeriod]
  FROM [employer_financial].[PeriodEnd]
  ORDER BY 
	completiondatetime DESC

