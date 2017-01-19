CREATE PROCEDURE [employer_account].[CreateAccountEmployerAgreement]
	@accountId int = 0,
	@employerAgreementId int
AS
	INSERT INTO [employer_account].[AccountEmployerAgreement](AccountId, EmployerAgreementId) VALUES (@accountId, @employerAgreementId)
