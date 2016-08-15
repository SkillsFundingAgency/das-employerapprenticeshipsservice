CREATE PROCEDURE [dbo].[ReleaseEmployerAgreementTemplate]
	@templateId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @lastTemplateId BIGINT;
	DECLARE @expiryDays INT;

	--get the current template id
	SELECT TOP 1 @lastTemplateId = Id 
	FROM [dbo].[EmployerAgreementTemplate]
	WHERE ReleasedDate IS NOT NULL
	ORDER BY ReleasedDate DESC; 
	
	SELECT @expiryDays = ExpiryDays
	FROM [dbo].[EmployerAgreementTemplate]
	WHERE Id = @lastTemplateId; 

	--set expirydate for agreements using previous template
	UPDATE [dbo].[EmployerAgreement]
	SET ExpiredDate = GETDATE()+@expiryDays
	WHERE TemplateId = @lastTemplateId;

	--insert new record for each legalentity with new template
	INSERT INTO [dbo].[EmployerAgreement](LegalEntityId, TemplateId, StatusId)
	SELECT LegalEntityId, @templateId, 1 
	FROM [dbo].[EmployerAgreement]
	WHERE TemplateId = @lastTemplateId;

	--insert into AccountEmployerAgreement with new Ids
	INSERT INTO [dbo].[AccountEmployerAgreement]
	SELECT aea.AccountId, t.Id
	FROM [dbo].[AccountEmployerAgreement] aea
		JOIN [dbo].[EmployerAgreement] ea
			ON ea.Id = aea.EmployerAgreementId
		JOIN 
		(
			SELECT *
			FROM [dbo].[EmployerAgreement]
			WHERE TemplateId = @templateId
		) t
		ON t.LegalEntityId = ea.LegalEntityId
	WHERE ea.TemplateId = @lastTemplateId;

	UPDATE [dbo].[EmployerAgreementTemplate]
	SET [ReleasedDate] = GETDATE()
	WHERE Id = @templateId;
END
