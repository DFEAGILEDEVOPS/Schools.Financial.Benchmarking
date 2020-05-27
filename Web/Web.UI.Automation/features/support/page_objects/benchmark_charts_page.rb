class BenchmarkChartsPage < SitePrism::Page
  set_url "/BenchmarkCharts"

  element :comparing_to, '.column-full', text: 'Comparing to'

  section :charts_tabs, ChartsTabs, '.tabs'
  section :charts, Charts, '.chartsSection'

  section :edit_characteristics, '.criteria-details' do
    element :summary, '.summary'
    sections :rows, 'tr' do
      elements :values, 'td'
    end
    element :edit, '.link-button'
  end

  def chart_groups
    {expenditure: {
        total_expenditure: 8,
        special_facilities: 1,
        staff: 6,
        premises: 4,
        occupation: 7,
        supplies_and_services: 4,
        cost_of_finance: 1,
        community: 3
    },
     income: {
         total_income: 3,
         grant_funding: 4,
         self_generated: 8
     },
     balance: {
         in_year_balance: 2
     }
    }
  end
end
