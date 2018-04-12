using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services.DataAccess;

namespace SFB.Web.Domain.Services.Comparison
{
    public class ComparisonService : IComparisonService
    {
        private readonly IFinancialDataService _financialDataService;
        private readonly IBenchmarkCriteriaBuilderService _benchmarkCriteriaBuilderService;

        public ComparisonService(IFinancialDataService financialDataService, IBenchmarkCriteriaBuilderService benchmarkCriteriaBuilderService)
        {
            _financialDataService = financialDataService;
            _benchmarkCriteriaBuilderService = benchmarkCriteriaBuilderService;
        }

        public async Task<ComparisonResult> GenerateBenchmarkListWithAdvancedComparisonAsync(BenchmarkCriteria criteria, EstablishmentType estType, int basketSize = ComparisonListLimit.LIMIT)
        {
            var benchmarkSchools = await _financialDataService.SearchSchoolsByCriteriaAsync(criteria, estType);
            var limitedList = benchmarkSchools.Take(basketSize).ToList();
            return new ComparisonResult()
            {
                BenchmarkSchools = limitedList,
                BenchmarkCriteria = criteria
            };
        }

        public async Task<ComparisonResult> GenerateBenchmarkListWithSimpleComparisonAsync(
            BenchmarkCriteria benchmarkCriteria, EstablishmentType estType,
            int basketSize,
            SimpleCriteria simpleCriteria, SchoolDataModel defaultSchoolDataModel)
        {
            var benchmarkSchools = await _financialDataService.SearchSchoolsByCriteriaAsync(benchmarkCriteria, estType);

            if (benchmarkSchools.Count > basketSize) //Original query returns more than required. Cut from top by proximity.
            {
                benchmarkSchools = benchmarkSchools.OrderBy(b => Math.Abs(b.GetPropertyValue<int>("No Pupils") - defaultSchoolDataModel.PupilCount)).Take(basketSize).ToList();
                benchmarkCriteria.MinNoPupil = benchmarkSchools.Min(s => s.GetPropertyValue<int>("No Pupils"));
                benchmarkCriteria.MaxNoPupil = benchmarkSchools.Max(s => s.GetPropertyValue<int>("No Pupils")); //Update the criteria to reflect the max and min pupil count of the found schools
            }

            var tryCount = 0;
            while (benchmarkSchools.Count < basketSize) //Original query returns less than required
            {
                if (++tryCount > CriteriaSearchConfig.MAX_TRY_LIMIT) //Max query try reached. Return whatever is found.
                {
                    break;
                }

                benchmarkCriteria = _benchmarkCriteriaBuilderService.BuildFromSimpleComparisonCriteria(defaultSchoolDataModel, simpleCriteria, tryCount);

                benchmarkSchools = await _financialDataService.SearchSchoolsByCriteriaAsync(benchmarkCriteria, estType);

                if (benchmarkSchools.Count > basketSize) //Number jumping to more than ideal. Cut from top by proximity.
                {
                    benchmarkSchools = benchmarkSchools.OrderBy(b => Math.Abs(b.GetPropertyValue<int>("No Pupils") - defaultSchoolDataModel.PupilCount)).Take(basketSize).ToList();
                    benchmarkCriteria.MinNoPupil = benchmarkSchools.Min(s => s.GetPropertyValue<int>("No Pupils"));
                    benchmarkCriteria.MaxNoPupil = benchmarkSchools.Max(s => s.GetPropertyValue<int>("No Pupils")); //Update the criteria to reflect the max and min pupil count of the found schools
                    break;
                }
            }

            return new ComparisonResult()
            {
                BenchmarkSchools = benchmarkSchools,
                BenchmarkCriteria = benchmarkCriteria
            };
        }
    }
}