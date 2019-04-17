Feature: Account is opened on the 23 MAY 2017 and after only added valid PAYE HMRC submissions no older than 12 months old

Scenario: 1 - Account is opened on the 23 MAY 2017 and after only added valid PAYE HMRC submissions no older than 12 months old
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
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 2018-05-20  |
	When we refresh levy data for paye scheme on the 5/2018
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 5/2018
	And we should see a level 1 screen with a balance of 13200 on the 5/2018

	#Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit
	#And Account 1 is opened
	#And Account 2 is opened
	#And Account 1 is associated with paye scheme 123/aaa
	#And Hmrc return the following submissions for paye scheme 123/aaa (with table)
	#And we refresh levy data for paye scheme 123/aaa
	#And account 2 is associated with paye scheme 123/aaa (paye scheme has been detached from account 1)
	#And Hmrc return the following submissions for paye scheme 123/aaa (with table)
	#when we refresh levy data for paye scheme 123/aaa
	#Then ....

Scenario: 2 - Account is opened on the 22 MAY 2017 and after only added valid submissions no older than 12 months old
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit    
	And Hmrc return the following submissions for paye scheme
    | Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
    | 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     | 2017-05-20  |
    | 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     | 2017-06-20  |
    | 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     | 2017-11-20  |
    | 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 2017-12-20  |
    | 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 2018-01-20  |
    | 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 2018-02-20  |
    | 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 2018-03-20  |
    | 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 2018-04-20  |
    | 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 2018-05-20  |
	When we refresh levy data for paye scheme on the 5/2018
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 5/2018
	And we should see a level 1 screen with a balance of 13200 on the 5/2018


Scenario: 3 - Account is opened on the 22 MAY 2017 and after only added valid submissions no older than 12 months old 
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit     
	And Hmrc return the following submissions for paye scheme    
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
	| 999000101 | 0          | 17-18        | 1             | 1                | 2017-05-19     | 2017-05-20  |
	| 999000102 | 0          | 17-18        | 2             | 1                | 2017-06-19     | 2017-06-20  |
	| 999000103 | 0          | 17-18        | 3             | 1                | 2017-07-19     | 2017-07-20  |
	| 999000104 | 0          | 17-18        | 4             | 1                | 2017-08-19     | 2017-08-20  |
	| 999000105 | 0          | 17-18        | 5             | 1                | 2017-09-19     | 2017-09-20  |
	| 999000106 | 0          | 17-18        | 6             | 1                | 2017-10-19     | 2017-10-20  |
	| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     | 2017-11-20  |
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 2017-12-20  |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 2018-01-20  |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 2018-02-20  |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 2018-03-20  |
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 2018-04-20  |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 2018-05-20  |
	When we refresh levy data for paye scheme on the 5/2018
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 5/2018
	And we should see a level 1 screen with a balance of 14300 on the 5/2018
