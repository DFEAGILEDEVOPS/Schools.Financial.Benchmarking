class BasketPage < SitePrism::Page
  set_url "/BenchmarkCriteria/OverwriteStrategy"

  element :new_basket, '#radio-1'
  element :add_to_existing_basket, '#radio-2'
  element :next, 'button', text: 'Next'
  element :back, 'a', text: 'Back'

end
