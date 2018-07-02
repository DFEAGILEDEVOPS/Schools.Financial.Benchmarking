Then(/^I should see an option to add the school to the comparison list$/) do
  expect(school_details_page).to have_add_to_benchmark_basket
end

When(/^I add the school to the comparison list$/) do
  school_details_page.add_to_benchmark_basket.click
end

Then(/^i should see that the school has been added$/) do
  expect(school_details_page).to have_remove_from_comparison
end

Given(/^I have added (\d+) schools to the comparison$/) do |number|
  UrnHelper.collection_of_urns.first(number.to_i).each do |urn|
    school_details_page.load(query: {'urn' => urn})
    step "I add the school to the comparison list"
    sleep 2
  end
end

And(/^I am on the school details page$/) do
  expect(school_details_page).to be_displayed
end

Then(/^I should see a count of (\d+) schools that are in the comparison list$/) do |number|
  expect(school_details_page.comparison_panel.count.text.scan(/\d+/).first).to eql number.to_s
end

Then(/^I should have the option to set the school as my home school$/) do
  expect(school_details_page).to have_make_default_benchmark
end

Given(/^I have set the school with the URN (\d+) as my home school$/) do |urn|
  step "I am on the school details page for the school with the URN #{urn}"
  school_details_page.make_default_benchmark.click
  @school_name = school_details_page.school_name.text
  sleep 2
end

When(/^I navigate to the home page$/) do
  home_page.load
end

Then(/^I should see my home school displayed as my default benchmark$/) do
  expect(home_page.default_benchmark_link.text).to eql @school_name
end

And(/^choose to view details of my home school$/) do
  home_page.default_benchmark_link.click
end

Then(/^I am taken to the details page of my home school$/) do
  expect(school_details_page.school_name.text).to eql @school_name
end

When(/^I choose to edit my benchmark list$/) do
  school_details_page.comparison_panel.edit.click
end

Then(/^I should see my home school in the benchmark list$/) do
  school = benchmark_list_page.list.find {|school| school.has_home_school?(wait: 1)}
  expect(school.home_school.text).to eql @school_name
end

When(/^I choose to view the benchmarking charts$/) do
  school_details_page.comparison_panel.view_benchmarks_charts.click
end

Then(/^I should see my home school as the main comparison$/) do
  expect(benchmark_charts_page.comparing_to.text).to eql "Comparing to #{@school_name}"
end

Then(/^I should see values for my home school in each chart$/) do
  benchmark_charts_page.charts.container.each {|chart| expect(chart.text).to include @school_name}
end

And(/^I have (\d+) schools in my benchmark list$/) do |number|
  step "I have added #{number} schools to the comparison"
end

When(/^I change my home school to another school$/) do
  school_details_page.comparison_panel.edit.click
  benchmark_list_page.list.find{|school| school.has_no_home_school?}.add_benchmark.click
end

Then(/^I should see my home school change to my new choice of school$/) do
  current_home_school = benchmark_list_page.list.find{|m| m.has_home_school?}.school.text
  expect(current_home_school).to_not eql @school_name
end

And(/^I choose to remove my home school from the benchmark list$/) do
  benchmark_list_page.list.find{|school| school.has_home_school?(wait: 1)}.remove_from_benchmark_list.click
  sleep 2
end

Then(/^I should not see my home school in the benchmark list$/) do
  expect(benchmark_list_page.list.find{|school| school.has_home_school?(wait: 1)}).to_not be_visible
end

And(/^I choose to remove my home school as the default benchmark$/) do
  benchmark_list_page.list.find{|school| school.has_home_school?(wait: 1)}.remove_benchmark.click
  sleep 0.5
end

Then(/^I should not have a home school set on home page or charts page$/) do
  home_page.load
  expect(home_page).to have_no_default_benchmark
  expect(home_page).to have_no_default_benchmark_link
  benchmark_charts_page.load
  expect(benchmark_charts_page).to have_no_comparing_to
end

Then(/^I should see that per pupil is selected by default$/) do
  expect(benchmark_charts_page.charts.show_value.value).to eql 'AbsoluteMoney'
end

Then(/^I should see all the charts$/) do
  expect(benchmark_charts_page.charts.container.size).to eql benchmark_charts_page.chart_groups[@tab.to_sym].values.flatten.inject(0){|sum,x| sum + x }
end

When(/^I choose to view (.+) benchmarking charts in the (.+) tab$/) do |chart_type, tab|
  benchmark_charts_page.charts_tabs.send(tab.gsub(' ', '_').gsub('-','_')).click
  chart_type = 'All' if chart_type == 'all'
  wait_for_ajax
  benchmark_charts_page.charts.show_chart_group.select chart_type if benchmark_charts_page.charts.show_chart_group.visible?
  @tab = tab.gsub(' ', '_')
  wait_for_ajax
end

Then(/^I should see the (.+) charts$/) do |chart_type|
  expect(benchmark_charts_page.charts.container.map{|c| c.chart_title.map{|title| title.text unless title.text.empty?}}.compact.size).to eql benchmark_charts_page.chart_groups[@tab.to_sym][chart_type.gsub('-', ' ').gsub(' ', '_').downcase.to_sym]
end

Given(/^I am on the benchmark charts page$/) do
  step "I have added 2 schools to the comparison"
  step "I choose to view the benchmarking charts"
end

Then(/^I should see a (.+) chart$/) do |title_text|
  expect(benchmark_charts_page.charts.container.first.chart_title.first.text).to eql title_text
end

Then(/^I should all children charts grouped$/) do
  charts = benchmark_charts_page.charts.container.reject {|chart| chart.chart_title.first.text == 'Staff total'}
  actual = charts.size
  expected = benchmark_charts_page.chart_groups[@tab.to_sym]['Staff'.downcase.to_sym] - 1 #minus 1 for Staff total chart
  expect(actual).to eql expected
end

And(/^I choose to open a group$/) do
  binding.pry
end