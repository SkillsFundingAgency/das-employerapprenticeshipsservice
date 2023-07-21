CREATE PROCEDURE [employer_account].[GetAccountHistory]	
	@accountId BIGINT
	
AS
select 
	history.AccountId, history.PayeRef, history.AddedDate as DateAdded ,history.RemovedDate as DateRemoved
from 
	[employer_account].[AccountHistory] history
where 
	history.AccountId = @accountId
