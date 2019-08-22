CREATE PROCEDURE employer_financial.GetPayeForAccount_ByRef
	@AccountId BIGINT,
	@Ref NVARCHAR(16)
AS
Select TOP 1
	paye.Ref,
    paye.Name
from employer_financial.AccountPaye paye
WHERE paye.AccountId = @AccountId AND paye.EmpRef = @Ref