#
#
#
#---
#Feature: HMRC-Scenario-05-Adjustment-to-prior-tax-year
#
Scenario: 4 - An End-of-year-adjustment is for a period older than 12 months and one younger than 12 months is in submissions of a newly added PAYE scheme
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit 
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate | EndofYear Adjustment |
	| 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     | 2017-05-20  | 0                    |
	| 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     | 2017-06-20  | 0                    |
	| 999000103 | 3000       | 17-18        | 3             | 1                | 2017-07-19     | 2017-07-20  | 0                    |
	| 999000104 | 4000       | 17-18        | 4             | 1                | 2017-08-19     | 2017-08-20  | 0                    |
	| 999000105 | 5000       | 17-18        | 5             | 1                | 2017-09-19     | 2017-09-20  | 0                    |
	| 999000106 | 6000       | 17-18        | 6             | 1                | 2017-10-19     | 2017-10-20  | 0                    |
	| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     | 2017-11-20  | 0                    |
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 2017-12-20  | 0                    |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 2018-01-20  | 0                    |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 2018-02-20  | 0                    |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 2018-03-20  | 0                    |
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 2018-04-20  | 0                    |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 2018-05-20  | 0                    |
	| 999000114 | 2000       | 18-19        | 2             | 1                | 2018-06-19     | 2018-06-20  | 0                    |
	| 999000115 | 3000       | 18-19        | 3             | 1                | 2018-07-19     | 2018-07-20  | 0                    |
	| 999000116 | 4000       | 18-19        | 4             | 1                | 2018-08-19     | 2018-08-20  | 0                    |
	| 999000117 | 5000       | 18-19        | 5             | 1                | 2018-09-19     | 2018-09-20  | 0                    |
	| 999000118 | 6000       | 18-19        | 6             | 1                | 2018-10-19     | 2018-10-20  | 0                    |
	| 999000119 | 11000      | 17-18        | 12            | 1                | 2018-10-19     | 2018-10-20  | 1                    |
	| 999000120 | 7000       | 18-19        | 7             | 1                | 2018-11-19     | 2018-11-20  | 0                    |
	| 999000121 | 8000       | 18-19        | 8             | 1                | 2018-12-19     | 2018-12-20  | 0                    |
	| 999000122 | 9000       | 18-19        | 9             | 1                | 2019-01-19     | 2019-01-20  | 0                    |
	| 999000123 | 10000      | 18-19        | 10            | 1                | 2019-02-19     | 2019-02-20  | 0                    |
	| 999000124 | 11000      | 18-19        | 11            | 1                | 2019-03-19     | 2019-03-20  | 0                    |
	| 999000125 | 12000      | 18-19        | 12            | 1                | 2019-04-19     | 2019-04-20  | 0                    |
	| 999000126 | 1000       | 19-20        | 1             | 1                | 2019-05-19     | 2019-05-20  | 0                    |
	| 999000127 | 11500      | 18-19        | 12            | 1                | 2019-05-19     | 2019-05-20  | 1                    |
	When we refresh levy data for paye scheme on the 5/2019
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 550 on the 05/2019
	And we should see a level 1 screen with a balance of 12650 on the 05/2019


Scenario: 5 - A PAYE being used to create a new account has been in a different account within the 12 month limit 
	Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit 
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate | EndofYear Adjustment |  
	| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 2017-12-20  | 0                    |
	| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 2018-01-20  | 0                    |
	| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 2018-02-20  | 0                    |
	| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 2018-03-20  | 0                    |
	And we refresh levy data for paye scheme on the 03/2018
	And Another account is opened and associated with the paye scheme
	And Hmrc return the following submissions for paye scheme
	| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate | EndofYear Adjustment |  
	| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 2018-04-20  | 0                    |
	| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 2018-05-20  | 0                    |
	| 999000114 | 2000       | 18-19        | 2             | 1                | 2018-06-19     | 2018-06-20  | 0                    |
	| 999000115 | 3000       | 18-19        | 3             | 1                | 2018-07-19     | 2018-07-20  | 0                    |
	| 999000116 | 4000       | 18-19        | 4             | 1                | 2018-08-19     | 2018-08-20  | 0                    |
	| 999000117 | 5000       | 18-19        | 5             | 1                | 2018-09-19     | 2018-09-20  | 0                    |
	| 999000118 | 6000       | 18-19        | 6             | 1                | 2018-10-19     | 2018-10-20  | 0                    |
	| 999000119 | 12500      | 17-18        | 12            | 1                | 2018-10-19     | 2018-10-20  | 1                    |
	| 999000120 | 7000       | 18-19        | 7             | 1                | 2018-11-19     | 2018-11-20  | 0                    |
	| 999000121 | 8000       | 18-19        | 8             | 1                | 2018-12-19     | 2018-12-20  | 0                    |
	| 999000122 | 9000       | 18-19        | 9             | 1                | 2019-01-19     | 2019-01-20  | 0                    |
	| 999000123 | 10000      | 18-19        | 10            | 1                | 2019-02-19     | 2019-02-20  | 0                    |
	When we refresh levy data for paye scheme on the 02/2019
	And all the transaction lines in this scenario have had their transaction date updated to their created date
	Then we should see a level 1 screen with a levy declared of 1100 on the 02/2019
	And we should see a level 1 screen with a balance of 12650 on the 02/2019


