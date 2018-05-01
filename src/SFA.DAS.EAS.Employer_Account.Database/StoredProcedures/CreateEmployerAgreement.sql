﻿CREATE PROCEDURE [employer_account].[CreateEmployerAgreement]
	@legalEntityId BIGINT,
	@accountId BIGINT,
	@employerAgreementId BIGINT OUTPUT
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @templateId INT
	
	SELECT TOP 1 @templateId = Id
	FROM [employer_account].[EmployerAgreementTemplate]
	ORDER BY VersionNumber DESC

	INSERT INTO [employer_account].[EmployerAgreement] (LegalEntityId, AccountId, TemplateId, StatusId) 
	VALUES (@legalEntityId, @accountId, @templateId, 1)

	SELECT @employerAgreementId = SCOPE_IDENTITY()
END