Feature: HMRC-Scenario-01-Single-PAYE-no-adjustments

Scenario: Month-01-submission
	Given We have an account with a paye scheme
	And Hmrc return the following submissions for paye scheme
		| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 999000101 | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-15  | 
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 11000 on the 05/2017
	And we should see a level 1 screen with a total levy of 11000 on the 05/2017
	And we should see a level 2 screen with a levy declared of 10000 on the 05/2017
	And we should see a level 2 screen with a top up of 1000 on the 05/2017

Scenario: Month-02-submission
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id           | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | 
		| 999000102    | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 
		| 999000103    | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 
		| 999000104    | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 		
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 22000 on the 06/2017
	And we should see a level 1 screen with a total levy of 11000 on the 06/2017
	And we should see a level 2 screen with a levy declared of 10000 on the 06/2017
	And we should see a level 2 screen with a top up of 1000 on the 06/2017
	
Scenario: Month-03-submission
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id           | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | 
		| 999000105    | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 
		| 999000106    | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 
		| 999000107    | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 33000 on the 07/2017
	And we should see a level 1 screen with a total levy of 11000 on the 07/2017
	And we should see a level 2 screen with a levy declared of 10000 on the 07/2017
	And we should see a level 2 screen with a top up of 1000 on the 07/2017

Scenario: Month-06-submission
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id           | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | 
		| 999000108    | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 
		| 999000109    | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 
		| 999000110    | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 
		| 999000111    | 40000      | 17-18        | 4             | 1                | 2017-08-15     | 
		| 999000112    | 50000      | 17-18        | 5             | 1                | 2017-09-15     | 
		| 999000113    | 60000      | 17-18        | 6             | 1                | 2017-10-15     | 		
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 66000 on the 10/2017
	And we should see a level 1 screen with a total levy of 11000 on the 10/2017
	And we should see a level 2 screen with a levy declared of 10000 on the 10/2017
	And we should see a level 2 screen with a top up of 1000 on the 10/2017

Scenario: Month-12-submission
	Given We have an account with a paye scheme 
	And Hmrc return the following submissions for paye scheme
		| Id           | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | 
		| 999000114    | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 
		| 999000115    | 20000      | 17-18        | 2             | 1                | 2017-06-15     | 
		| 999000116    | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 
		| 999000117    | 40000      | 17-18        | 4             | 1                | 2017-08-15     | 
		| 999000118    | 50000      | 17-18        | 5             | 1                | 2017-09-15     | 
		| 999000119    | 60000      | 17-18        | 6             | 1                | 2017-10-15     | 
		| 999000120    | 70000      | 17-18        | 7             | 1                | 2017-11-15     | 
		| 999000121    | 80000      | 17-18        | 8             | 1                | 2017-12-15     | 
		| 999000122    | 90000      | 17-18        | 9             | 1                | 2018-01-15     | 
		| 999000123    | 100000     | 17-18        | 10            | 1                | 2018-02-15     | 
		| 999000124    | 110000     | 17-18        | 11            | 1                | 2018-03-15     | 
		| 999000125    | 120000     | 17-18        | 12            | 1                | 2018-04-15     | 		
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a balance of 132000 on the 04/2018 
	And we should see a level 1 screen with a total levy of 11000 on the 04/2018 
	And we should see a level 2 screen with a levy declared of 10000 on the 04/2018 
	And we should see a level 2 screen with a top up of 1000 on the 04/2018 
