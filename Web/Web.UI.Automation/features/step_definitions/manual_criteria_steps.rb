Then(/^I should see the option to compare to similar schools$/) do
  expect(school_details_page).to have_compare_with_similar_schools
end

When(/^I choose to compare to similar schools$/) do
  school_details_page.compare_with_similar_schools.click
end

Then(/^I should be taken to the comparison strategy page$/) do
  expect(comparison_strategy_page).to be_displayed
end

But(/^decide I want to go back$/) do
  comparison_strategy_page.back.click
end

Then(/^I should be taken back to the school details page$/) do
  expect(current_url.downcase).to include school_details_page.url.downcase
end

Given(/^I am on the comparison strategy page for the school with the urn (\d+)$/) do |urn|
  @urn = urn
  comparison_strategy_page.load(query: {urn: @urn})
end

When(/^I decide to choose the characteristics that i have chosen$/) do
  comparison_strategy_page.user_defined.set true
  comparison_strategy_page.next.click
end

Then(/^I should be taken to step one of the criteria process$/) do
  expect(current_url).to include criteria_step_one_page.url(comparison_type: 'Advanced', urn: @urn)
end

Given(/^I am on step one of the criteria process using school (\d+)$/) do |urn|
  criteria_step_one_page.load(comparison_type: 'Advanced', urn: urn)
end

When(/^I choose (.+) for comparison$/) do |type|
  criteria_step_one_page.send(type).set true
  criteria_step_one_page.next.click
end

Then(/^I should be taken to step two of the criteria process$/) do
  expect(criteria_step_two_page).to be_displayed
end

Given(/^I have chosen (.+) to be my comparison using school (\d+)$/) do |type, urn|
  type = 'all' if type == 'all_schools'
  characteristics_page.load({est_type: type, urn: urn, comparison_type: 'Advanced', area_type: 'All'})
end

Then(/^I should see school (\d+) as my baseline comparison$/) do |urn|
  expect(current_url).to include urn
  expect(characteristics_page.comparison_school.text).to eql 'Bedford College'
end

Then(/^I should see a list of characteristics$/) do
  expect(characteristics_page).to have_characteristics
end

When(/^I should be able to add (.+) as my characteristic$/) do |value|
  # @value = value
  characteristics_page.characteristics.gender_of_pupils.last.checkbox.trigger('click')
  characteristics_page.characteristics.gender_of_pupils.first.panel.boys.click
end

Given(/^I have added the Gender of school pupils characteristic$/) do
  step "I have chosen all_schools to be my comparison using school 102901"
  step "I should be able to add Gender of pupils as my characteristic"
end

When(/^I decide to remove the Gender of school pupils characteristic$/) do
  characteristics_page.characteristics.gender_of_pupils.last.checkbox.click
end

Then(/^I should not see Gender of school pupils selected on the page$/) do
  expect(characteristics_page.characteristics.gender_of_pupils.first.panel).to_not be_visible
end

Then(/^I should be able to select the gender as mixed$/) do
  characteristics_page.characteristics.gender_of_pupils.first.panel.mixed.click
end

Then(/^I should see a benchmark value for my baseline school$/) do
  expect(characteristics_page.characteristics.gender_of_pupils.first.panel.benchmark_school_value.text).to eql 'Your default school\'s value is Mixed'
end

Then(/^I should not be able to add (.+) as another characteristic$/) do |value|
  expect(characteristics_page.characteristics.last.options.last.text).to_not include value
end

Then(/^I should see search is disabled$/) do
  expect(characteristics_page.search).to be_disabled
end

When(/^I choose to compare to similar schools using pre selected characteristics$/) do
  school_details_page.compare_with_similar_schools.click
  comparison_strategy_page.dfe_defined.click
  comparison_strategy_page.next.click
end

Then(/^I should be taken to the quick comparison strategy page$/) do
  expect(criteria_step_one_page).to be_displayed
end

Given(/^I am on the quick comparison strategy page for the school with the URN (.+)$/) do |urn|
  criteria_step_one_page.load(comparison_type: 'Basic', urn: urn)
end

Then(/^I should be able to select the default basket size$/) do
  expect(criteria_step_one_page).to have_default_size
end

Then(/^I should be able to define my own basket size$/) do
  criteria_step_one_page.custom_size.click
  expect(criteria_step_one_page).to have_basket_size
end

When(/^I enter a custom basket size of less than (\d+)$/) do |number|
  criteria_step_one_page.custom_size.click
  criteria_step_one_page.basket_size.set (number.to_i - 1)
  criteria_step_one_page.next.click
end

Then(/^I should see a validation error$/) do
  expect(criteria_step_one_page).to have_validation_errors
end

When(/^I enter a custom basket size of more than (\d+)$/) do |number|
  criteria_step_one_page.custom_size.click
  criteria_step_one_page.basket_size.set (number.to_i + 1)
  criteria_step_one_page.next.click
end

Then(/^I should see a live counter$/) do
  wait_for_ajax
  expect(characteristics_page.info_panel.live_count.text).to eql '209'
