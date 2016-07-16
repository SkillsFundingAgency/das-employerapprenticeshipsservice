Feature: FakeLandingPage
	As a AML User
	I want	to be able to choose the user i'm logged in as
	So that	i can use the system

#NOTE This is a FAKE landing page that 
#enables to use of the "Create Employer Account" and "View Transactions" epics while we are waiting for 
#light registration integration. 

@Sprint1
Scenario: Browse Landing Page	
	Given I have Navigated to Landing Page
	When I wait "3" seconds until "SelectedUserId" found by "Name" is displayed
	Then Browser can be closed