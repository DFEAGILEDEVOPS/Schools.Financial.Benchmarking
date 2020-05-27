Given(/^I have navigated to a page that does not exist$/) do
  visit Capybara.app_host + '/ssdkfslk'
end

Then(/^I should be shown an error page$/) do
  expect(error_page).to be_all_there
end
