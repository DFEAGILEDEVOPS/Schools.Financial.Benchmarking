Feature:
  As a user I need to be notified of any errors
  So that I quickly know there is a problem and continue

  Scenario: Error page is displayed when a page is not found
    Given I have navigated to a page that does not exist
    Then I should be shown an error page
