Feature: HMRC Scenario Eleven - Non Delarations

Scenario: Month one submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | NonDeclaration | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | False          | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |		
	Then the balance on 05/2017 should be 11000 on the screen
	And the total levy shown for month 05/2017 should be 11000
	And For month 05/2017 the levy declared should be 10000 and the topup should be 1000

Scenario: Month two submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | NonDeclaration | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | False          | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | True           | 0          | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |		
	Then the balance on 06/2017 should be 11000 on the screen		
	And the total levy shown for month 06/2017 should be 0
	And For month 06/2017 the levy declared should be 0 and the topup should be 0
	
Scenario: Month three submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | NonDeclaration | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | False          | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | True           | 0          | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | False          | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |		
	Then the balance on 07/2017 should be 33000 on the screen
	And the total levy shown for month 07/2017 should be 22000
	And For month 07/2017 the levy declared should be 20000 and the topup should be 2000

Scenario: Month four submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | NonDeclaration | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | False          | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | True           | 0          | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | False          | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |		
		| 123/ABC     | True           | NULL       | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |		
	Then the balance on 08/2017 should be 33000 on the screen
	And the total levy shown for month 08/2017 should be 0
	And For month 08/2017 the levy declared should be 0 and the topup should be 0

Scenario: Month five submission
	Given I have an account
	When I have the following submissions
		| Paye_scheme | NonDeclaration | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 123/ABC     | False          | 10000      | 17-18        | 1             | 1                | 2017-05-15     | 2017-05-23  |
		| 123/ABC     | True           | 0          | 17-18        | 2             | 1                | 2017-06-15     | 2017-06-23  |
		| 123/ABC     | False          | 30000      | 17-18        | 3             | 1                | 2017-07-15     | 2017-07-23  |		
		| 123/ABC     | True           | NULL       | 17-18        | 4             | 1                | 2017-08-15     | 2017-08-23  |		
		| 123/ABC     | False          | 40000      | 17-18        | 5             | 1                | 2017-09-15     | 2017-09-23  |		
	Then the balance on 09/2017 should be 44000 on the screen
	And the total levy shown for month 09/2017 should be 11000
	And For month 09/2017 the levy declared should be 10000 and the topup should be 1000
