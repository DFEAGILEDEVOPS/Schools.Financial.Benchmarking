class CharacteristicsPage < SitePrism::Page
  set_url '/BenchmarkCriteria/AdvancedCharacteristics?areaType={area_type}&lacode={la_code}&Urn={urn}&ComparisonType={comparison_type}&EstType={est_type}'

  element :view_comparison_charts, '.button', text: 'View benchmarking charts'
  element :back, 'a', text: 'Back'
  element :comparison_school, '.highlight'
  element :open_all_characteristics, '.accordion-expand-all'
  element :clear_all_characteristics, 'a', text: 'Clear all characteristics'
  elements :characteristics, '.question'

  section :info_panel, '.comparisonListInfoPanel', visible: true do
    element :live_count, '#schoolCount'
    element :clear_all, 'a', text: 'Clear all characteristics'
    element :view_comparison_charts, '.view-benchmark-charts'
  end

  section :pop_up, '#js-modal.modal' do
    element :close, '#js-modal-close.modal-close'
    element :title, '#modal-title'
    element :message, '#modal-content', text: 'Please refine the characteristics entered until there are 30 or fewer matched schools.'
  end


end
