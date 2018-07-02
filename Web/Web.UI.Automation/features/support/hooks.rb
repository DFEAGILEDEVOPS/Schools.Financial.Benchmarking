After do |scenario|
  if scenario.failed?
    time = Time.now.strftime("%H_%M_%S")
    embed("data:image/png;base64,#{Capybara.current_session.driver.browser.screenshot_as(:base64)}", 'image/png', 'Failure')
    page.save_screenshot("screenshots/#{scenario.name.downcase.gsub(' ', '_')}_#{time}.png")
    p "Screenshot raised - " + "screenshots/#{scenario.name.downcase.gsub(' ', '_')}_#{time}.png"
  end
end