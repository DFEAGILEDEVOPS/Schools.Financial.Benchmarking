def wait_for
  Timeout.timeout(Capybara.default_max_wait_time) do
    begin
      loop until yield
    rescue
    end
  end
end

def wait_for_ajax
  wait_for { page.evaluate_script('jQuery.active').zero? }
end
