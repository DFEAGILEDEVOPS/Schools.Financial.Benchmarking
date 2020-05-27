Feature:
  As a user I need to view a school's context and financial data in full
  So that I can see drilled down financial data for a school

  Scenario: Per pupil value is selected by default when viewing charts on the school details page
    Given I am on the school details page for the school with the URN 102901
    Then I should see that per pupil is selected by default

  Scenario Outline: Users can select different chart groups in the Revenue expenditure tab on the school details page for a MAT
    Given I am on the school details page for the school with the URN 102901
    When I choose to view <chart_type> benchmarking charts in the expenditure tab on the school details page
    Then I should see the <chart_type> charts on the school details page

    Examples:
      | chart_type            |
      | Total expenditure     |
      | Special facilities    |
      | Staff                 |
      | Premises              |
      | Occupation            |
      | Supplies and services |
      | Cost of finance       |

  Scenario Outline: Users can select different chart groups in the Revenue income tab on the school details page for a MAT
    Given I am on the school details page for the school with the URN 102901
    When I choose to view <chart_type> benchmarking charts in the income tab on the school details page
    Then I should see the <chart_type> charts on the school details page

    Examples:
      | chart_type     |
      | Total income   |
      | Grant funding       |
      | Self-generated |

  Scenario Outline: Users can select different chart groups in the Revenue income tab on the school details page for a MAT
    Given I am on the school details page for the school with the URN 102901
    When I choose to view <chart_type> benchmarking charts in the balance tab on the school details page
    Then I should see the <chart_type> charts on the school details page

    Examples:
      | chart_type      |
      | In-year balance |


  Scenario Outline: Users can select different chart groups in the Revenue expenditure tab on the school details page
    Given I am on the school details page for the school with the URN 102901
    When I choose to view <chart_type> benchmarking charts in the expenditure tab on the school details page
    Then I should see the <chart_type> charts on the school details page

    Examples:
      | chart_type            |
      | Total expenditure     |
      | Special facilities    |
      | Staff                 |
      | Premises              |
      | Occupation            |
      | Supplies and services |
      | Cost of finance       |

  Scenario Outline: Users can select different chart groups in the Revenue income tab on the school details page
    Given I am on the school details page for the school with the URN 102901
    When I choose to view <chart_type> benchmarking charts in the income tab on the school details page
    Then I should see the <chart_type> charts on the school details page

    Examples:
      | chart_type     |
      | Total income   |
      | Grant funding       |
      | Self-generated |

  Scenario Outline: Users can select different chart groups in the Revenue income tab on the school details page
    Given I am on the school details page for the school with the URN 102901
    When I choose to view <chart_type> benchmarking charts in the balance tab on the school details page
    Then I should see the <chart_type> charts on the school details page

    Examples:
      | chart_type      |
      | In-year balance |

  @wip
  Scenario: Total revenue expenditure is a sum of all revenue expenditure areas
    Given I am on the school details page for the school with the URN 102901
    When I select to view the expenditure tab
    And I select to view the Absolute value
    Then I should see that each chart has a value

  @wip
  Scenario: Total revenue income is a sum of all revenue income areas
    Given I am on the school details page for the school with the URN 102901
    When I select to view the income tab
    And I select to view the Absolute value
    Then I should see that each chart has a value

  @wip
  Scenario: Total revenue balance is a sum of all revenue balance areas
    Given I am on the school details page for the school with the URN 102901
    When I select to view the balance tab
    And I select to view the Absolute value
    Then I should see that each chart has a value

  Scenario Outline: Users can view how much each area contributes to the total revenues as a percentage
    Given I am on the school details page for the school with the URN 102901
    When I choose to view benchmarking charts in the <type> tab on the school details page
    And I select to view the Percentage of total value
    Then I should see how much each area contributes to the total <type> as a percentage

    Examples:
      | type        |
      | expenditure |
      | income      |
#      | balance     |


  Scenario Outline: Users can view how much each area contributes to the total revenues per pupil
    Given I am on the school details page for the school with the URN 102901
    When I choose to view benchmarking charts in the <type> tab on the school details page
    And I select to view the Per pupil value
    Then I should see that each chart has a value

    Examples:
      | type        |
      | expenditure |
      | income      |
      | balance     |

  Scenario Outline: Users can view how much each area contributes to the total revenues per teacher
    Given I am on the school details page for the school with the URN 102901
    When I choose to view benchmarking charts in the <type> tab on the school details page
    And I select to view the Per teacher value
    Then I should see that each chart has a value

    Examples:
      | type        |
      | expenditure |
      | income      |
      | balance     |

  Scenario: Previous years financial data available for users to view
    Given I am on the school details page for the school with the URN 102901
    Then I should able to view financial data from previous years