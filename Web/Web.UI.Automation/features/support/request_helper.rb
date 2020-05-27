module RequestHelper
  class << self
    BASE_URL = "http://API:+767jjUUhytagkkllPlli212@dfe-api-dev.azurewebsites.net/api/school/"

    def request(urn)
      HTTParty.get(BASE_URL+urn)
    end


  end
end