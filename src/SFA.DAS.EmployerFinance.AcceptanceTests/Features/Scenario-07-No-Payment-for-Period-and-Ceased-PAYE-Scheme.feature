﻿Feature: HMRC-Scenario-07-No-Payment-for-Period-and-Ceased-PAYE-Scheme

Scenario: Balance-should-remain-if-no-payment-occurs
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate | LevyAllowanceForFullYear | NoPaymentForPeriod | DateCeased |
		| 999000701   | 1000       | 17-18        | 1             | 1                | 2017-04-15     | 2017-04-23  | 15000                    |                    |            |
		| 999000702   | 2000       | 17-18        | 2             | 1                | 2017-05-15     | 2017-05-23  | 15000                    | true				|            |		
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to the specified created date
	Then we should see a level 1 screen with a balance of 1100 on the 06/2017
	

Scenario: Balance-should-not-be-affected-by-past-non-payment-months
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id           | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate | LevyAllowanceForFullYear | NoPaymentForPeriod | DateCeased |
		| 999000703    | 1000       | 17-18         | 1            | 1                | 2017-04-15     | 2017-04-23  | 15000                    |                    |            |
		| 999000704    | 0          | 17-18         | 2            | 1                | 2017-05-15     | 2017-05-23  | 15000                    | true               |            |
		| 999000705    | 2000       | 17-18         | 3            | 1                | 2017-06-15     | 2017-06-23  | 15000                    |                    |            |
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to the specified created date
	Then we should see a level 1 screen with a balance of 2200 on the 06/2017