#
#Scenario 6 A PAYE being used to create a new account has been in a different account longer ago than the 12 month limit and the new account includes 
#end of year adjustments within that time limit and ignores one outside the limit. 
#
##Given An employer is adding a PAYE which has submissions older than the 12 month expiry rule limit
#    
##And Account 1 is opened
#    
##And Account 2 is opened
#    
##And Account 1 is associated with paye scheme 123/aaa
#    
##And Hmrc return the following submissions for paye scheme 123/aaa  
#| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate | EndofYear Adjustment |
#| 999000100 | 12000      | 16-17        | 12            | 1                | 2017-04-19     | 2017-04-20  | 0 			 | 
#| 999000101 | 1000       | 17-18        | 1             | 1                | 2017-05-19     | 2017-05-20  | 0 			 |
#| 999000102 | 2000       | 17-18        | 2             | 1                | 2017-06-19     | 2017-06-20  | 0			 |	
#| 999000103 | 3000       | 17-18        | 3             | 1                | 2017-07-19     | 2017-07-20  | 0                    |
#| 999000104 | 4000       | 17-18        | 4             | 1                | 2017-08-19     | 2017-08-20  | 0                    |
#| 999000105 | 5000       | 17-18        | 5             | 1                | 2017-09-19     | 2017-09-20  | 0                    |
#
##And we refresh levy data for paye scheme 123/aaa
#    
##And account 2 is associated with paye scheme 123/aaa (paye scheme has been detached from account 1)
#    
##And Hmrc return the following submissions for paye scheme 123/aaa (with table)
# 
#
#| Id        | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate | EndofYear Adjustment |  
#| 999000106 | 6000       | 17-18        | 6             | 1                | 2017-10-19     | 2017-10-20  | 0                    |
#| 999000107 | 7000       | 17-18        | 7             | 1                | 2017-11-19     | 2017-11-20  | 0                    |
#| 999000108 | 8000       | 17-18        | 8             | 1                | 2017-12-19     | 2017-12-20  | 0                    |
#| 999000109 | 9000       | 17-18        | 9             | 1                | 2018-01-19     | 2018-01-20  | 0                    |
#| 999000110 | 10000      | 17-18        | 10            | 1                | 2018-02-19     | 2018-02-20  | 0                    |
#| 999000111 | 11000      | 17-18        | 11            | 1                | 2018-03-19     | 2018-03-20  | 0                    |
#| 999000112 | 12000      | 17-18        | 12            | 1                | 2018-04-19     | 2018-04-20  | 0                    |
#| 999000113 | 1000       | 18-19        | 1             | 1                | 2018-05-19     | 2018-05-20  | 0                    |
#| 999000114 | 2000       | 18-19        | 2             | 1                | 2018-06-19     | 2018-06-20  | 0                    |
#| 999000115 | 3000       | 18-19        | 3             | 1                | 2018-07-19     | 2018-07-20  | 0                    |
#| 999000116 | 4000       | 18-19        | 4             | 1                | 2018-08-19     | 2018-08-20  | 0                    |
#| 999000117 | 5000       | 18-19        | 5             | 1                | 2018-09-19     | 2018-09-20  | 0                    |
#| 999000118 | 6000       | 18-19        | 6             | 1                | 2018-10-19     | 2018-10-20  | 0                    |
#| 999000119 | 12500      | 17-18        | 12            | 1                | 2018-10-19     | 2018-10-20  | 1                    |
#| 999000120 | 7000       | 18-19        | 7             | 1                | 2018-11-19     | 2018-11-20  | 0                    |
#| 999000121 | 8000       | 18-19        | 8             | 1                | 2018-12-19     | 2018-12-20  | 0                    |
#| 999000122 | 9000       | 18-19        | 9             | 1                | 2019-01-19     | 2019-01-20  | 0                    |
#| 999000123 | 10000      | 18-19        | 10            | 1                | 2019-02-19     | 2019-02-20  | 0                    |
#| 999000124 | 11000      | 18-19        | 11            | 1                | 2019-03-19     | 2019-03-20  | 0                    |
#| 999000125 | 13000      | 16-17        | 12            | 1                | 2019-03-19     | 2019-03-20  | 1                    |
#
##when we refresh levy data for paye scheme 123/aaa
#    
#WHEN we view the account transactions
#THEN we should see a level 1 screen with an levy declared of 13750
#AND we should see a level 1 screen with a balance of 13750 