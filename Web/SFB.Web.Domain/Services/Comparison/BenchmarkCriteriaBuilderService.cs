using SFB.Web.Common;
using SFB.Web.Domain.Helpers.Constants;
using SFB.Web.Domain.Models;

namespace SFB.Web.Domain.Services.Comparison
{
    public class BenchmarkCriteriaBuilderService : IBenchmarkCriteriaBuilderService
    {
        public BenchmarkCriteria BuildFromBicComparisonCriteria(FinancialDataModel benchmarkSchoolData, BestInClassCriteria bicCriteria, int percentageMargin = 0)
        {
            return new BenchmarkCriteria()
            {
                SchoolOverallPhase = new[] { bicCriteria.OverallPhase },
                UrbanRural = new[] { bicCriteria.UrbanRural },
                MinNoPupil = (bicCriteria.NoPupilsMin - (benchmarkSchoolData.PupilCount * percentageMargin / 100)) < 0 ? 0 : (bicCriteria.NoPupilsMin - (benchmarkSchoolData.PupilCount * percentageMargin / 100)),
                MaxNoPupil = (bicCriteria.NoPupilsMax + (benchmarkSchoolData.PupilCount * percentageMargin / 100)),
                MinPerFSM = WithinPercentLimits(bicCriteria.PercentageFSMMin - (benchmarkSchoolData.PercentageOfEligibleFreeSchoolMeals * percentageMargin / 100)),
                MaxPerFSM = WithinPercentLimits(bicCriteria.PercentageFSMMax + (benchmarkSchoolData.PercentageOfEligibleFreeSchoolMeals * percentageMargin / 100)),
                MinPerSEN = WithinPercentLimits(bicCriteria.PercentageSENMin - (benchmarkSchoolData.PercentageOfPupilsWithSen * percentageMargin / 100)),
                MaxPerSEN = WithinPercentLimits(bicCriteria.PercentageSENMax + (benchmarkSchoolData.PercentageOfPupilsWithSen * percentageMargin / 100)),
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
                criteria.MinPerFSM =  WithinPercentLimits(fsm - percentageMargin);
                criteria.MaxPerFSM = WithinPercentLimits(fsm + percentageMargin);
            }

            if(includeSen)
            {
                var sen = benchmarkSchoolData.PercentageOfPupilsWithSen;
                criteria.MinPerSEN =  WithinPercentLimits(sen - percentageMargin);
                criteria.MaxPerSEN = WithinPercentLimits(sen + percentageMargin);
            }

            if(includeEal)
            {
                var eal = benchmarkSchoolData.PercentageOfPupilsWithEal;
                criteria.MinPerEAL = WithinPercentLimits(eal - percentageMargin);
                criteria.MaxPerEAL = WithinPercentLimits(eal + percentageMargin);
            }

            if(includeLa)
            {
                criteria.LocalAuthorityCode = benchmarkSchoolData.LaNumber;
            }

            return criteria;
        }

        private decimal? WithinPercentLimits(decimal? percent)
        {
            if(percent > 100)
            {
                return 100;
            }
            if (percent < 0)
            {
                return 0;
            }
            else return percent;
        }
    }

}