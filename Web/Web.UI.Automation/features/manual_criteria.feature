Feature:
  As a User I want to create a benchmark set of institutions within ranges of values using specified criteria
  So that I can research other variations.

  Scenario: School details page has option to compare to other similar schools
    Given I am on the school details page for the school with the URN 102976
    Then I should see the option to compare to similar schools

  Scenario: Comparison page is displayed when user decides to compare with similar schools
    Given I am on the school details page for the school with the URN 102976
    When I choose to compare to similar schools
    Then I should be taken to the comparison strategy page

  Scenario: Quick comparison page is displayed when user decides to compare with similar schools
    Given I am on the school details page for the school with the URN 102976
    When I choose to compare to similar schools using pre selected characteristics
    Then I should be taken to the quick comparison strategy page

  Scenario: Users can choose the default basket size
    Given I am on the quick comparison strategy page for the school with the URN 102976
    Then I should be able to select the default basket size

  Scenario: Users can define their own basket size
    Given I am on the quick comparison strategy page for the school with the URN 102976
    Then I should be able to define my own basket size

  Scenario: Minimum basket size is 5
    Given I am on the quick comparison strategy page for the school with the URN 102976
    When I enter a custom basket size of less than 5
    Then I should see a validation error

  Scenario: Max basket size is 30
    Given I am on the quick comparison strategy page for the school with the URN 102976
    When I enter a custom basket size of more than 30
    Then I should see a validation error

  Scenario: Back button takes users back to the school details page
    Given I am on the school details page for the school with the URN 102976
    When I choose to compare to similar schools
    But decide I want to go back
    Then I should be taken back to the school details page

  Scenario: Comparison strategy page has option for user to define criteria
    Given I am on the comparison strategy page for the school with the urn 102976
    When I decide to choose the characteristics that i have chosen
    Then I should be taken to step one of the criteria process

  Scenario: Users can choose to include schools in the comparison from step one
    Given I am on step one of the criteria process using school 102980
    When I choose maintained_schools for comparison
    Then I should be taken to step two of the criteria process

  Scenario: Users can choose to include academies in the comparison from step one
    Given I am on step one of the criteria process using school 102980
    When I choose academies for comparison
    Then I should be taken to step two of the criteria process

  Scenario: Users can choose to include all establishments in the comparison from step one
    Given I am on step one of the criteria process using school 102980
    When I choose all_schools for comparison
    Then I should be taken to step two of the criteria process

  @wip
  Scenario: The school being compared is displayed on step two
    Given I have chosen all_schools to be my comparison using school 131222
    Then I should see school 131222 as my baseline comparison

  Scenario: User is displayed a list of characteristicss
    Given I have chosen all_schools to be my comparison using school 102980
    Then I should see a list of characteristics

  Scenario: Characteristics can be added
    Given I have chosen all_schools to be my comparison using school 104988
    Then I should be able to add Gender of pupils as my characteristic

  Scenario: Characteristics can be removed
    Given I have added the Gender of pupils characteristic
    When I decide to remove the Gender of pupils characteristic
    Then I should not see Gender of pupils selected on the page

  Scenario: Characteristic values can be added
    Given I have added the Gender of pupils characteristic
    Then I should be able to select the gender as mixed

  Scenario: Baseline school has a value against the characterisitic
    Given I have added the Gender of pupils characteristic
    Then I should see a benchmark value for my baseline school

  @manual
  Scenario: Characteristic cannot be added twice
    Given I have added the Gender of pupils characteristic
    Then I should not be able to add Gender of pupils as another characteristic

  Scenario: Users can perform a search using the criteria selected using a new basket
    Given I have added 4 schools to the comparison
    And I have selected my criteria
    And I am within the limit of schools
    When I perform the search using a new_basket
    Then I should be taken to the charts page

  Scenario: Users can perform a search using the criteria selected using my existing basket
    Given I have added 4 schools to the comparison
    And I have selected my criteria
    And I am within the limit of schools
    When I perform the search using a existing_basket
    Then I should be taken to the charts page

  Scenario: Users can view their characteristics from charts page
    Given I have selected my criteria
    When I perform the search
    Then I should be able to view my chosen characteristics

  Scenario: Users can edit their characteristics from charts page
    Given I am on the charts page after selecting characteristics
    When I decide to edit my characteristics
    Then I should be on the characteristics page

  Scenario: Users can update their characteristics from charts page
    Given I have to updated my characteristics
    Then I should see my updated choices on the benchmark charts page

  @wip
  Scenario: Search is disabled if there are no characteristics added
    Given I have chosen all_schools to be my comparison using school 130597
    Then I should see search is disabled

  Scenario: Live counter is displayed with number of schools matching criteria
    Given I have added the Gender of school pupils characteristic for school 104991
    Then I should see a live counter

  Scenario: No count is displayed when no schools match the criteria
    Given I have added critera that does not match any schools
    Then I should not see a live counter

  Scenario: Users can clear all characteristics using the info panel
    Given I have added the Gender of pupils characteristic
    When I want to clear all characteristics using the info panel
    Then all characteristics should be cleared

  Scenario: Users can view the benchmark charts from the info panel
    Given I have added the Gender of pupils characteristic
    Then I should be able to view the benchmark charts

  Scenario: Pop up is displayed when user wants to view charts and school count is over the limit
    Given I have added the Gender of school pupils characteristic for school 104993
    But my school count is over the limit
    When I want to view the charts
    Then I should see a pop up with the school count
    And a message that states I should refine the search

  Scenario: Pop up can be closed
    Given the pop up is visible
    When I close the pop up
    Then I should not see the pop up

