Feature: Late Accounts Feature

Scenario: 1 - Account is opened on the 23 MAY 2017 and after only added valid PAYE HMRC submissions no older than 12 months old - excludes 1 HMRC submission
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate |
	| 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     |
	| 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     |
	| 999000103 | 3000       | 17-18        | 3             | 1                | 2017-07-19     |
	| 999000104 | 4000       | 17-18        | 4             | 1                | 2017-08-19     |
	| 999000105 | 5000       | 17-18        | 5             | 1                | 2017-09-19     |
	| 999000106 | 6000       | 17-18        | 6             | 1                | 2017-10-19     |
	| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     |
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     |
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     |
	When we refresh levy data for paye scheme on the 5/2018
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 5/2018
	And we should see a level 1 screen with a balance of 13200 on the 5/2018

Scenario: 2 - Account is opened on the 22 MAY 2017 and after only added valid submissions no older than 12 months old - excludes 2 HMRC submissions
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit    
	And Hmrc return the following submissions for paye scheme
    | Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate |
    | 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     |
    | 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     |
    | 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     |
    | 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     |
    | 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     |
    | 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     |
    | 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     |
    | 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     |
    | 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     |
	When we refresh levy data for paye scheme on the 5/2018
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 5/2018
	And we should see a level 1 screen with a balance of 13200 on the 5/2018


Scenario: 3 - Account is opened on the 22 MAY 2017 and after only added valid submissions no older than 12 months old - no HMRC submissions older than 12 months
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit     
	And Hmrc return the following submissions for paye scheme    
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate |
	| 999000101 | 0          | 17-18        | 1             | 1                | 2017-05-19     |
	| 999000102 | 0          | 17-18        | 2             | 1                | 2017-06-19     |
	| 999000103 | 0          | 17-18        | 3             | 1                | 2017-07-19     |
	| 999000104 | 0          | 17-18        | 4             | 1                | 2017-08-19     |
	| 999000105 | 0          | 17-18        | 5             | 1                | 2017-09-19     |
	| 999000106 | 0          | 17-18        | 6             | 1                | 2017-10-19     |
	| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     |
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     |
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     |
	When we refresh levy data for paye scheme on the 5/2018
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 5/2018
	And we should see a level 1 screen with a balance of 14300 on the 5/2018

Scenario: 4 - An End-of-year-adjustment is for a period older than 12 months and one younger than 12 months is in submissions of a newly added PAYE scheme - excludes 1 HMRC submission
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit 
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | EndofYear Adjustment |
	| 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     | 0                    |
	| 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     | 0                    |
	| 999000103 | 3000       | 17-18        | 3             | 1                | 2017-07-19     | 0                    |
	| 999000104 | 4000       | 17-18        | 4             | 1                | 2017-08-19     | 0                    |
	| 999000105 | 5000       | 17-18        | 5             | 1                | 2017-09-19     | 0                    |
	| 999000106 | 6000       | 17-18        | 6             | 1                | 2017-10-19     | 0                    |
	| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     | 0                    |
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 0                    |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 0                    |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 0                    |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 0                    |
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 0                    |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 0                    |
	| 999000114 | 2000       | 18-19        | 2             | 1                | 2018-06-19     | 0                    |
	| 999000115 | 3000       | 18-19        | 3             | 1                | 2018-07-19     | 0                    |
	| 999000116 | 4000       | 18-19        | 4             | 1                | 2018-08-19     | 0                    |
	| 999000117 | 5000       | 18-19        | 5             | 1                | 2018-09-19     | 0                    |
	| 999000118 | 6000       | 18-19        | 6             | 1                | 2018-10-19     | 0                    |
	| 999000119 | 11000      | 17-18        | 12            | 1                | 2018-10-19     | 1                    |
	| 999000120 | 7000       | 18-19        | 7             | 1                | 2018-11-19     | 0                    |
	| 999000121 | 8000       | 18-19        | 8             | 1                | 2018-12-19     | 0                    |
	| 999000122 | 9000       | 18-19        | 9             | 1                | 2019-01-19     | 0                    |
	| 999000123 | 10000      | 18-19        | 10            | 1                | 2019-02-19     | 0                    |
	| 999000124 | 11000      | 18-19        | 11            | 1                | 2019-03-19     | 0                    |
	| 999000125 | 12000      | 18-19        | 12            | 1                | 2019-04-19     | 0                    |
	| 999000126 | 1000       | 19-20        | 1             | 1                | 2019-05-19     | 0                    |
	| 999000127 | 11500      | 18-19        | 12            | 1                | 2019-05-19     | 1                    |
	When we refresh levy data for paye scheme on the 5/2019
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 550 on the 05/2019
	And we should see a level 1 screen with a balance of 12650 on the 05/2019


