Feature:
  As a user I want to be able to filter result sets
  So that I can add schools that my criteria to the comparison list

  Scenario Outline: Users can sort their results alphabetically when searching by name
    Given I have searched for Bedford via name
    When I choose to sort my search results from <sort>
    Then I should see the results are sorted <sort>

    Examples:
      | sort  |
      | a - z |
      | z - a |

  @wip
  Scenario Outline: Users can filter results by School level
    Given I have searched for London via name
    When I choose to filter my results by the <level> school_level
    Then I should see filtered schools that are school level <level> only

    Examples:
      | level     |
      | primary   |
      | secondary |
#      | sixteen_plus            |
#      | not_applicable          |

  Scenario Outline: Users can filter results by Ofstead Rating
    Given I want to search for a school via location
    And I enter a valid town like Bedford
    When I choose to filter my results by the <rating> ofsted_rating
    Then I should see filtered schools that are <rating> in ofsted_ratings

    Examples:
      | rating               |
      | outstanding          |
      | good                 |
      | requires_improvement |

  Scenario Outline: Users can filter results by School type
    Given I want to search for a school via location
    And I enter a valid town like Bedford
    When I choose to filter my results by the <type> school_type
    Then I should see filtered schools that are school type <type> only

    Examples:
      | type                        |
      | community_school            |
      | academy_converter           |
      | voluntary_controlled_school |

  Scenario Outline: Users can filter results by Religious character
    Given I want to search for a school via location
    And I enter a valid town like Bedford
    When I choose to filter my results by the <character> religious_character
    Then I should see filtered schools that are <character> in religious_character

    Examples:
      | character         |
      | none              |
      | roman_catholic    |
      | church_of_england |

