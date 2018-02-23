using SFB.Web.Domain.Models;
using SFB.Web.UI.Helpers.Constants;
using SFB.Web.UI.Models;
using System.Linq;
using SFB.Web.Common;

namespace SFB.Web.UI.Services
{
    public class StatisticalCriteriaBuilderService : IStatisticalCriteriaBuilderService
    {

        public BenchmarkCriteria Build(SchoolViewModel benchmarkSchool, bool includeFsm, bool includeSen, bool includeEal, bool includeLa, int percentageMargin)
        {
            var criteria = new BenchmarkCriteria();

            var latestSchoolData = benchmarkSchool.HistoricalSchoolDataModels.Last();

            criteria.SchoolOverallPhase = new []{ latestSchoolData.SchoolOverallPhase};
            criteria.UrbanRural = new []{ latestSchoolData.UrbanRural};

            var minMarginFactor = 1 - ((percentageMargin + CriteriaSearchConfig.DEFAULT_MARGIN) / 100d);
            var maxMarginFactor = 1 + ((percentageMargin + CriteriaSearchConfig.DEFAULT_MARGIN) / 100d);
            
            criteria.MinNoPupil = latestSchoolData.PupilCount * minMarginFactor;
            criteria.MaxNoPupil = latestSchoolData.PupilCount * maxMarginFactor;

            if(includeFsm)
            {
                var fsm = double.Parse(latestSchoolData.PercentageOfEligibleFreeSchoolMeals);
                criteria.MinPerFSM =  (fsm - percentageMargin) < 0 ? 0 : (fsm - percentageMargin);
                criteria.MaxPerFSM = fsm + percentageMargin;
            }

            if(includeSen)
            {
                var sen = double.Parse(latestSchoolData.PercentageOfPupilsWithSen);
                criteria.MinPerSEN =  (sen - percentageMargin < 0) ? 0  : (sen - percentageMargin);
                criteria.MaxPerSEN = sen + percentageMargin;
            }

            if(includeEal)
            {
                var eal = double.Parse(latestSchoolData.PercentageOfPupilsWithEal);
                criteria.MinPerEAL = (eal - percentageMargin) < 0 ? 0 : (eal - percentageMargin) ;
                criteria.MaxPerEAL = eal + percentageMargin;
            }

            if(includeLa)
            {
                criteria.LaCode = latestSchoolData.LaNumber;
            }

            return criteria;
        }
    }
}