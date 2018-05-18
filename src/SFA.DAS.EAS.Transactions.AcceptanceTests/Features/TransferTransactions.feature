Feature: TransferTransactions


Scenario: Sender Transfer Transactions
Given I have an account
When I send a levy transfer to a company
Then I should see a transfer sent transaction on the transaction summary page

Scenario: Receiver Transfer Transactions
Given I have an account
When I receive a levy transfer from a company
Then I should see a transfer received transaction on the transaction summary page
