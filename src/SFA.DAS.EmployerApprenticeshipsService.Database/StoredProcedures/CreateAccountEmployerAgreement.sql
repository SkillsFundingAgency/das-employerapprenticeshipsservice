CREATE PROCEDURE [dbo].[CreateAccountEmployerAgreement]
	@accountId int = 0,
	@employerAgreementId int
AS
	INSERT INTO [dbo].[AccountEmployerAgreement](AccountId, EmployerAgreementId) VALUES (@accountId, @employerAgreementId)
