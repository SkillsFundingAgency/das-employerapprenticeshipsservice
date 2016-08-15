CREATE PROCEDURE [dbo].[CreateEmployerAgreementTemplate]
	@ref NVARCHAR(50),
	@text NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[EmployerAgreementTemplate]([Text], CreatedDate, Ref) 
	VALUES (@text, GETDATE(), @ref);
END