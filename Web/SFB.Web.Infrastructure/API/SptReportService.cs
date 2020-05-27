using SFB.Web.Infrastructure.ApiWrappers;
using System.Collections.Generic;
using System.Net;

namespace SFB.Web.ApplicationCore.Services.SptReport
{
    public class SptReportService : ISptReportService
    {
        private readonly IApiRequest _apiRequest;

        public SptReportService(IApiRequest apiRequest)
        {
            _apiRequest = apiRequest;
        }

        public bool SptReportExists(int urn)
        {
            return _apiRequest.Head("/estab-details/", new List<string> { urn.ToString() }).statusCode == HttpStatusCode.OK;
        }
    }
}
