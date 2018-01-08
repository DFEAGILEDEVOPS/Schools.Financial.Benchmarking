class HomePage < SitePrism::Page
  set_url '/'

  section :search_by, '#finderSection' do
    element :name, '#SchoolOrCollegeNameId'
    element :name_field, '#FindByNameId'
    element :name_search, 'button[value="search-by-name-id"]'

    element :location, '#TownOrCity'
    element :location_field, '#FindSchoolByTown'
    element :location_search, 'button[value="search-by-location"]'

    element :la_name, '#LaCodeName'
    element :la_name_field, '#FindSchoolByLaCodeName'
    element :la_name_search, 'button[value="search-by-la-code-name"]'

    element :trust_name, '#TrustName'
    element :trust_name_field, '#FindByTrustName'
    element :trust_name_search, 'button[value="search-by-trust-name"]'
  end

  sections :results_drop_down, '.tt-menu.tt-open .tt-dataset .tt-suggestion' do
    element :suggestion, 'a'
  end

  element :default_benchmark, '.heading-small', text: 'My Default Benchmark'
  element :default_benchmark_link, 'a[href*="/school/detail?urn"]'

  def perform_search_via(type, value)
    search_by.send("#{type}").click
    search_by.send("#{type}_field").set value
  end

end
