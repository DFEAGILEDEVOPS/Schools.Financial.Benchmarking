class CriteriaStepTwoPage < SitePrism::Page
  set_url '/BenchmarkCriteria/ChooseRegion?EstType={est_type}&Urn={urn}&ComparisonType={comparison_type}&BasketSize='

  element :all_of_england, '#All'

  element :la_code, '#LaCodeAccordion'
  element :la_code_field, '#FindSchoolByLaCode'

  element :la_name, '#LaCodeAccordion'
  element :la_name_field, '#FindSchoolByLaName'

  element :next, 'button', text: 'Next'
  element :back, 'a', text: 'Back'

end
