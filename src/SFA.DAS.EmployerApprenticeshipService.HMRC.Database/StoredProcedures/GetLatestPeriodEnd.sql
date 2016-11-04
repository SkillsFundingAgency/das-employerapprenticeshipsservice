CREATE PROCEDURE [levy].[GetLatestPeriodEnd]
	
AS
SELECT top 1 
      [PeriodEndId] AS Id
      ,[CalendarPeriodMonth]
      ,[CalendarPeriodYear]
      ,[AccountDataValidAt]
      ,[CommitmentDataValidAt]
      ,[CompletionDateTime]
      ,[PaymentsForPeriod]
  FROM [levy].[PeriodEnd]
  ORDER BY 
	completiondatetime DESC

