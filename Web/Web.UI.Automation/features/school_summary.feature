@wip
Feature:
  As a user,
  I would like to see a summary of a particular school
  So I can quickly and easily view data relevant to me

  Scenario Outline: View schools summary data
    Given I am on the school details page for the school with the URN <urn>
    Then I should be able to view a summary of the school
    And I should see a map indicating the location of the school

    Examples:
      | urn    |
      | 102901   |
      | 102981 |

  Scenario Outline: View academies in a MAT
    Given I am on the MAT index page for <mat_no>
    Then I should see the academies <academy_urns> make up <mat_no>

    Examples:
      | mat_no | academy_urns  |
      | MAT904 | 138103,141986 |
