using SFB.Web.ApplicationCore.Helpers.Constants;
using SFB.Web.ApplicationCore.Models;

namespace SFB.Web.ApplicationCore.Services.Comparison
{
    public class BenchmarkCriteriaBuilderService : IBenchmarkCriteriaBuilderService
    {
        public BenchmarkCriteria BuildFromBicComparisonCriteria(FinancialDataModel benchmarkSchoolData, BestInClassCriteria bicCriteria, int percentageMargin = 0)
        {
            var bmCriteria = new BenchmarkCriteria()
            {
                SchoolOverallPhase = new[] { bicCriteria.OverallPhase },
                SchoolPhase = CriteriaSearchConfig.BIC_ALLOWED_PHASES,
                MinKs2Progress = bicCriteria.Ks2ProgressScoreMin,
                MaxKs2Progress = bicCriteria.Ks2ProgressScoreMax,
                MinP8Mea = bicCriteria.Ks4ProgressScoreMin,
                MaxP8Mea = bicCriteria.Ks4ProgressScoreMax,
                MinRRToIncome = bicCriteria.RRPerIncomeMin,
                MinNoPupil = WithinPositiveLimits(bicCriteria.NoPupilsMin - (bicCriteria.NoPupilsMin * percentageMargin / 100)),
                MaxNoPupil = bicCriteria.NoPupilsMax + (bicCriteria.NoPupilsMax * percentageMargin / 100),
                MinPerPupilExp = bicCriteria.PerPupilExpMin,
                MaxPerPupilExp = bicCriteria.PerPupilExpMax + (bicCriteria.PerPupilExpMax * percentageMargin / 100),
                MinPerFSM = WithinPercentLimits(bicCriteria.PercentageFSMMin - (bicCriteria.PercentageFSMMin * percentageMargin / 100)),
                MaxPerFSM = WithinPercentLimits(bicCriteria.PercentageFSMMax + (bicCriteria.PercentageFSMMax * percentageMargin / 100)),
                LondonWeighting = bicCriteria.LondonWeighting
            };

            if(bicCriteria.OverallPhase == "All-through")
            {
                bmCriteria.SchoolPhase = new[] { "All-through" };
            }

            if (bicCriteria.SENEnabled)
            {
                bmCriteria.MinPerSEN = WithinPercentLimits(bicCriteria.PercentageSENMin - (bicCriteria.PercentageSENMin * percentageMargin / 100));
                bmCriteria.MaxPerSEN = WithinPercentLimits(bicCriteria.PercentageSENMax + (bicCriteria.PercentageSENMax * percentageMargin / 100));
            }

            if(bicCriteria.UREnabled)
            {
                bmCriteria.UrbanRural = new[] { bicCriteria.UrbanRural };
            }

            return bmCriteria;
        }

        public BenchmarkCriteria BuildFromOneClickComparisonCriteria(FinancialDataModel benchmarkSchoolData, int percentageMargin = 0)
        {
            var criteria = new BenchmarkCriteria();

            criteria.SchoolOverallPhase = new[] { benchmarkSchoolData.SchoolOverallPhase };
            criteria.UrbanRural = new[] { benchmarkSchoolData.UrbanRural };

            var minPcMarginFactor = 1 - ((percentageMargin + CriteriaSearchConfig.PC_DEFAULT_MARGIN) / 100m);
            var maxPcMarginFactor = 1 + ((percentageMargin + CriteriaSearchConfig.PC_DEFAULT_MARGIN) / 100m);

            criteria.MinNoPupil = WithinPositiveLimits(benchmarkSchoolData.PupilCount - CriteriaSearchConfig.QC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * minPcMarginFactor;
            criteria.MaxNoPupil = (benchmarkSchoolData.PupilCount + CriteriaSearchConfig.QC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * maxPcMarginFactor;

            var fsm = benchmarkSchoolData.PercentageOfEligibleFreeSchoolMeals;
            criteria.MinPerFSM = WithinPercentLimits(fsm - percentageMargin);
            criteria.MaxPerFSM = WithinPercentLimits(fsm + percentageMargin);

            var sen = benchmarkSchoolData.PercentageOfPupilsWithSen;
            criteria.MinPerSEN = WithinPercentLimits(sen - percentageMargin);
            criteria.MaxPerSEN = WithinPercentLimits(sen + percentageMargin);

            var eal = benchmarkSchoolData.PercentageOfPupilsWithEal;
            criteria.MinPerEAL = WithinPercentLimits(eal - percentageMargin);
            criteria.MaxPerEAL = WithinPercentLimits(eal + percentageMargin);

            var ppGrantFunding = benchmarkSchoolData.PerPupilGrantFunding;
            var minGfMarginFactor = 1 - ((percentageMargin + CriteriaSearchConfig.GF_DEFAULT_MARGIN) / 100m);
            var maxGfMarginFactor = 1 + ((percentageMargin + CriteriaSearchConfig.GF_DEFAULT_MARGIN) / 100m);
            criteria.MinPerPupilGrantFunding = ppGrantFunding * minGfMarginFactor;
            criteria.MaxPerPupilGrantFunding = ppGrantFunding * maxGfMarginFactor;

            criteria.LondonWeighting = benchmarkSchoolData.LondonWeighting == "Neither" ? new[] { "Neither" } : new[] { "Inner", "Outer" };

            criteria.PeriodCoveredByReturn = 12;

            return criteria;
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

            var minMarginFactor = 1 - ((percentageMargin + CriteriaSearchConfig.PC_DEFAULT_MARGIN) / 100m);
            var maxMarginFactor = 1 + ((percentageMargin + CriteriaSearchConfig.PC_DEFAULT_MARGIN) / 100m);
            
            criteria.MinNoPupil = WithinPositiveLimits(benchmarkSchoolData.PupilCount - CriteriaSearchConfig.QC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * minMarginFactor;
            criteria.MaxNoPupil = (benchmarkSchoolData.PupilCount + CriteriaSearchConfig.QC_DEFAULT_CONSTANT_PUPIL_COUNT_TOPUP) * maxMarginFactor;

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

            criteria.PeriodCoveredByReturn = 12;

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

        private decimal? WithinPositiveLimits(decimal? value)
        {
            if (value < 0)
            {
                return 0;
            }
            else return value;
        }
    }

}