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

        element :school_level, '.option-select-label', text: 'Education phase'
        element :primary, '#Primary'
        element :secondary, '#Secondary'
        element :sixteen_plus, 'input[value="16 Plus"]'
        element :not_applicable, '#Notapplicable'
        element :middle_deemed_secondary, '#MiddleDeemedSecondary'

        element :ofsted_rating, '.option-select-label', text: 'Ofsted rating'
        element :outstanding, "input[id='1']", visible: true
        element :good, "input[id='2']", visible: true
        element :requires_improvement, "input[id='3']", visible: true

        element :school_type, '.option-select-label', text: 'School type'
        element :community_school, '#CommunitySchool'
        element :other_independent_school, '#OtherIndependentSchool'
        element :voluntary_controlled_school, '#VoluntaryControlledSchool'
        element :academy_converter, '#AcademyConverter'
        element :academy_sponsor_led, '#AcademySponsorLed'
        element :free_schools, '#FreeSchools'
        element :further_education, '#FurtherEducation'
        element :foundation_school, '#FoundationSchool'
        element :higher_education_institutions, '#HigherEducationInstitutions'
        element :voluntary_aided_school, '#VoluntaryAidedSchool'
        element :pupil_referral_unit, '#PupilReferralUnit'

        element :religious_character, '.option-select-label', text: 'Religious character'
        element :none, '#None'
        element :does_not_apply, '#Doesnotapply'
        element :no_religious_character, '#Noreligiouscharacter'
        element :church_of_england, '#ChurchofEngland'
        element :roman_catholic, '#RomanCatholic'
        element :methodist, '#Methodist'
        element :united_reformed_church, '#UnitedReformedChurch'
        element :quaker, '#Quaker'
        element :greek_orthodox, '#GreekOrthodox'
        element :moravian, '#Moravian'
        element :seventh_day_adventist, '#SeventhDayAdventist'
        element :other_christian, '#OtherChristian'
        element :muslim, '#Muslim'
        element :hindu, '#Hindu'
        element :buddhist, '#Buddhist'
        element :jewish, '#Jewish'
        element :sikh, '#Sikh'
        element :unitarian, '#Unitarian'
        element :multifaith, '#Multifaith'

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
        '7'
      when 'good'
        '29'
      when 'requires_improvement'
        '2'
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
        '3'
      when 'roman_catholic'
        '4'
      when 'none'
        '36'
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