Feature: AddPayeScheme
	In order to associate multiple PAYE schemes with an account
	As an account owner
	I want to be able to add new PAYE schemes attached to existing or new legal entities

@mytag
Scenario Outline: Add new PAYE scheme to existing legal entity
	Given I am an account "<account_role>"
	When I Add a new PAYE scheme to my existing legal entity
	Then The PAYE scheme Is "<scheme_status>"
Examples:
	| account_role | scheme_status |
	| Owner        | created       |
	| Viewer       | not_created   |
	| Transactor   | not_created   |

Scenario Outline: Add new PAYE scheme to new legal entity
Given I am an account "<account_role>"
	When I Add a new PAYE scheme to my new legal entity
	Then The PAYE scheme Is "<scheme_status>"
Examples:
	| account_role | scheme_status |
	| Owner        | created       |
	| Viewer       | not_created   |
	| Transactor   | not_created   |

Scenario: View my available schemes
	Given I am part of an account
	Then I can view all of my PAYE schemes