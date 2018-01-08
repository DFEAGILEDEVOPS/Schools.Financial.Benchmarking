Feature:
  As a user,
  I would like the ability to search in different ways
  So I can quickly and easily view data relevant to me

  Scenario: Predictive search only returns a list of schools after 3 chars have been entered
    Given I want to search for a school via name
    When I enter the first 3 letters of Bedford Modern School
    Then I should see a results list dropdown

  Scenario: Selecting a predicted search result populates directs user to details page
    Given I want to search for a school via name
    When I enter the first 3 letters of Lutterworth College
    And select the first option
    Then I should be taken to details page for the school with the urn 138150

  Scenario: No Predictive results are returned if the school is not found
    Given I want to search for a school via name
    When I enter a school which has no results
    Then I should not see any results

  Scenario: Predictive search only returns a list of academies after 3 chars have been entered
    Given I want to search for a trust via name
    When I enter the 3 letters of the Southmoor Academy trust
    Then I should see a results list dropdown

  Scenario: Selecting a predicted search result populates directs user to mat index page
    Given I want to search for a trust via name
    When I enter the 3 letters of the Southfield Grange Trust trust
    And select the first option
    Then I should be taken to mat index page for the mat with id MAT904

  Scenario: No Predictive results are returned if the school is not found
    Given I want to search for a trust via name
    When I enter a trust which has no results
    Then I should not see any results

  Scenario: Predictive search only returns a list of local authority after 3 chars have been entered
    Given I want to search for a local authority via name
    When I enter the 3 letters of the Hertfordshire authority
    Then I should see a results list dropdown

  Scenario: Selecting a predicted search result populates directs user to mat index page
    Given I want to search for a local authority via name
    When I enter the 3 letters of the Hertfordshire authority
    And select the first option
    Then I should see the search results page

  Scenario: No Predictive results are returned if the school is not found
    Given I want to search for a local authority via name
    When I enter a local authority which has no results
    Then I should not see any results

  Scenario: School information is displayed when searching via result from predictive search
    Given I have selected an option from the results list dropdown
    Then I should be taken to the school details page

  Scenario: Users can search via postcode
    Given I want to search for a school via location
    When I enter a valid postcode like LU20QB
    Then I should see a search results page

  Scenario: Users can search via town
    Given I want to search for a school via location
    When I enter a valid town like London
    Then I should see a search results page

  Scenario: Users can search via street
    Given I want to search for a school via location
    When I enter a valid street like Old Bedford Road
    Then I should see a search results page

  Scenario: Users can search via LA code
    Given I want to search for a school via LA code
    When I search for a valid LA code like 821
    Then I should only see schools from Luton

  Scenario: Users can search via maintained school id using URN
    Given I want to search for a school via maintained school id
    When I search for a valid maintained school id code using urn like 102976
    Then I should be taken to details page for the school with the urn 102976

  Scenario: Users can search via maintained school id using LAESTAB
    Given I want to search for a school via maintained school id
    When I search for a valid maintained school id code using laestab like 8802407
    Then I should be taken to details page for the school with the laestab 8802407


