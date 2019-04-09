Feature: Account is opened on the 23 MAY 2017 and after only added valid PAYE HMRC submissions no older than 12 months old

Scenario: Account is opened on the 23 MAY 2017 and after only added valid PAYE HMRC submissions no older than 12 months old
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |   
	| 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     | 2017-05-20  |
	| 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     | 2017-06-20  |
	| 999000103 | 3000       | 17-18        | 3             | 1                | 2017-07-19     | 2017-07-20  |
	| 999000104 | 4000       | 17-18        | 4             | 1                | 2017-08-19     | 2017-08-20  |
	| 999000105 | 5000       | 17-18        | 5             | 1                | 2017-09-19     | 2017-09-20  |
	| 999000106 | 6000       | 17-18        | 6             | 1                | 2017-10-19     | 2017-10-20  |
	| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     | 2017-11-20  |
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 2017-12-20  |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 2018-01-20  |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 2018-02-20  |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 2018-03-20  |
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 2018-04-20  |
	| 999000113 | 13000      | 18-19        | 1             | 1                | 2018-05-19     | 2018-05-20  |
	When we refresh levy data for paye scheme
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a total levy of 13200 on the 5/2018
	#Then we should see a level 1 screen with a levy transaction of 13200
	#And we should see a level 1 screen with a balance of 13200 on the 5/19 
	#And we should see a level 2 screen with 12 transactions of 1100 each  
	#And levy declared 1000
	#And English fraction of 1
	#And top up value of 100
