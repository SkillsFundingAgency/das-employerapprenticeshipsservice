-- Update TermsAndConditionsAcceptedOn for all users created afer 27th July 2021
	 Update [employer_account].[User] 
     Set TermAndConditionsAcceptedOn = GETDATE()
     where Id in (
			Select UserId from (
				Select *, ROW_NUMBER() over (partition by M.UserId order by M.CreatedDate) As Rank2 
				from [employer_account].[Membership] M
			) Result 
			Where Result.Rank2 = 1
			and CreatedDate >= CONVERT(DATETIME,'27/07/2021',103) --dd/MM/yyyy
		)