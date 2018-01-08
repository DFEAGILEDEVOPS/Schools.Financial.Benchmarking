require 'active_support'
require 'capybara'
require 'capybara/dsl'
require 'cucumber'
require 'capybara/cucumber'
require 'cucumber/rake/task'
require 'rake'
require 'rake/task'
require 'require_all'
require 'rspec'
require 'rubygems'
require 'selenium/webdriver'
require 'site_prism'
require 'pry'
require 'settingslogic'
require 'capybara/poltergeist'
require 'httparty'
require 'capybara-screenshot/cucumber'

raise 'Please pass a BASE_URL with the cucumber command.' unless ENV['BASE_URL']

Capybara.configure do |config|
  config.default_driver = :chrome
  config.app_host = ENV["BASE_URL"]
  config.exact_options = true
  config.ignore_hidden_elements = false
  config.visible_text_only = true
  config.default_max_wait_time = 30
end

Capybara.register_driver(:chrome) do |app|
  Capybara::Selenium::Driver.new(app, browser: :chrome)
end

Capybara.register_driver :poltergeist do |app|
  Capybara::Poltergeist::Driver.new(app, js_errors: false, timeout: 60)
end

Capybara::Screenshot.register_filename_prefix_formatter(:cucumber) do |scenario|
  "/screenshots/screenshot_#{scenario.title.tr(' ', '-').gsub(%r{/^.*\/cucumber\//}, '')}"
end

Capybara.javascript_driver = :poltergeist
