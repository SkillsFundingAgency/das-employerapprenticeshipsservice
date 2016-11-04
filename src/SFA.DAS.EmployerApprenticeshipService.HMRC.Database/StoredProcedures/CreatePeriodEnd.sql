CREATE PROCEDURE [levy].[CreatePeriodEnd]
	@PeriodEndId nvarchar(20),
	@CalendarPeriodMonth int,
	@CalendarPeriodYear int,
	@AccountDataValidAt datetime,
	@CommitmentDataValidAt datetime,
	@CompletionDateTime datetime,
	@PaymentsForPeriod nvarchar(250)
AS

INSERT INTO [levy].[PeriodEnd]
           ([PeriodEndId]
           ,[CalendarPeriodMonth]
           ,[CalendarPeriodYear]
           ,[AccountDataValidAt]
           ,[CommitmentDataValidAt]
           ,[CompletionDateTime]
           ,[PaymentsForPeriod])
     VALUES
           (@PeriodEndId
           ,@CalendarPeriodMonth
           ,@CalendarPeriodYear
           ,@AccountDataValidAt
           ,@CommitmentDataValidAt
           ,@CompletionDateTime
           ,@PaymentsForPeriod)