CREATE PROCEDURE [employer_account].[RemoveMembership]
(
	@UserId BIGINT,
	@AccountId BIGINT
)
AS
BEGIN

	DELETE FROM [employer_account].[Membership]
	WHERE AccountId = @AccountId AND UserId = @UserId;

	DELETE s
	FROM [employer_account].UserLegalEntitySettings s
	join [employer_account].AccountEmployerAgreement a on a.EmployerAgreementId = s.EmployerAgreementId
	where s.UserId = @UserId and a.AccountId = @AccountId

END
