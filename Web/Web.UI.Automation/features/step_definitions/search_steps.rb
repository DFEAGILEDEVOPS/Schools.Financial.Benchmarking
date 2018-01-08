Given(/^I want to search for a school via name$/) do
  home_page.load
  home_page.search_by.name.click
end

When(/^I enter the first (\d+) letters of (.+)$/) do |number, name|
  home_page.search_by.name_field.set name[0, 3]
end

Then(/^I should see a results list dropdown$/) do
  expect(home_page).to have_results_drop_down
end

When(/^select the first option$/) do
  home_page.wait_for_results_drop_down(2)
  @suggestion_value = home_page.results_drop_down.first.suggestion.text
  home_page.results_drop_down.first.suggestion.click
end

Then(/^the search field should be populated with the selected option$/) do
  expect(home_page.search_by.name_field.value).to eql @suggestion_value
end

When(/^I enter a school which has no results$/) do
  home_page.search_by.name_field.set 'Non Existent School'
end

Then(/^I should not see any results$/) do
  expect(home_page).to have_no_results_drop_down
end

Given(/^I have selected an option from the results list dropdown$/) do
  home_page.load
  home_page.perform_search_via('name', 'Bedford')
  home_page.wait_for_results_drop_down(2)
  home_page.results_drop_down.first.suggestion.click
end

When(/^I select search$/) do
  home_page.search_by.name_search.click
end

Then(/^I should be taken to the school details page$/) do
  expect(school_details_page).to be_displayed
  @urn = current_url.split('urn=').last
end

Given(/^I want to search for a school via location$/) do
  home_page.load
  home_page.search_by.location.click
end

When(/^I enter a valid postcode like (.+)$/) do |postcode|
  home_page.search_by.location_field.set postcode
  home_page.search_by.location_search.click
end

Then(/^I should see a search results page$/) do
  expect(search_results_page).to be_displayed
end

When(/^I enter a valid town like (.+)$/) do |town|
  home_page.search_by.location_field.set town
  home_page.search_by.location_search.click
end

When(/^I enter a valid street like (.+)$/) do |street|
  home_page.search_by.location_field.set street
  home_page.search_by.location_search.click
end

Given(/^I want to search for a school via LA code$/) do
  home_page.load
  home_page.search_by.la_name.click
end

When(/^I search for a valid LA code like (\d+)$/) do |la_code|
  home_page.search_by.la_name_field.set la_code
  home_page.search_by.la_name_search.click
  wait_for_ajax
end

Then(/^I should only see schools from (.+)$/) do |location|
  search_results_page.search_results.results.result_set.schools.each { |school| expect(school.metadata.text.downcase).to include location.downcase }
end

Given(/^I want to search for a school via maintained school id$/) do
  home_page.load
  home_page.search_by.name.click
end

When(/^I search for a valid maintained school id code using urn like (\d+)$/) do |urn|
  home_page.search_by.name_field.set urn
  home_page.search_by.name_search.click
end

Then(/^I should be taken to details page for the school with the urn (\d+)$/) do |urn|
  expect(current_url.downcase).to include school_details_page.url(query: {urn: urn})
end

When(/^I search for a valid maintained school id code using laestab like (\d+)$/) do |laestab|
  home_page.search_by.name_field.set laestab
  home_page.search_by.name_search.click
end

Then(/^I should be taken to details page for the school with the laestab (\d+)$/) do |laestab|
  expect(school_details_page.details.map { |detail| detail.text.gsub(' ', '')}).to include laestab
end

Given(/^I want to search for a trust via name$/) do
  home_page.load
  home_page.search_by.trust_name.click
end

When(/^I enter the (\d+) letters of the (.+) trust$/) do |number,trust_name|
  home_page.search_by.trust_name_field.set trust_name[0, 3]
end

When(/^I enter a trust which has no results$/) do
  home_page.search_by.trust_name_field.set 'Non Existent Trust'
end

Then(/^I should be taken to mat index page for the mat with id (.+)$/) do |mat_no|
  expect(current_url).to include mat_index_page.url(mat_no: mat_no)
end

Given(/^I want to search for a local authority via name$/) do
  home_page.load
  home_page.search_by.la_name.click
end

When(/^I enter the (\d+) letters of the (.+) authority$/) do |number,la_name|
  home_page.search_by.la_name_field.set la_name[0, 3]
end

When(/^I enter a local authority which has no results$/) do
  home_page.search_by.la_name_field.set 'Non Existent Local Authority'
end

Then(/^I should see the search results page$/) do
  expect(current_url).to include search_results_page.url.downcase
end