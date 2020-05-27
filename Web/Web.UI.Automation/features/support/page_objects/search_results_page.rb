class SearchResultsPage < SitePrism::Page
  set_url "/SchoolSearch/Search"

  section :search_results, '.search-results' do
    element :heading, '.page-heading'
    section :results, '.js-live-search-results-block' do
      section :order_by, '#SearchFacetsForm2' do
        element :order_by, '#OrderByControl'
      end
      section :filters, '.filter-form' do

        element :selected_counter, '.js-selected-counter'

        element :school_level_opened, 'div[aria-expanded=true] div.option-select-label', text: 'Education phase'
        element :school_level_closed, 'div[aria-expanded=false] div', text: 'Education phase'
        element :primary, '#Primary'
        element :secondary, '#Secondary'
        element :sixteen_plus, 'input[value="16 Plus"]'
        element :not_applicable, '#Notapplicable'
        element :middle_deemed_secondary, '#MiddleDeemedSecondary'

        element :ofsted_rating_opened, 'div[aria-expanded=true] div.option-select-label', text: 'Ofsted rating'
        element :ofsted_rating_closed, 'div[aria-expanded=false] div', text: 'Ofsted rating'
        element :outstanding, "#ofstedrating_1", visible: true
        element :good, "#ofstedrating_2", visible: true
        element :requires_improvement, "#ofstedrating_3", visible: true

        element :school_type_opened, 'div[aria-expanded=true] div.option-select-label', text: 'School type'
        element :school_type_closed, 'div[aria-expanded=false] div', text: 'School type'
        element :community_school, '#schooltype_Communityschool'
        element :other_independent_school, '#schooltype_Otherindependentschool'
        element :voluntary_controlled_school, '#schooltype_Voluntarycontrolledschool'
        element :academy_converter, '#schooltype_Academyconverter'
        element :academy_sponsor_led, '#schooltype_Academysponsorled'
        element :free_schools, '#schooltype_Freeschools'
        element :further_education, '#schooltype_Furthereducation'
        element :foundation_school, '#schooltype_Foundationschool'
        element :higher_education_institutions, '#schooltype_Highereducationinstitutions'
        element :voluntary_aided_school, '#schooltype_Voluntaryaidedschool'
        element :pupil_referral_unit, '#schooltype_Pupilreferralunit'

        element :religious_character_opened, 'div[aria-expanded=true] div.option-select-label', text: 'Religious character'
        element :religious_character_closed, 'div[aria-expanded=false] div', text: 'Religious character'
        element :none, '#faith_None'
        element :does_not_apply, '#faith_Doesnotapply'
        element :no_religious_character, '#faith_Noreligiouscharacter'
        element :church_of_england, '#faith_ChurchofEngland'
        element :roman_catholic, '#faith_RomanCatholic'
        element :methodist, '#faith_Methodist'
        element :united_reformed_church, '#faith_UnitedReformedChurch'
        element :quaker, '#faith_Quaker'
        element :greek_orthodox, '#faith_GreekOrthodox'
        element :moravian, '#faith_Moravian'
        element :seventh_day_adventist, '#faith_SeventhDayAdventist'
        element :other_christian, '#faith_OtherChristian'
        element :muslim, '#faith_Muslim'
        element :hindu, '#faith_Hindu'
        element :buddhist, '#faith_Buddhist'
        element :jewish, '#faith_Jewish'
        element :sikh, '#faith_Sikh'
        element :unitarian, '#faith_Unitarian'
        element :multifaith, '#faith_Multifaith'

      end
      section :result_set, '.column-two-thirds' do
        element :filtered_count, '.result-info .bold'
        sections :schools, 'ul li.document' do
          element :school_link, 'a[href*="/school/detail?urn="]'
          element :metadata, '.metadata'
          element :add_to_comparison, '.addto'
          element :remove_from_comparison, '.removefrom'
        end

      end
    end
  end

  def ofsted_rating_school_count(rating)
    case rating
      when 'outstanding'
        '9'
      when 'good'
        '31'
      when 'requires_improvement'
        '3'
      when 'inadequate'
        '9'
      else
        fail 'Rating not found'
    end
  end


  def religious_character_school_count(character)
    case character
      when 'does_not_apply'
        '9'
      when 'church_of_england'
        '4'
      when 'roman_catholic'
        '8'
      when 'none'
        '25'
      when 'methodist'
        '2'
      when 'united_reformed_church'
        '0'
      when 'quaker'
        '0'
      when 'greek_orthodox'
        '0'
      when 'moravian'
        '0'
      when 'seventh_day_adventist'
        '0'
      when 'other_christian'
        '0'
      when 'muslim'
        '0'
      when 'hindu'
        '0'
      when 'buddhist'
        '0'
      when 'jewish'
        '0'
      when 'sikh'
        '0'
      when 'unitarian'
        '0'
      when 'multifaith'
        '0'
      else
        fail 'Character not found'
    end
  end
end