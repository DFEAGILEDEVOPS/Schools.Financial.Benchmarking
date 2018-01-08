class ComparisonStrategyPage < SitePrism::Page
  set_url "/BenchmarkCriteria/ComparisonStrategy{?query*}"

  element :dfe_defined, '#radio-1'
  element :user_defined, '#radio-2'
  element :next, 'button', text: 'Next'
  element :back, 'a', text: 'Back'

end
