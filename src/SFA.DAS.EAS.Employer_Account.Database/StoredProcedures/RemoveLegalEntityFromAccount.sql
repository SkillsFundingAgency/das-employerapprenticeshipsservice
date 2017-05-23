CREATE PROCEDURE [employer_account].[RemoveLegalEntityFromAccount]
	@employerAgreementId BIGINT 
AS

UPDATE employer_account.EmployerAgreement 
	SET StatusId = 5 
where Id = @employerAgreementId 
