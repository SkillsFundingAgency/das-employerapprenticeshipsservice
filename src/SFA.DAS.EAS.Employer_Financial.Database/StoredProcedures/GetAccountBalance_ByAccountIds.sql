CREATE PROCEDURE [employer_financial].[GetAccountBalance_ByAccountIds]
	@accountIds [employer_financial].[AccountIds] Readonly
AS
	select 
		tl.AccountId,
		Sum(Amount) Balance ,
		MAX(dervx.IsLevyPayer) IsLevyPayer
	from 
		[employer_financial].TransactionLine tl
	inner join 
	(	select 
			case when SUM(LevyDueYTD) > 0 then 1
			else
				case when MAX(t.IsLevyPayer) is not null 
				then MAX(t.IsLevyPayer) 
				else 0 end end
			as IsLevyPayer, 
			AccountId 
		from employer_financial.LevyDeclaration ld
			OUTER APPLY
			(
				SELECT TOP 1 IsLevyPayer
				FROM [employer_financial].LevyOverride lo
				WHERE lo.AccountId = ld.AccountId 
				ORDER BY [DateAdded] DESC
			) t
		group by accountid
	) dervx on dervx.AccountId = tl.AccountId
	inner join 
		@accountIds acc on acc.AccountId = tl.AccountId
	Group by tl.AccountId

