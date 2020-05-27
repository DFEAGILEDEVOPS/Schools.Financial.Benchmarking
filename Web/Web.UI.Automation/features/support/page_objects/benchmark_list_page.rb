class BenchmarkListPage < SitePrism::Page
  set_url "/benchmarklist"

  element :view_charts, '.view-benchmark'

  sections :list, '.benchmark-list ul .document' do
    element :home_school, '.highlight a[href*="/school/detail?urn"]'
    element :school, 'div a[href*="/school/detail?urn"]'
    element :details, '.metadata'
    element :remove_benchmark, '.make-benchmark a', text: 'Remove as default school'
    element :add_benchmark, '.make-benchmark a', text: 'Use as default school'
    element :remove_from_benchmark_list, '.remove-benchmark'
  end
end