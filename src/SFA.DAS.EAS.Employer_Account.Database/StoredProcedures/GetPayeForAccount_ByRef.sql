CREATE PROCEDURE employer_account.GetPayeForAccount_ByRef
	@HashedAccountId NVARCHAR(100),
	@Ref NVARCHAR(16)
AS
Select TOP 1
	paye.Ref,
    paye.Name,
	ah.AddedDate,
	ah.RemovedDate
from employer_account.Paye paye
INNER JOIN employer_account.AccountHistory ah ON ah.PayeRef = paye.Ref
INNER JOIN employer_account.Account a ON a.Id = ah.AccountId
WHERE a.HashedId = @HashedAccountId AND paye.Ref = @Ref
ORDER BY ah.Id DESC