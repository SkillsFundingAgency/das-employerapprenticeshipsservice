CREATE PROCEDURE [employer_financial].[GetAccountBalance_ByAccountIds]
	@accountIds [employer_financial].[AccountIds] Readonly
AS
	select 
		AccountId,
		Sum(Amount) Balance 
	from 
		[employer_financial].TransactionLine 
	Group by AccountId

