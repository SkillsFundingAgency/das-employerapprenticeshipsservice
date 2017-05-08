Feature: TransactionLine
	In order to show details of my balance
	I want view transactions in and out for my account


Scenario: Transaction History levy declarations
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 223/ABC     | 1000       | 16-17        | 11            | 1                | 2017-03-18     | 2017-03-23  |
		| 223/ABC     | 1100       | 16-17        | 12            | 1                | 2017-04-18     | 2017-04-23  |
	Then the balance should be 1210 on the screen									  

Scenario: Transaction History levy declarations with multiple schemes
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |SubmissionDate |CreatedDate |
		| 123/ABC     | 1000       | 16-17        | 11            | 1                |2017-03-17     |			  |
		| 456/ABC     | 1000       | 16-17        | 11            | 1                |2017-03-18     |2017-03-23  |
		| 123/ABC     | 1100       | 16-17        | 12            | 1                |2017-04-18     |2017-04-23  |
	Then the balance should be 2310 on the screen									  

Scenario: Transaction History levy declarations over Payroll_year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |SubmissionDate |
		| 323/ABC     | 1000       | 16-17        | 12            | 1                |2017-04-18     |
		| 323/ABC     | 100        | 17-18        | 01            | 1                |2017-05-18     |
	Then the balance should be 1210 on the screen

Scenario: End of Year Adjustment to account is applied to levy credit for adjustment
	Given I have an account
	When I have the following submissions
         | Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | EndOfYearAdjustment | EndOfYearAdjustmentAmount |
         | 423/ABC     | 1000       | 17-18        | 11            | 1                | 2018-03-18     | 0                   | 0                         |
         | 423/ABC     | 1100       | 17-18        | 12            | 1                | 2018-04-18     | 0                   | 0                         |
         | 423/ABC     | 100        | 18-19        | 01            | 1                | 2018-05-18     | 0                   | 0                         |
         | 423/ABC     | 1050       | 17-18        | 12            | 1                | 2018-05-18     | 1                   | 50                        |
	Then the balance should be 1265 on the screen

Scenario: End of Year Adjustment to account is applied to levy credit for positive adjustment
	Given I have an account
	When I have the following submissions
         | Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | EndOfYearAdjustment | EndOfYearAdjustmentAmount |
         | 423/ABC     | 1000       | 17-18        | 11            | 1                | 2018-03-18     | 0                   | 0                         |
         | 423/ABC     | 1100       | 17-18        | 12            | 1                | 2018-04-18     | 0                   | 0                         |
         | 423/ABC     | 100        | 18-19        | 01            | 1                | 2018-05-18     | 0                   | 0                         |
         | 423/ABC     | 1150       | 17-18        | 12            | 1                | 2018-05-18     | 1                   | -50                        |
	Then the balance should be 1375 on the screen

Scenario: Transaction History levy declarations and Payments
	Given I have an account
	When I have the following submissions
         | Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |SubmissionDate |
         | 423/ABC     | 1000       | 17-18        | 01            | 1                |2017-03-18     |
         | 423/ABC     | 1100       | 17-18        | 02            | 1                |2017-04-18     |
	And I have the following payments												   
		| Payment_Amount | Payment_Type |											   
		| 100            | levy         |
		| 200            | cofund       |
	Then the balance should be 1110 on the screen
	
Scenario Outline: Transaction History levy declarations late account registration in payroll year
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |SubmissionDate |
		| 425/ABC     | 1000       | 16-17        | 01            | 1                |2016-05-18     |
		| 424/ABC     | 100        | 16-17        | 01            | 1                |2016-05-18     |
		| 425/ABC     | 2000       | 16-17        | 02            | 1                |2016-06-18     |
		| 424/ABC     | 200        | 16-17        | 02            | 1                |2016-06-18     |	
	And I register on month "<Month>"
	Then the balance should be <Balance> on the screen
	Examples: 
	| Month | Balance |
	| 02    | 2420    |
	| 03    | 3300    |

Scenario: Transaction History levy declarations next year registration 
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |SubmissionDate |
		| 323/ABC     | 1000       | 16-17        | 12            | 1                |2016-05-18     |
		| 323/ABC     | 100        | 17-18        | 01            | 1                |2016-05-18     |
	And I register on DAS in year 16-17 month 12									  
	Then the balance should be 1210 on the screen									  

Scenario: Transaction History levy declarations next year registration multiple PAYE schemes
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |SubmissionDate |
		| 327/ABC     | 1000       | 16-17        | 12            | 1                |2016-05-18     |
		| 427/ABC     | 1000       | 16-17        | 12            | 1                |2016-05-18     |
		| 327/ABC     | 100        | 17-18        | 01            | 1                |2016-06-18     |
		| 427/ABC     | 100        | 17-18        | 01            | 1                |2016-06-18     |	
	And I register on DAS in year 16-17 month 01
	Then the balance should be 2420 on the screen
	
Scenario: Single PAYE scheme removed
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction |SubmissionDate |
		| 328/ABC     | 1000       | 16-17        | 01            | 1                |2016-05-18     |
		| 328/ABC     | 1100       | 16-17        | 02            | 1                |2016-05-18     |
		| 328/ABC     | 1200       | 16-17        | 03            | 1                |2016-06-18     |	
	And I register on DAS in year 16-17 month 01									  
	And I remove the PAYE scheme in month 04
	Then the balance should be 1320 on the screen
