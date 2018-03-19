CREATE PROCEDURE [employer_financial].[GetAllPeriodEnds]	
AS
	SELECT [PeriodEndId] AS Id
      ,[CalendarPeriodMonth]
      ,[CalendarPeriodYear]
      ,[AccountDataValidAt]
      ,[CommitmentDataValidAt]
      ,[CompletionDateTime]
      ,[PaymentsForPeriod]
  FROM [employer_financial].[PeriodEnd]
