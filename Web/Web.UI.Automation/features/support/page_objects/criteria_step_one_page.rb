class CriteriaStepOnePage < SitePrism::Page
  set_url '/BenchmarkCriteria/StepOne?comparisonType={comparison_type}&Urn={urn}'

  element :maintained_schools, '#radio-1'
  element :academies, '#radio-2'
  element :all_schools, '#radio-3'

  element :default_size, '#DefaultAccordion'
  element :custom_size, '#CustomBasketSize'
  element :basket_size, '#BasketSizeInput'

  elements :validation_errors, '.error-summary-list li a'

  element :next, 'button', text: 'Next'
  element :back, 'a', text: 'Back'

end
