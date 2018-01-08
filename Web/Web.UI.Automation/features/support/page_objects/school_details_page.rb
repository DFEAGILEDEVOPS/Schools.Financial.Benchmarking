class SchoolDetailsPage < SitePrism::Page
  set_url "/school/detail{?query*}"

  element :school_name, '.page-heading'
  elements :details, '.metadata-school-detail .values'
  element :add_to_benchmark_basket, '.button', text: 'Add to benchmark basket'
  element :remove_from_comparison, '.button', text: 'Remove from benchmark basket'
  element :make_default_benchmark, 'a', text: 'Set as default school'
  element :compare_with_similar_schools, '.button', text: 'Compare with other schools'
  element :map, '#SchoolLocationMap'

  sections :totals, '.revenueDiv' do
    element :total_title, 'h3.heading-medium'
    element :total_amount, 'span.heading-medium'
  end

  section :comparison_panel, '.comparisonListInfoPanel' do
    element :count, '.count'
    element :edit, 'a', text: 'Edit basket'
    element :view_benchmarks_charts, 'a.view-benchmark-charts'
  end

  section :charts_tabs, ChartsTabs, '.tabs'
  section :charts, Charts, '.chartsSection'

  def expected_summary_details_from(urn)
    response = RequestHelper.request(urn)
    school_level = calculate_school_level(response)
    total_pupils = calculate_total_number_of_pupils(response)
    website = calculate_website(response)
    ofsted_rating = calculate_ofsted_rating(response['OFSTED_INSPOUTCOME'])
    ht_name = response['HEADTEACHERFULLNAME'].split
    {
        address: "#{response['STREET']}, #{response["TOWN"]}, #{response['POSTCODE']}",
        contact_number: response['TELNUM'],
        school_type: response['EDUB_TYPEOFESTABLISHMENT'],
        school_level: school_level,
        # number_of_pupils: total_pupils,
        urn: response['id'],
        ofsted_rating: ofsted_rating,
        laestab: response['LAESTAB'],
        age_range: "#{response['AGEL']} to #{response['AGEH']}",
        head_teacher: "#{ht_name[1]} #{ht_name[2]}",
        has_6th_form: response['ISPOST16']=='1' ? 'Yes' : 'No',
        has_nursery: response['AGEL'] <= '1' ? 'Yes' : 'No',
        website: website,
        # download: "Download data for #{urn}"
    }.delete_if { |key, value| value.strip == '' }
  end

  def calculate_ofsted_rating(rating)
    if rating
      rate_mappings = {'1': '1Outstanding', '2': '2Good', '3': '3Satisfactory', '4': '4Inadequate'}
      rate_mappings[rating.to_sym]
    else
      ''
    end
  end

  def calculate_school_level(response)
    @array = []
    response['ISPRIMARY']=='1' ? @array << 'Primary' : ''
    response['ISSECONDARY']=='1' ? @array << 'Secondary' : ''
    response['ISPOST16']=='1' ? @array << '16 to 18' : ''
    @array.join(' and ')
  end

  def calculate_website(response)
    if !response['EDUB_WEBSITEADDRESS'].include?('http://') && response['EDUB_WEBSITEADDRESS'].include?('www')
      'http://' + response['EDUB_WEBSITEADDRESS']
    else
      response['EDUB_WEBSITEADDRESS']
    end
  end

  def calculate_total_number_of_pupils(response)
    case
      when !response['ks4'].empty?
        response['ks4'].first['TOTPUPS']
      when !response['ks2'].empty?
        response['ks2'].first['TOTPUPS']
      when !response['census'].empty?
        response['census']['NOR']
      when !response['cfr'].empty?
        response['cfr']['PUPILS']
      else
        ''
    end
  end

  def chart_groups
    {expenditure: {
        total_expenditure: 8,
        special_facilities: 1,
        staff: 13,
        premises: 6,
        occupation: 9,
        supplies_and_services: 9,
        cost_of_finance: 3,
        community: 1
    },
     income: {
         total_income: 3,
         grant_funding: 14,
         self_generated: 8
     },
     balance: {
         in_year_balance: 1
     }
    }
  end
end