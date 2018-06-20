using NUnit.Framework;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;
using SFB.Web.Domain.Services.Comparison;
using SFB.Web.Common.DataObjects;

namespace SFB.Web.UI.UnitTests
{
    public class StatisticalCriteriaBuilderServiceUnitTests
    {
        [Test]
        public void BuildShouldApplySchoolPhaseToCriteria()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 100;
            financeObj.PercentagePupilsWSEN = 100;
            financeObj.PercentagePupilsWEAL = 100;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), true, true, true, true, 0);

            Assert.AreEqual(financeObj.OverallPhase, criteria.SchoolOverallPhase[0]);
        }

        [Test]
        public void BuildShouldApplyUrbanRuralToCriteria()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 100;
            financeObj.PercentagePupilsWSEN = 100;
            financeObj.PercentagePupilsWEAL = 100;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), true, true, true, true, 0);

            Assert.AreEqual(financeObj.UrbanRural, criteria.UrbanRural[0]);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForNumberOfPupils()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 100;
            financeObj.PercentagePupilsWSEN = 100;
            financeObj.PercentagePupilsWEAL = 100;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), true, true, true, true, 5);

            Assert.AreEqual(financeObj.NoPupils * 0.85, criteria.MinNoPupil);
            Assert.AreEqual(financeObj.NoPupils * 1.15, criteria.MaxNoPupil);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForFreeSchoolMeals()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 10;
            financeObj.PercentagePupilsWSEN = 10;
            financeObj.PercentagePupilsWEAL = 10;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), true, true, true, true, 3);

            Assert.AreEqual(7, criteria.MinPerFSM);
            Assert.AreEqual(13, criteria.MaxPerFSM);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForSenRegister()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 10;
            financeObj.PercentagePupilsWSEN = 10;
            financeObj.PercentagePupilsWEAL = 10;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), true, true, true, true, 4);

            Assert.AreEqual(6, criteria.MinPerSEN);
            Assert.AreEqual(14, criteria.MaxPerSEN);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForEal()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 2;
            financeObj.PercentagePupilsWSEN = 2;
            financeObj.PercentagePupilsWEAL = 2;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), true, true, true, true, 5);

            Assert.AreEqual(0, criteria.MinPerEAL);
            Assert.AreEqual(7, criteria.MaxPerEAL);
        }

        [Test]
        public void BuildShouldApplyLACriteria()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 100;
            financeObj.PercentagePupilsWSEN = 100;
            financeObj.PercentagePupilsWEAL = 100;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), true, true, true, true, 10);

            Assert.AreEqual(financeObj.LA, criteria.LocalAuthorityCode);
        }

        [Test]
        public void BuildShouldNotApplyOptionalCriteriaIfNotIncluded()
        {
            dynamic model = new EdubaseDataObject();
            model.NumberOfPupils = 100;

            var financeObj = new SchoolTrustFinancialDataObject();
            financeObj.NoPupils = 100;
            financeObj.OverallPhase = "Secondary";
            financeObj.UrbanRural = "Urban and city";
            financeObj.PercentageFSM = 2;
            financeObj.PercentagePupilsWSEN = 2;
            financeObj.PercentagePupilsWEAL = 2;
            financeObj.LA = 831;

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalFinancialDataModels = new List<FinancialDataModel>();
            benchmarkSchool.HistoricalFinancialDataModels.Add(new FinancialDataModel("123", "2014-2015", financeObj, EstablishmentType.Maintained));

            var builder = new BenchmarkCriteriaBuilderService();

            var criteria = builder.BuildFromSimpleComparisonCriteria(benchmarkSchool.HistoricalFinancialDataModels.Last(), false, false, false, false, 10);

            Assert.AreEqual(null, criteria.MinPerFSM);
            Assert.AreEqual(null, criteria.MaxPerFSM);
            Assert.AreEqual(null, criteria.MinPerSEN);
            Assert.AreEqual(null, criteria.MaxPerSEN);
            Assert.AreEqual(null, criteria.MinPerEAL);
            Assert.AreEqual(null, criteria.MaxPerEAL);
            Assert.AreEqual(null, criteria.LocalAuthorityCode);
        }
    }
}
