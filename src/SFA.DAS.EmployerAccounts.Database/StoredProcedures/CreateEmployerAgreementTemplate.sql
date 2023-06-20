CREATE PROCEDURE [employer_account].[CreateEmployerAgreementTemplate]
	@PartialViewName NVARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [employer_account].[EmployerAgreementTemplate](PartialViewName, CreatedDate) 
	VALUES (@PartialViewName, GETDATE());
END