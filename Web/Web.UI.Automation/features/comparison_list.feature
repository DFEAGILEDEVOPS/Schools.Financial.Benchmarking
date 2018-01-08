Feature:
  As part of viewing a school
  I want to be able to add a school to a comparison list
  So that I can compare different schools

  Scenario: Schools can be added to a list so they can be compared
    Given I am on the school details page for the school with the URN 102976
    Then I should see an option to add the school to the comparison list

  Scenario: Schools can be added to the comparison list
    Given I am on the school details page for the school with the URN 102976
    When I add the school to the comparison list
    Then i should see that the school has been added

  Scenario: The comparison list has a count of schools displayed on the details page
    Given I have added 3 schools to the comparison
    And I am on the school details page
    Then I should see a count of 3 schools that are in the comparison list

  @wip
  Scenario: The comparison list can have a maximum of 30 schools in the list
    Given I have added 30 schools to the comparison
    And I am on the school details page
    Then I should see a count of 30 schools that are in the comparison list

  Scenario: School details page allows user to add school as home school
    Given I am on the school details page for the school with the URN 102976
    Then I should have the option to set the school as my home school

  Scenario: My home school is displayed on the home page
    Given I have set the school with the URN 102976 as my home school
    When I navigate to the home page
    Then I should see my home school displayed as my default benchmark

  Scenario: My home school can take me to the details page of the school from the homepage
    Given I have set the school with the URN 102976 as my home school
    When I navigate to the home page
    And choose to view details of my home school
    Then I am taken to the details page of my home school

  Scenario: My home school is displayed when I choose to edit my benchmark list
    Given I have set the school with the URN 102976 as my home school
    And I have added 1 schools to the comparison
    When I choose to edit my benchmark list
    Then I should see my home school in the benchmark list

  @wip
  Scenario: My home school is displayed when viewing benchmarking charts
    Given I have set the school with the URN 109575 as my home school
    When I choose to view the benchmarking charts
    Then I should see my home school as the main comparison

  @wip
  Scenario: Each benchmarking chart contains values for my home school
    Given I have set the school with the URN 102976 as my home school
    When I choose to view the benchmarking charts
    Then I should see values for my home school in each chart

  Scenario: Home school can be changed to another school in the benchmark list
    Given I have set the school with the URN 102976 as my home school
    And I have 4 schools in my benchmark list
    When I change my home school to another school
    Then I should see my home school change to my new choice of school

  Scenario: Home school can be removed from the benchmark list
    Given I have set the school with the URN 102976 as my home school
    And I have added 1 schools to the comparison
    When I choose to edit my benchmark list
    And I choose to remove my home school from the benchmark list
    Then I should not see my home school in the benchmark list

  Scenario: No home school displayed on home page or charts page if its not set
    Given I have set the school with the URN 102976 as my home school
    And I have added 1 schools to the comparison
    When I choose to edit my benchmark list
    And I choose to remove my home school as the default benchmark
    Then I should not have a home school set on home page or charts page

  Scenario: Per pupil value is selected by default when viewing benchmarking charts
    Given I have added 2 schools to the comparison
    When I choose to view the benchmarking charts
    Then I should see that per pupil is selected by default

  Scenario Outline: Users can select different chart groups in the Revenue expenditure tab
    Given I have added 2 schools to the comparison
    And I choose to view the benchmarking charts
    When I choose to view <chart_type> benchmarking charts in the expenditure tab
    Then I should see the <chart_type> charts

    Examples:
      | chart_type            |
      | Total expenditure     |
      | Special facilities    |
      | Staff                 |
      | Premises              |
      | Occupation            |
      | Supplies and services |
      | Cost of finance       |

  Scenario Outline: Users can select different chart groups in the Revenue income tab
    Given I have added 2 schools to the comparison
    And I choose to view the benchmarking charts
    When I choose to view <chart_type> benchmarking charts in the income tab
    Then I should see the <chart_type> charts

    Examples:
      | chart_type     |
      | Total income   |
      | Grant funding  |
      | Self-generated |

  Scenario Outline: Users can select different chart groups in the Revenue balance tab
    Given I have added 2 schools to the comparison
    And I choose to view the benchmarking charts
    When I choose to view <chart_type> benchmarking charts in the balance tab
    Then I should see the <chart_type> charts

    Examples:
      | chart_type      |
      | In-year balance |

  Scenario: Totals charts for each child are displayed
    Given I am on the benchmark charts page
    When I choose to view Staff benchmarking charts in the expenditure tab
    Then I should see a Staff total chart

  Scenario: Children of total charts are grouped
    Given I am on the benchmark charts page
    When I choose to view Staff benchmarking charts in the expenditure tab
    Then I should all children charts grouped

  @manual
  Scenario: Accordians can be used to open a child group
    Given I am on the benchmark charts page
    When I choose to view Staff benchmarking charts in the expenditure tab
    And I choose to open a group
    Then I should see the child group open


