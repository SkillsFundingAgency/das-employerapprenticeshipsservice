CREATE PROCEDURE [levy].[ProcessDeclarationsTransactions]
AS


--Add the topup from the declaration
INSERT INTO levy.LevyDeclarationTopup
	select mainUpdate.* from
	(
select 
		x.AccountId,
		GetDate() as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.EnglishFraction) * y.TopUpPercentage as Amount
	FROM 
		[levy].[GetLevyDeclarations] x
	
	left join 
		[levy].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
	where
		y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from levy.LevyDeclaration
	EXCEPT
		select SubmissionId from levy.LevyDeclarationTopup
	) dervx on dervx.submissionId = mainUpdate.SubmissionId


-- Create Declarations


INSERT INTO levy.TransactionLine
select mainUpdate.* from
	(
	select 
			x.AccountId,
			GetDate() as DateCreated,
			x.SubmissionId as SubmissionId,
			x.SubmissionDate as TransactionDate,
			1 as TransactionType,
			((y.LevyDueYTD - isnull(LAG(y.LevyDueYTD) OVER(Partition by y.empref order by y.SubmissionDate asc, y.submissionId),0)) * y.EnglishFraction) + ldt.amount as Amount,
			x.EmpRef as EmpRef,
			null as PeriodEnd,
			null as UkPrn
		FROM 
			[levy].[GetLevyDeclarations] x
		inner join
			[levy].[LevyDeclarationTopup] ldt on ldt.submissionId = x.submissionId
		left join 
			[levy].[GetLevyDeclarations] y on y.lastsubmission = 1 and y.id = x.id
		where
			y.LevyDueYTD is not null
	) mainUpdate
	inner join (
		select submissionId from levy.LevyDeclaration
	EXCEPT
		select SubmissionId from levy.TransactionLine where TransactionType = 1
	) dervx on dervx.submissionId = mainUpdate.SubmissionId