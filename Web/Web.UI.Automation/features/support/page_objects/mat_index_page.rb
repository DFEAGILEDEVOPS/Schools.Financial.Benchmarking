class MatIndexPage < SitePrism::Page
  set_url "/trust/index?matno={mat_no}&name={name}"

  element :heading, 'heading-xlarge'
  section :result_list, '.resultListPanel ul' do
    elements :academies, 'li a'
  end
end
