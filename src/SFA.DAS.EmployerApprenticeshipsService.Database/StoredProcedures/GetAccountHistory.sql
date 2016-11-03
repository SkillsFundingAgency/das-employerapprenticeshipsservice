CREATE PROCEDURE [account].[GetAccountHistory]	
	@accountId BIGINT
	
AS
select 
	history.AccountId, history.PayeRef, history.AddedDate as DateAdded ,history.RemovedDate as DateRemoved
from 
	[account].[AccountHistory] history
where 
	history.AccountId = @accountId
