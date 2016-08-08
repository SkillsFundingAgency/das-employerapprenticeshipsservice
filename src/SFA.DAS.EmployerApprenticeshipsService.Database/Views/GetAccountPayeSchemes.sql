CREATE VIEW [dbo].[GetAccountPayeSchemes]
AS 
SELECT p.Ref AS EmpRef,
	p.AccountId,
	a.Name AS AccountName
FROM [dbo].[Paye] p
	JOIN [dbo].[Account] a
		ON a.Id = p.AccountId
