CREATE PROCEDURE [account].[CreateAccountEmployerAgreement]
	@accountId int = 0,
	@employerAgreementId int
AS
	INSERT INTO [account].[AccountEmployerAgreement](AccountId, EmployerAgreementId) VALUES (@accountId, @employerAgreementId)
