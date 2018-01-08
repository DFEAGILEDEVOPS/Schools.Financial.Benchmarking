class ChartsTabs < SitePrism::Section

  element :active_tab, 'li.active'
  element :expenditure, 'a', text: 'Expenditure'
  element :income, 'a', text: 'Income'
  element :balance, 'a', text: 'Balance'

end