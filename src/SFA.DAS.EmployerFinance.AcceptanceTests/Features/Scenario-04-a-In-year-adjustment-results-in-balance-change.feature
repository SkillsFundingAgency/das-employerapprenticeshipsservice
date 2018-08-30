Feature: HMRC-Scenario-04a-In-year-adjustment-results-in-balance-change

Scenario: Sceanrio-01-Future-submission
	Given We have an account with id 25
	And Hmrc return the following submissions for paye scheme 123/ABC
		| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 999000411 | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 999000412 | 20000      | 17-18        | 2             | 1                | 2017-05-16     | 2017-06-23  |
		| 999000413 | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |			
		| 999000414 | 40000      | 17-18        | 4             | 1                | 2017-08-17     | 2017-08-23  |	
	When we refresh levy data for account id 25 paye scheme 123/ABC
	And All the transaction lines in this scenario have had there transaction date updated to the specified created date
	Then account with id 25 should see a level 1 screen with a balance of 44000 on the 08/2017
	And account with id 25 should see a level 1 screen with a total levy of 11000 on the 05/2017
	And account with id 25 should see a level 1 screen with a total levy of 11000 on the 06/2017
	And account with id 25 should see a level 2 screen with a levy declared of 10000 on the 05/2017
	And account with id 25 should see a level 2 screen with a top up of 1000 on the 05/2017
	And account with id 25 should see a level 2 screen with a levy declared of 10000 on the 06/2017
	And account with id 25 should see a level 2 screen with a top up of 1000 on the 06/2017

Scenario: Sceanrio-02-Late-submissions
	Given We have an account with id 25
	And Hmrc return the following submissions for paye scheme 123/ABC
		| Id          | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 999000415   | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 999000416   | 20000      | 17-18        | 2             | 1                | 2017-06-16     | 2017-06-23  |
		| 999000417   | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |			
		| 999000418   | 40000      | 17-18        | 4             | 1                | 2017-08-17     | 2017-08-23  |		
		| 999000419   | 21250      | 17-18        | 2             | 1                | 2017-09-16     | 2017-09-23  |
		| 999000420   | 31250      | 17-18        | 3             | 1                | 2017-09-15     | 2017-09-23  |			
		| 999000421   | 41250      | 17-18        | 4             | 1                | 2017-09-17     | 2017-09-23  |		
		| 999000422   | 51250      | 17-18        | 5             | 1                | 2017-09-17     | 2017-09-23  |		
	When we refresh levy data for account id 25 paye scheme 123/ABC
	And All the transaction lines in this scenario have had there transaction date updated to the specified created date
	Then account with id 25 should see a level 1 screen with a balance of 56375 on the 08/2017
	And account with id 25 should see a level 1 screen with a total levy of 11000 on the 08/2017
	And account with id 25 should see a level 1 screen with a total levy of 12375 on the 09/2017
	And account with id 25 should see a level 2 screen with a levy declared of 10000 on the 08/2017
	And account with id 25 should see a level 2 screen with a top up of 1000 on the 08/2017
	And account with id 25 should see a level 2 screen with a levy declared of 11250 on the 09/2017
	And account with id 25 should see a level 2 screen with a top up of 1125 on the 09/2017
