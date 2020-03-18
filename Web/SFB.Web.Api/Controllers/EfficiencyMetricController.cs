using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger _logger;

        public EfficiencyMetricController(IContextDataService contextDataService, 
            IFinancialDataService financialDataService, 
            IEfficiencyMetricDataService efficiencyMetricDataService,
            ILogger<EfficiencyMetricController> logger)
        {
            _contextDataService = contextDataService;
            _financialDataService = financialDataService;
            _efficiencyMetricDataService = efficiencyMetricDataService;

            _logger = logger;

        }

        // GET api/efficiencymetric/5
        [HttpGet("{urn}")]
        public async Task<ActionResult<EfficiencyMetricModel>> GetAsync(int urn)
        {
            var model = new EfficiencyMetricModel();

            try
            {
                model.ContextData = await _contextDataService.GetSchoolDataObjectByUrnAsync(urn);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            if (model.ContextData == null)
            {
                _logger.LogWarning("No school found with URN: {urn}", urn);
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