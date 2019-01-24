using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.Comparison
{
    public class BenchmarkCriteriaBuilderService : IBenchmarkCriteriaBuilderService
    {
        public BenchmarkCriteria BuildFromBicComparisonCriteria(BestInClassCriteria bicCriteria)
        {
            return new BenchmarkCriteria()
            {
                SchoolOverallPhase = new[] { bicCriteria.OverallPhase },
                UrbanRural = new[] { bicCriteria.UrbanRural },
                MinNoPupil = bicCriteria.NoPupilsMin,
                MaxNoPupil = bicCriteria.NoPupilsMax,
                MinPerFSM = bicCriteria.PercentageFSMMin,
                MaxPerFSM = bicCriteria.PercentageFSMMax,
                MinPerSEN = bicCriteria.PercentageSENMin,
                MaxPerSEN = bicCriteria.PercentageSENMax,
                MinKs2Progress = bicCriteria.Ks2ProgressScoreMin,
                MaxKs2Progress = bicCriteria.Ks2ProgressScoreMax,
                MinP8Mea = bicCriteria.Ks4ProgressScoreMin,
                MaxP8Mea = bicCriteria.Ks4ProgressScoreMax
            };
        }

        public BenchmarkCriteria BuildFromSimpleComparisonCriteria(FinancialDataModel benchmarkSchoolData, SimpleCriteria simpleCriteria, int percentageMargin = 0)
        {
            return BuildFromSimpleComparisonCriteria(benchmarkSchoolData, simpleCriteria.IncludeFsm.GetValueOrDefault(),
                simpleCriteria.IncludeSen.GetValueOrDefault(), simpleCriteria.IncludeEal.GetValueOrDefault(),
                simpleCriteria.IncludeLa.GetValueOrDefault(), percentageMargin);
        }

        public BenchmarkCriteria BuildFromSimpleComparisonCriteria(FinancialDataModel benchmarkSchoolData, bool includeFsm, bool includeSen, bool includeEal, bool includeLa, int percentageMargin = 0) 
        {
            var criteria = new BenchmarkCriteria();

            criteria.SchoolOverallPhase = new []{ benchmarkSchoolData.SchoolOverallPhase};
            criteria.UrbanRural = new []{ benchmarkSchoolData.UrbanRural};

            var minMarginFactor = 1 - ((percentageMargin + CriteriaSearchConfig.DEFAULT_MARGIN) / 100m);
            var maxMarginFactor = 1 + ((percentageMargin + CriteriaSearchConfig.DEFAULT_MARGIN) / 100m);
            
            criteria.MinNoPupil = benchmarkSchoolData.PupilCount * minMarginFactor;
            criteria.MaxNoPupil = benchmarkSchoolData.PupilCount * maxMarginFactor;

            if(includeFsm)
            {
                var fsm = benchmarkSchoolData.PercentageOfEligibleFreeSchoolMeals;
                criteria.MinPerFSM =  (fsm - percentageMargin) < 0 ? 0 : (fsm - percentageMargin);
                criteria.MaxPerFSM = (fsm + percentageMargin) > 100 ? 100 : (fsm + percentageMargin);
            }

            if(includeSen)
            {
                var sen = benchmarkSchoolData.PercentageOfPupilsWithSen;
                criteria.MinPerSEN =  (sen - percentageMargin) < 0 ? 0  : (sen - percentageMargin);
                criteria.MaxPerSEN = (sen + percentageMargin) > 100 ? 100 : (sen + percentageMargin);
            }

            if(includeEal)
            {
                var eal = benchmarkSchoolData.PercentageOfPupilsWithEal;
                criteria.MinPerEAL = (eal - percentageMargin) < 0 ? 0 : (eal - percentageMargin) ;
                criteria.MaxPerEAL = (eal + percentageMargin) > 100 ? 100 : (eal + percentageMargin);
            }

            if(includeLa)
            {
                criteria.LocalAuthorityCode = benchmarkSchoolData.LaNumber;
            }

            return criteria;
        }
    }
}