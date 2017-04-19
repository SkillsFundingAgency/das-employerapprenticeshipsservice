Feature: Agreements
	In order to allow spending on my account
	I want to be able to sign an ESFA agreement against a legal entity connected to my account

@mytag
Scenario Outline: Sign Agreements
	Given I am an account "<accountRole>"
	When  I sign Agreement
	Then Agreement Status is "<status>"
Examples: 
| accountRole | status     |
| Owner       | Signed     |
| Transactor  | Pending	   |
| Viewer      | Pending	   |

