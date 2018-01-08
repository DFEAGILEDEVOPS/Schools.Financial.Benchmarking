class ErrorPage < SitePrism::Page

  element :error_title, 'h1', text: 'Page not found'
  element :error_text, 'p', text: 'If you entered a web address, please check it was correct.'
  element :error_instructions, 'p', text: 'You can also search GOV.UK or browse from the homepage to find the information you need.'
  element :search_gov, "a[href='http://www.gov.uk/search']"
  element :browser_gov, "a[href='http://www.gov.uk']"
end
