class Charts < SitePrism::Section

  element :show_chart_group, '#ChartGroup'
  element :show_value, '#ShowValue'
  element :show_year, '#Year'

  sections :container, '.chartContainer' do
    elements :chart_title, '.heading-medium'
    element :school, '.b-school-link'
    element :balance, '.desktop .lastYearBalance'
    sections :details, '.column-full.no-padding' do
      elements :chart_title, '.heading-medium.chart-accordion-header'
      elements :school, '.b-school-link'
    end
  end
end
