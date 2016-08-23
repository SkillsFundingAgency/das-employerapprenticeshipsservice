Feature: Agreements
	In order to 

@mytag
Scenario Outline: Sign Agreements
	Given I am an account "<accountRole>"
	When  I sign Agreement
	Then Agreement Status is "<status>"
Examples: 
| accountRole | status     |
| Owner       | signed     |
| Transactor  | not_signed |
| Viewer      | not_signed |

@mytag
Scenario: Check Aknowledgement
Given I am an account "Owner"
And I have not checked aknowledgement
