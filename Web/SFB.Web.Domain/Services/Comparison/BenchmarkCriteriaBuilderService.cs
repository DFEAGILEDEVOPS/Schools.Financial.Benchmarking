using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.Comparison
{
    public class BenchmarkCriteriaBuilderService : IBenchmarkCriteriaBuilderService
    {
        public BenchmarkCriteria BuildFromSimpleComparisonCriteria(SchoolDataModel benchmarkSchoolData, SimpleCriteria simpleCriteria, int percentageMargin = 0)
        {
            return BuildFromSimpleComparisonCriteria(benchmarkSchoolData, simpleCriteria.IncludeFsm.GetValueOrDefault(),
                simpleCriteria.IncludeSen.GetValueOrDefault(), simpleCriteria.IncludeEal.GetValueOrDefault(),
                simpleCriteria.IncludeLa.GetValueOrDefault(), percentageMargin);
        }

        public BenchmarkCriteria BuildFromSimpleComparisonCriteria(SchoolDataModel benchmarkSchoolData, bool includeFsm, bool includeSen, bool includeEal, bool includeLa, int percentageMargin = 0) 
        {
            var criteria = new BenchmarkCriteria();

            criteria.SchoolOverallPhase = new []{ benchmarkSchoolData.SchoolOverallPhase};
            criteria.UrbanRural = new []{ benchmarkSchoolData.UrbanRural};

            var minMarginFactor = 1 - ((percentageMargin + CriteriaSearchConfig.DEFAULT_MARGIN) / 100d);
            var maxMarginFactor = 1 + ((percentageMargin + CriteriaSearchConfig.DEFAULT_MARGIN) / 100d);
            
            criteria.MinNoPupil = benchmarkSchoolData.PupilCount * minMarginFactor;
            criteria.MaxNoPupil = benchmarkSchoolData.PupilCount * maxMarginFactor;

            if(includeFsm)
            {
                var fsm = double.Parse(benchmarkSchoolData.PercentageOfEligibleFreeSchoolMeals);
                criteria.MinPerFSM =  (fsm - percentageMargin) < 0 ? 0 : (fsm - percentageMargin);
                criteria.MaxPerFSM = fsm + percentageMargin;
            }

            if(includeSen)
            {
                var sen = double.Parse(benchmarkSchoolData.PercentageOfPupilsWithSen);
                criteria.MinPerSEN =  (sen - percentageMargin < 0) ? 0  : (sen - percentageMargin);
                criteria.MaxPerSEN = sen + percentageMargin;
            }

            if(includeEal)
            {
                var eal = double.Parse(benchmarkSchoolData.PercentageOfPupilsWithEal);
                criteria.MinPerEAL = (eal - percentageMargin) < 0 ? 0 : (eal - percentageMargin) ;
                criteria.MaxPerEAL = eal + percentageMargin;
            }

            if(includeLa)
            {
                criteria.LaCode = benchmarkSchoolData.LaNumber;
            }

            return criteria;
        }
    }
}