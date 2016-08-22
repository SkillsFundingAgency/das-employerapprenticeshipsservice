CREATE VIEW [account].[GetAccountPayeSchemes]
AS 
SELECT p.Ref AS EmpRef,
	p.AccountId,
	l.Name AS LegalEntityName,
	l.Id as LegalEntityId
FROM 
	[account].[Paye] p
inner join 
	[account].[LegalEntity] l on l.Id = p.LegalEntityId
