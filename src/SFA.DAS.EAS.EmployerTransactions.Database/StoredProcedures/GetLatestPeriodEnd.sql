CREATE PROCEDURE [employer_transactions].[GetLatestPeriodEnd]
	
AS
SELECT top 1 
      [PeriodEndId] AS Id
      ,[CalendarPeriodMonth]
      ,[CalendarPeriodYear]
      ,[AccountDataValidAt]
      ,[CommitmentDataValidAt]
      ,[CompletionDateTime]
      ,[PaymentsForPeriod]
  FROM [employer_transactions].[PeriodEnd]
  ORDER BY 
	completiondatetime DESC