end

Given(/^I have added critera that does not match any schools$/) do
  step "I have added the Gender of school pupils characteristic"
  characteristics_page.characteristics.admission_policy.last.checkbox.click
  characteristics_page.characteristics.admission_policy.first.panel.non_selective.click
end

Then(/^I should not see a live counter$/) do
  expect(characteristics_page.info_panel.live_count.text).to eql '0'
end

When(/^I want to clear all characteristics using the info panel$/) do
  characteristics_page.info_panel.clear_all.click
end

Then(/^all characteristics should be cleared$/) do
  expect(characteristics_page.characteristics.gender_of_pupils.first.panel).to_not be_visible
end

Then(/^I should be able to view the benchmark charts$/) do
  expect(characteristics_page.info_panel).to have_view_comparison_charts
end

When(/^I want to view the charts$/) do
  characteristics_page.info_panel.view_comparison_charts.click
end

Then(/^I should see a pop up with the school count$/) do
  characteristics_page.wait_for_pop_up
  expect(characteristics_page.pop_up.title.text).to include '209 matches found'
end

And(/^a message that states I should refine the search$/) do
  characteristics_page.wait_for_pop_up
  expect(characteristics_page.pop_up).to have_message
end

When(/^I close the pop up$/) do
  characteristics_page.wait_for_pop_up
  characteristics_page.pop_up.close.click
end

Given(/^the pop up is visible$/) do
  step "I have added the Gender of school pupils characteristic"
  wait_for_ajax
  characteristics_page.view_comparison_charts.click
end

And(/^I should not see the pop up$/) do
  expect(characteristics_page).to have_no_pop_up
end

But(/^my school count is over the limit$/) do
  wait_for_ajax
  expect(characteristics_page.info_panel.live_count.text.to_i).to be > 30
end

Given(/^I have selected my criteria$/) do
  school_details_page.load(query: {'urn' => '102976'})
  school_details_page.compare_with_similar_schools.click
  comparison_strategy_page.user_defined.click
  comparison_strategy_page.next.click
  criteria_step_one_page.all_schools.click
  criteria_step_one_page.next.click
  criteria_step_two_page.all_of_england.click
  criteria_step_two_page.next.click
  step "I should be able to add Gender of pupils as my characteristic"
  characteristics_page.characteristics.admission_policy.last.checkbox.click
  characteristics_page.characteristics.admission_policy.first.panel.modern.click
end

And(/^I am within the limit of schools$/) do
  wait_for_ajax
  expect(characteristics_page.info_panel.live_count.text.to_i).to be < 30
end

When(/^I perform the search using a (.+)$/) do |basket_type|
  wait_for_ajax
  characteristics_page.view_comparison_charts.click
  basket_type = 'add_to_existing_basket' if basket_type == 'existing_basket'
  basket_page.send(basket_type).click
  basket_page.next.click
end

Then(/^I should be taken to the charts page$/) do
  expect(benchmark_charts_page).to be_displayed
end

Given(/^I have added the Gender of school pupils characteristic for school (\d+)$/) do |urn|
  step "I have chosen all_schools to be my comparison using school #{urn}"
  step "I should be able to add Gender of pupils as my characteristic"
end


When(/^I perform the search$/) do
  wait_for_ajax
  characteristics_page.view_comparison_charts.click
end

Then(/^I should be able to view my chosen characteristics$/) do
  expect(benchmark_charts_page.edit_characteristics.rows.
      map { |row| row.values.map { |value| value.text } }.flatten).to eql ["School type", "Maintained",
                                                                           "All", "Area", "Sutton", "All England", "Gender of pupils",
                                                                           "Mixed", "Boys", "Admissions policy", "N/A", "Modern"]
end

Then(/^I should be able to edit characteristics$/) do

end

Given(/^I am on the charts page after selecting characteristics$/) do
  step 'I have selected my criteria'
  step 'I perform the search'
end

When(/^I decide to edit my characteristics$/) do
  benchmark_charts_page.edit_characteristics.summary.click
  benchmark_charts_page.edit_characteristics.edit.click
end

Then(/^I should be on the characteristics page$/) do
  expect(characteristics_page).to have_characteristics
end

Given(/^I have to updated my characteristics$/) do
  step 'I have selected my criteria'
  step 'I perform the search'
  benchmark_charts_page.edit_characteristics.summary.click
  benchmark_charts_page.edit_characteristics.edit.click
  characteristics_page.characteristics.admission_policy.first.panel.non_selective.click
  wait_for_ajax
  characteristics_page.view_comparison_charts.click
  basket_page.next.click
end


Then(/^I should see my updated choices on the benchmark charts page$/) do
  expect(benchmark_charts_page.edit_characteristics.rows.
      map { |row| row.values.map { |value| value.text } }.flatten).to eql ["School type", "Maintained",
                                                                           "All", "Area", "Sutton", "All England", "Gender of pupils",
                                                                           "Mixed", "Boys", "Admissions policy", "N/A", "Modern, Non-selective"]
end