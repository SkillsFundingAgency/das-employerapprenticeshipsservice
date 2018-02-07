Feature: FinancialDashboard
	In order to review financial details of my account
	I want view a summary of my account's financial figures


Scenario: Transfer Balance
	Given I have an account
	When I have the following submissions
		| Paye_scheme | LevyDueYtd | Payroll_Year | Payroll_Month | English_Fraction | SubmissionDate | CreatedDate |
		| 223/ABC     | 1000       | 16-17        | 11            | 1                | 2017-03-18     | 2017-03-23  |
		| 223/ABC     | 2000       | 16-17        | 11            | 1                | 2017-03-20     | 2017-03-23  |
		| 223/ABC     | 3000       | 16-17        | 12            | 1                | 2017-04-18     | 2017-04-23  |
		| 223/ABC     | 1000       | 17-18        | 1             | 1                | 2017-05-18     | 2017-05-23  |
		| 223/ABC     | 2000       | 17-18        | 2             | 1                | 2017-06-18     | 2017-06-23  |
		| 223/ABC     | 3000       | 17-18        | 3             | 1                | 2017-07-18     | 2017-07-23  |
	Then the transfer balance should be 3300 on the financial dashboard screen