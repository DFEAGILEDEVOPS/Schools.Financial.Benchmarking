using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Controllers
{
    public class SelfAssessmentDashboardController : Controller
    {
        private readonly ISelfAssessmentModalContentService _contentService;
        
        public SelfAssessmentDashboardController(ISelfAssessmentModalContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet, Route("SelfAssessmentDashboard/{urn}")]
        public async Task<ActionResult> Index(int urn)
        {
            var vm = new SelfAssessmentDashboardViewModel
            {
                DashboardData = await GetSelfAssessmentData(urn),
                ModalMappings = _contentService.GetAllSadModals()
            };

            return View(vm);
        }

       

        private static readonly HttpClient Client = new HttpClient();

        private async Task<SelfAssesmentModel> GetSelfAssessmentData(int urn)
        {
            var path = string.Concat("https://", 
                ConfigurationManager.AppSettings["SfbApiUrl"], 
                "/api/selfassessment/",
                urn);
            SelfAssesmentModel saModel = null;
            
            HttpResponseMessage response = await Client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                saModel = JsonConvert.DeserializeObject<SelfAssesmentModel>(jsonString);
                
            }

            return saModel;
        }
    }
}