Scenario: 5 - A PAYE being used to create a new account has been in a different account within the 12 month limit - excludes 4 HMRC submissions from a previous account
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit 
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | EndofYear Adjustment |  
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 0                    |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 0                    |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 0                    |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 0                    |
	And we refresh levy data for paye scheme on the 03/2018
	And Another account is opened and associated with the paye scheme
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | EndofYear Adjustment |  
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 0                    |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 0                    |
	| 999000114 | 2000       | 18-19        | 2             | 1                | 2018-06-19     | 0                    |
	| 999000115 | 3000       | 18-19        | 3             | 1                | 2018-07-19     | 0                    |
	| 999000116 | 4000       | 18-19        | 4             | 1                | 2018-08-19     | 0                    |
	| 999000117 | 5000       | 18-19        | 5             | 1                | 2018-09-19     | 0                    |
	| 999000118 | 6000       | 18-19        | 6             | 1                | 2018-10-19     | 0                    |
	| 999000119 | 12500      | 17-18        | 12            | 1                | 2018-10-19     | 1                    |
	| 999000120 | 7000       | 18-19        | 7             | 1                | 2018-11-19     | 0                    |
	| 999000121 | 8000       | 18-19        | 8             | 1                | 2018-12-19     | 0                    |
	| 999000122 | 9000       | 18-19        | 9             | 1                | 2019-01-19     | 0                    |
	| 999000123 | 10000      | 18-19        | 10            | 1                | 2019-02-19     | 0                    |
	When we refresh levy data for paye scheme on the 02/2019
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 02/2019
	And we should see a level 1 screen with a balance of 12650 on the 02/2019

Scenario: 6 - A PAYE being used to create a new account has been in a different account longer ago than the 12 month limit and the new account includes end of year adjustments within that time limit and ignores one outside the limit -
		excludes 13 HMRC submissions including an end of year adjustment relating to a period older than 12 months. 
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit    
	And Hmrc return the following submissions for paye scheme  
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | EndofYear Adjustment |
	| 999000100 | 12000      | 16-17        | 12            | 1                | 2017-04-19     | 0 			 | 
	| 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     | 0 			 |
	| 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     | 0			 |	
	| 999000103 | 3000       | 17-18        | 3             | 1                | 2017-07-19     | 0                    |
	| 999000104 | 4000       | 17-18        | 4             | 1                | 2017-08-19     | 0                    |
	| 999000105 | 5000       | 17-18        | 5             | 1                | 2017-09-19     | 0                    |
	And we refresh levy data for paye scheme on the 09/2017
    And Another account is opened and associated with the paye scheme
    And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | EndofYear Adjustment |
	| 999000106 | 6000       | 17-18        | 6             | 1                | 2017-10-19     | 0                    |
	| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     | 0                    |
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 0                    |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 0                    |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 0                    |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 0                    |
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 0                    |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 0                    |
	| 999000114 | 2000       | 18-19        | 2             | 1                | 2018-06-19     | 0                    |
	| 999000115 | 3000       | 18-19        | 3             | 1                | 2018-07-19     | 0                    |
	| 999000116 | 4000       | 18-19        | 4             | 1                | 2018-08-19     | 0                    |
	| 999000117 | 5000       | 18-19        | 5             | 1                | 2018-09-19     | 0                    |
	| 999000118 | 6000       | 18-19        | 6             | 1                | 2018-10-19     | 0                    |
	| 999000119 | 12500      | 17-18        | 12            | 1                | 2018-10-19     | 1                    |
	| 999000120 | 7000       | 18-19        | 7             | 1                | 2018-11-19     | 0                    |
	| 999000121 | 8000       | 18-19        | 8             | 1                | 2018-12-19     | 0                    |
	| 999000122 | 9000       | 18-19        | 9             | 1                | 2019-01-19     | 0                    |
	| 999000123 | 10000      | 18-19        | 10            | 1                | 2019-02-19     | 0                    |
	| 999000124 | 11000      | 18-19        | 11            | 1                | 2019-03-19     | 0                    |
	| 999000125 | 13000      | 16-17        | 12            | 1                | 2019-03-19     | 1                    |
	When we refresh levy data for paye scheme on the 03/2019
    And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 03/2019
	And we should see a level 1 screen with a balance of 13750 on the 03/2019