Feature: AddPayeScheme
	In order to associate multiple PAYE schemes with an account
	As an account owner
	I want to be able to add new PAYE schemes attached to existing or new legal entities

@mytag
Scenario Outline: Remove PAYE scheme 
Given I am an account "<account_role>"
When I remove a scheme
Then Scheme is "status"
Examples:
| account_role | scheme_status |
	| Owner        | removed       |
	| Viewer       | not_removed   |
	| Transactor   | not_removed  |


Scenario Outline: Add new PAYE scheme
Given I am an account "<account_role>"
	When I Add a new PAYE scheme
	Then The PAYE scheme Is "<scheme_status>"
Examples:
	| account_role | scheme_status |
	| Owner        | created       |
	| Viewer       | not_created   |
	| Transactor   | not_created   |

Scenario Outline: View my available schemes
	Given I am an account "<account_role>"
	Then I can view all of my PAYE schemes
Examples:
	| account_role |
	| Owner        |
	| Viewer       |
	| Transactor   |