using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFB.Web.UI.Attributes;
using SFB.Web.UI.Helpers.Enums;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;

namespace SFB.Web.UI.Controllers
{
    [CustomAuthorize]
    public class TrustSelfAssessmentController : Controller
    {
        private readonly ISelfAssessmentModalContentService _contentService;

        public TrustSelfAssessmentController(ISelfAssessmentModalContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet, Route("TrustSelfAssessment/{uid}/{category}", Name = "TrustSelfAssessment")]
        public async Task<ActionResult> Index(int uid, SadCategories category = SadCategories.InYearBalance)
        {
            if (string.IsNullOrEmpty(category.ToString()))
            {
                category = SadCategories.InYearBalance;
            }

            var vm = await GetEstablishmentSelfAssessmentData(uid);

            vm.CurrentCategory = category;
            vm.ModalMappings =  _contentService.GetAllSadModals();
            return View(vm);
        }

        [HttpGet, Route("TrustSelfAssessment/summary/{uid}")]
        public async Task<ActionResult> GetSummaryAsync(int uid)
        {
            var result = await GetEstablishmentSelfAssessmentData(uid);
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("TrustSelfAssessment/Help/{id}", Name = "TrustSelfAssessmentHelp")]
        public JsonResult Help(int id)
        {
            var content = _contentService.GetById(id);

            return new JsonResult
            {
                Data = new
                {
                    id = content.Id,
                    title = content.Title,
                    content = content.Content,
                    assessmentArea = content.AssessmentArea
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };
        }

        [HttpGet, Route("TrustSelfAssessment/Help/All", Name = "TrustSelfAssessmentHelpAll")]
        public JsonResult HelpText()
        {
            var modalModels = _contentService.GetAllSadModals();
        
            var list = new List<dynamic>();
            
            foreach (var modal in modalModels)
            {
                list.Add(new 
                    {
                        id = modal.Id,
                        title = modal.Title,
                        content = modal.Content,
                        assessmentArea = modal.AssessmentArea
                    }
                );
            }

            return new JsonResult
            {
                Data = list,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

        }


        private static readonly HttpClient Client = new HttpClient();
        private async Task<TrustSelfAssessmentModel> GetEstablishmentSelfAssessmentData(int uid)
        {
            var path = string.Concat("https://", ConfigurationManager.AppSettings["SfbApiUrl"], "/api/SelfAssessment/trust/", uid.ToString()) ;
            TrustSelfAssessmentModel saModel = null;
            HttpResponseMessage response = await Client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                saModel = JsonConvert.DeserializeObject<TrustSelfAssessmentModel>(jsonString);

                if (saModel != null)
                {
                    var navItems = new List<KeyValuePair<string, string>>();
                    foreach (var category in Enum.GetValues(typeof(SadCategories)))
                    {
                        var linkText = GenerateLinkText(category.ToString());
                        navItems.Add(new KeyValuePair<string, string>(category.ToString(), linkText));
                    }

                    saModel.Navigation = navItems;
                }
            }  
            return saModel;
        }
        
        private static string GenerateLinkText(string originalText)
        {
            if (originalText.StartsWith("Ks2"))
            {
                return "KS2 score";
            }
            // boundary match on CAPs or 8
            var linkText = Regex.Replace(originalText, "(\\B[A-Z]|8)", " $1");

            return string.Concat(linkText.Substring(0, 1), linkText.Substring(1).ToLower());
        }
    }
}