CREATE VIEW [employer_account].[GetAccountPayeSchemes]
AS 
SELECT p.Ref AS EmpRef,
	ah.AccountId,
	acc.HashedId
FROM 
	[employer_account].[Paye] p
inner join 
	[employer_account].[AccountHistory] ah on ah.PayeRef = p.Ref and ah.RemovedDate is null
inner join 
	[employer_account].[Account] acc on acc.Id = ah.AccountId

