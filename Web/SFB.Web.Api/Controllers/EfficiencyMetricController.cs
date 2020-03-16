using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFB.Web.ApplicationCore.Helpers.Enums;
using SFB.Web.ApplicationCore.Models;
using SFB.Web.ApplicationCore.Services.DataAccess;

namespace SFB.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EfficiencyMetricController : ControllerBase
    {
        private readonly IEfficiencyMetricDataService _efficiencyMetricDataService;
        private readonly IContextDataService _contextDataService;
        private readonly IFinancialDataService _financialDataService;

        public EfficiencyMetricController(IContextDataService contextDataService, IFinancialDataService financialDataService, IEfficiencyMetricDataService efficiencyMetricDataService)
        {
            _contextDataService = contextDataService;
            _financialDataService = financialDataService;
            _efficiencyMetricDataService = efficiencyMetricDataService;

        }

        // GET api/efficiencymetric/5
        [HttpGet("{urn}")]
        public async Task<ActionResult<EfficiencyMetricModel>> GetAsync(int urn)
        {
            var model = new EfficiencyMetricModel();

            model.ContextData = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);

            if (model.ContextData == null)
            {
                return NoContent();
            }
            else
            {
                var financeType = (EstablishmentType)Enum.Parse(typeof(EstablishmentType), model.ContextData.FinanceType);
                model.FinancialData = await _financialDataService.GetSchoolFinancialDataObjectAsync(urn, financeType);
                model.EfficiencyMetricData = await _efficiencyMetricDataService.GetSchoolDataObjectByUrnAsync(urn);

                return model;
            }
        }
    }
}