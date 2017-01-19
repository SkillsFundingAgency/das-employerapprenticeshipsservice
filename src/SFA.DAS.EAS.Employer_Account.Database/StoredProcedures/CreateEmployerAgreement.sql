CREATE PROCEDURE [employer_account].[CreateEmployerAgreement]
	@legalEntityId BIGINT,
	@employerAgreementId BIGINT OUTPUT
AS
BEGIN	
	DECLARE @templateId INT
	
	SELECT TOP 1 @templateId = Id FROM [employer_account].[EmployerAgreementTemplate] ORDER BY Id ASC;

	INSERT INTO [employer_account].[EmployerAgreement](LegalEntityId, TemplateId, StatusId) 
	VALUES (@legalEntityId, @templateId, 1)

	SELECT @employerAgreementId = SCOPE_IDENTITY();
END