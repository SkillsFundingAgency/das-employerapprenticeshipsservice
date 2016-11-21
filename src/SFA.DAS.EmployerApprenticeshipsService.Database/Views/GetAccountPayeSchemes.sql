CREATE VIEW [account].[GetAccountPayeSchemes]
AS 
SELECT p.Ref AS EmpRef,
	ah.AccountId,
	acc.HashedId
FROM 
	[account].[Paye] p
inner join 
	[account].[AccountHistory] ah on ah.PayeRef = p.Ref and ah.RemovedDate is null
inner join 
	[account].[Account] acc on acc.Id = ah.AccountId

