CREATE VIEW [dbo].[GetAccountPayeSchemes]
AS 
SELECT p.Ref AS EmpRef,
	p.AccountId,
	l.Name AS LegalEntityName
FROM 
	[dbo].[Paye] p
inner join 
	[dbo].[LegalEntity] l on l.Id = p.LegalEntityId
