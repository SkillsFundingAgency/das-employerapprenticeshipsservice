 WITH duplicateRow as (SELECT Id, LegalEntityId, AccountId,  TemplateId, StatusId, row_number() 
 OVER(PARTITION BY LegalEntityId, AccountId,TemplateId ORDER BY SignedDate DESC) AS [rn]
 FROM [employer_account].[EmployerAgreement])

 DELETE FROM [employer_account].[EmployerAgreement]
 WHERE Id IN 
 ( 
	 SELECT Id FROM duplicateRow 
	 WHERE rn > 1
	 AND StatusId <> 5
 )