Given(/^I have searched for (.+) via name$/) do |search_term|
  step "I want to search for a school via name"
  home_page.search_by.name_field.set search_term
  home_page.search_by.name_search.click
  expect(search_results_page).to be_displayed
end

When(/^I choose to sort my search results from (.+)$/) do |sort|
  search_results_page.search_results.results.order_by.select "alphabetical #{sort}"
  @first_school_before_sorting = search_results_page.search_results.results.result_set.schools.first.school_link.text
end

Then(/^I should see the results are sorted (.+)$/) do |sort|
  wait_for_ajax
  school_after_sorting = search_results_page.search_results.results.result_set.schools.first.school_link.text
  if sort == 'a - z'
    expect(school_after_sorting[0]).to eql @first_school_before_sorting[0]
  else
    expect(school_after_sorting[0]).to eql 'T'
  end
end

When(/^I choose to filter my results by the (.+) (school_level|ofsted_rating|school_type|religious_character)$/) do |option,filter|
  filter = search_results_page.search_results.results.filters.send(filter)
  filter.click unless search_results_page.search_results.results.filters.send("has_#{option}?")
  search_results_page.search_results.results.filters.send(option).trigger('click')
  search_results_page.search_results.results.filters.wait_for_selected_counter
end

Then(/^I should see filtered schools that are (school level|school type) (.+) only$/) do |filter, level|
  level = '16 Plus' if level == 'sixteen_plus'
  wait_for_ajax
  search_results_page.search_results.results.result_set.schools.each {|school| expect(school.metadata.text.downcase).to include level.gsub('_', ' ').downcase}
end

Then(/^I should see filtered schools that are (.+) in ofsted_ratings$/) do |value|
  wait_for_ajax
  expect(search_results_page.search_results.results.result_set.filtered_count.text).to eql search_results_page.ofsted_rating_school_count(value)
end


Then(/^I should see filtered schools that are (.+) in religious_character/) do |value|
  wait_for_ajax
  expect(search_results_page.search_results.results.result_set.filtered_count.text).to eql search_results_page.religious_character_school_count(value)
end
