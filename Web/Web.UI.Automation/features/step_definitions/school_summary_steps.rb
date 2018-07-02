Given(/^I am on the school details page for the school with the URN (.+)$/) do |urn|
  @urn = urn
  school_details_page.load(query: {'urn' => @urn})
end

Then(/^I should be able to view a summary of the school$/) do
  expected = school_details_page.expected_summary_details_from(@urn).values.sort
  actual = school_details_page.details.map{|detail| detail.text}.reject(&:empty?).sort
  binding.pry
  expected.map{|a| a.gsub(' ', '')}.each {|value| expect(actual.map{|a| a.gsub(' ', '')}).to include value}
end

And(/^I should see a map indicating the location of the school$/) do
  expect(school_details_page).to have_map
end

Given(/^I am on the MAT index page for (.*)$/) do |mat_no|
  mat_index_page.load(mat_no: mat_no)
end

Then(/^I should see the academies that make up (.*)$/) do |mat_no|
  expect(mat_index_page.result_list.academies.map{|trust| trust.text}).to eql ["SOUTHMOOR ACADEMY", "Sandhill View Academy"]
end

Then(/^I should see the academies (.*) make up (.*)$/) do |academy_urns, mat_no|
  expect(mat_index_page.result_list.academies.map{|trust| trust['href'].split('urn=').last}).to eql academy_urns.split(',')
end