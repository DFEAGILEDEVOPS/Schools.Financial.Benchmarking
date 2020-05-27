When(/^I choose to view (.+) benchmarking charts in the (.+) tab on the school details page$/) do |chart_type, tab|
  school_details_page.charts_tabs.send(tab.gsub(' ', '_').gsub('-','_')).click
  chart_type = 'All' if chart_type == 'all'
  school_details_page.charts.show_chart_group.select chart_type if benchmark_charts_page.charts.show_chart_group.visible?
  @tab = tab.gsub(' ', '_')
  wait_for_ajax
  end

When(/^I choose to view benchmarking charts in the (.+) tab on the school details page$/) do |tab|
  school_details_page.charts_tabs.send(tab.gsub(' ', '_').gsub('-','_')).click
  @tab = tab.gsub(' ', '_')
  wait_for_ajax
end

Then(/^I should see all the charts on the school details page$/) do
  expect(school_details_page.charts.container.map{|chart| chart.chart_title.text unless chart.chart_title.text.empty?}.compact.size).to eql school_details_page.chart_groups[@tab.to_sym].values.flatten.inject(0){|sum,x| sum + x }
end

Then(/^I should see the (.*) charts on the school details page$/) do |chart_type|
  expect(school_details_page.charts.container.map{|chart| chart.chart_title.first.text unless chart.chart_title.first.text.empty?}.compact.size).to eql school_details_page.chart_groups[@tab.to_sym][chart_type.gsub('-', ' ').gsub(' ', '_').downcase.to_sym]
end

Then(/^I should see that each chart has a value$/) do
  wait_for_ajax
  school_details_page.charts.container.each {|chart| expect(chart.balance.text).to include "£"}
end

When(/^I select to view the (.+) value$/) do |value_type|
  school_details_page.charts.show_value.select value_type
  wait_for_ajax
end

When(/^I select to view the (.+) tab$/) do |tab|
  school_details_page.charts_tabs.send(tab.gsub(' ', '_').gsub('-','_')).click
end

Then(/^I should see how much each area contributes to the total (.+) as a percentage$/) do |type|
  school_details_page.charts.container.each {|chart| expect(chart.balance.text).to include '%'}
end

Then(/^I should see how much each area contributes to the total (.+) per pupil$/) do |type|
  type = 'in-year balance' if type == 'balance'
  expected_total_value = school_details_page.totals.find {|total_type| total_type.text.downcase.include? type}.text.split('£').last.delete(',').to_i
  total_per_pupil = school_details_page.charts.container.map {|chart| chart.balance.text.gsub(',', '').delete('£').to_i}.inject(0) {|sum, i|  sum + i }
  total_pupils = 209
  expect((total_per_pupil * total_pupils).round(-3)).to eql expected_total_value
end

Then(/^I should see how much each area contributes to the total (.+) per teacher$/) do |type|
  type = 'in-year balance' if type == 'balance'
  expected_total_value = school_details_page.totals.find {|total_type| total_type.text.downcase.include? type}.text.split('£').last.delete(',').to_i
  total_per_teacher = school_details_page.charts.container.map {|chart| chart.balance.text.gsub(',', '').delete('£').to_i}.inject(0) {|sum, i|  sum + i }
  total_teachers = 10
  expect((total_per_teacher * total_teachers).round(-3)).to eql expected_total_value
end

Then(/^I should able to view financial data from previous years$/) do
  expect(school_details_page.charts).to have_show_year
end