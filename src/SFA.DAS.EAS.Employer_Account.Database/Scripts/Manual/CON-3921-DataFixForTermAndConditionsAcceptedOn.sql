-- Update TermsAndConditionsAcceptedOn for all users created afer 28th July 2021
	 Update U 
     Set U.TermAndConditionsAcceptedOn = T.CreatedDate
	 From [employer_account].[User] U
     Inner join (
			Select UserId, CreatedDate from (
				Select *, ROW_NUMBER() over (partition by M.UserId order by M.CreatedDate) As Rank2 
				from [employer_account].[Membership] M
			) Result 
			Where Result.Rank2 = 1
			and CreatedDate >= CONVERT(DATETIME,'28/07/2021',103) --dd/MM/yyyy
		) T on U.Id = T.UserId