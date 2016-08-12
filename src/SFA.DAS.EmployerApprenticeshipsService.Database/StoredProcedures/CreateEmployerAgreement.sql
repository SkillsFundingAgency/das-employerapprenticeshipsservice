CREATE PROCEDURE [dbo].[CreateEmployerAgreement]
	@legalEntityId BIGINT,
	@employerAgreementId BIGINT OUTPUT
AS
BEGIN	
	DECLARE @templateId INT
	
	SELECT TOP 1 @templateId = Id FROM [dbo].[EmployerAgreementTemplate] ORDER BY Id ASC;

	INSERT INTO [dbo].[EmployerAgreement](LegalEntityId, TemplateId, StatusId) 
	VALUES (@legalEntityId, @templateId, 1)

	SELECT @employerAgreementId = SCOPE_IDENTITY();
END