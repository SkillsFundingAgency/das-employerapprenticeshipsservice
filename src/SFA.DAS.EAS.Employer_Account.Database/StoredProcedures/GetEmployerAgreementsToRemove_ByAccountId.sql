CREATE PROCEDURE [employer_account].[GetEmployerAgreementsToRemove_ByAccountId]
	@AccountId BIGINT
	
AS
	SELECT 
		ea.Id,
		le.Name AS Name,
		case 
			when ea.StatusId = 1 then 1 
			else 0 end 
		as CanBeRemoved,
		acc.HashedId as HashedAccountId
		
	FROM [employer_account].[LegalEntity] le
		JOIN [employer_account].[EmployerAgreement] ea
			ON ea.LegalEntityId = le.Id
		JOIN [employer_account].[AccountEmployerAgreement] aea
			ON aea.[EmployerAgreementId] = ea.Id
		JOIN [employer_account].[EmployerAgreementTemplate] eat
			ON eat.Id = ea.TemplateId
		join [employer_account].Account acc
			on	acc.Id = aea.AccountId
	WHERE ea.AccountId = @AccountId
	And ea.StatusId <> 5