class CharacteristicsPage < SitePrism::Page
  set_url '/BenchmarkCriteria/AdvancedCharacteristics?areaType={area_type}&lacode={la_code}&Urn={urn}&ComparisonType={comparison_type}&EstType={est_type}'

  element :view_comparison_charts, '.button', text: 'View benchmarking charts'
  element :back, 'a', text: 'Back'
  element :comparison_school, '.highlight'
  element :clear_all_characteristics, 'a', text: 'Clear all characteristics'

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

  section :characteristics, '#criteriaForm' do
    sections :gender_of_pupils, '.question', text: "Gender of pupils" do
      element :checkbox, '#checkbox-gender'
      section :panel, '.panel-border-narrow' do
        element :benchmark_school_value, '.benchmark-school-value'
        element :error_message, '.error-message'
        element :boys, "input[value='Boys']"
        element :girls, "input[value='Girls']"
        element :mixed, "input[value='Mixed']"
      end
    end
    sections :admission_policy, '.question', text: "Admissions policy" do
      element :checkbox, '#checkbox-admission'
      section :panel, '.panel-border-narrow' do
        element :benchmark_school_value, '.benchmark-school-value'
        element :error_message, '.error-message'
        element :comprehensive, "input[value='Comprehensive']"
        element :selective, "input[value='Selective']"
        element :modern, "input[value='Modern']"
        element :non_selective, "input[value='Non-selective']"
        element :n_a, "input[value='N/z']"
      end
    end
  end

end
