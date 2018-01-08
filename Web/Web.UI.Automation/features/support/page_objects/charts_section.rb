class Charts < SitePrism::Section

  element :show_chart_group, '#ChartGroup'
  element :show_value, '#ShowValue'
  element :show_year, '#Year'

  sections :container, '.chartContainer' do
    elements :chart_title, 'h3.heading-medium'
    element :school, '.b-school-link'
    element :balance, '.desktop .lastYearBalance'
    sections :details, 'details' do
      elements :chart_title, 'h3.heading-medium.chart-accordion-header'
      elements :school, '.b-school-link'
    end
  end
end
