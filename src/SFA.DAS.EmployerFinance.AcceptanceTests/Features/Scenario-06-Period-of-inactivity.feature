﻿Feature: HMRC-Scenario-06-Period-of-inactivity

Scenario: Inactivity-period

	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 999000601   | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 999000602   | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 999000603   | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |
		| 999000604   | 40000      | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |		
		| 999000605   | 47500      | 17-18        | 7             | 1                | 2017-11-15     | 2017-11-23  |
		| 999000606   | 57500      | 17-18        | 8             | 1                | 2017-12-15     | 2017-12-23  |		
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to the specified created date
	Then we should see a level 1 screen with a balance of 63250 on the 12/2017
	And we should see a level 1 screen with a total levy of 8250 on the 11/2017
	And we should see a level 2 screen with a levy declared of 7500 on the 11/2017
	And we should see a level 2 screen with a top up of 750 on the 11/2017
