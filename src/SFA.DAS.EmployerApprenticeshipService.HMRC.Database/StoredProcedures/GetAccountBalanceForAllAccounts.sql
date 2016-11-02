CREATE PROCEDURE [levy].[GetAccountBalanceForAllAccounts]
	
AS
	select 
		AccountId,
		Sum(Amount) Balance 
	from 
		levy.TransactionLine 
	Group by AccountId

