using NUnit.Framework;
using SFB.Web.UI.Models;
using SFB.Web.UI.Services;
using System.Collections.Generic;
using System.Dynamic;
using SFB.Web.Common;

namespace SFB.Web.UI.UnitTests
{
    public class StatisticalCriteriaBuilderServiceUnitTests
    {
        [Test]
        public void BuildShouldApplySchoolPhaseToCriteria()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 100);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 100);
            financeDoc.SetPropertyValue("% of pupils with EAL", 100);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, true, true, true, true, 0);

            Assert.AreEqual(financeDoc.GetPropertyValue<string>("Overall Phase"), criteria.SchoolOverallPhase[0]);
        }

        [Test]
        public void BuildShouldApplyUrbanRuralToCriteria()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 100);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 100);
            financeDoc.SetPropertyValue("% of pupils with EAL", 100);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, true, true, true, true, 0);

            Assert.AreEqual(financeDoc.GetPropertyValue<string>("UrbanRuralInner"), criteria.UrbanRural[0]);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForNumberOfPupils()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100d);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 100);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 100);
            financeDoc.SetPropertyValue("% of pupils with EAL", 100);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, true, true, true, true, 5);

            Assert.AreEqual(financeDoc.GetPropertyValue<double>("No Pupils") * 0.85, criteria.MinNoPupil);
            Assert.AreEqual(financeDoc.GetPropertyValue<double>("No Pupils") * 1.15, criteria.MaxNoPupil);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForFreeSchoolMeals()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100d);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 10);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 10);
            financeDoc.SetPropertyValue("% of pupils with EAL", 10);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, true, true, true, true, 3);

            Assert.AreEqual(7, criteria.MinPerFSM);
            Assert.AreEqual(13, criteria.MaxPerFSM);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForSenRegister()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100d);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 10);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 10);
            financeDoc.SetPropertyValue("% of pupils with EAL", 10);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, true, true, true, true, 4);

            Assert.AreEqual(6, criteria.MinPerSEN);
            Assert.AreEqual(14, criteria.MaxPerSEN);
        }

        [Test]
        public void BuildShouldApplyPercentageMarginForEal()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100d);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 2);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 2);
            financeDoc.SetPropertyValue("% of pupils with EAL", 2);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, true, true, true, true, 5);

            Assert.AreEqual(0, criteria.MinPerEAL);
            Assert.AreEqual(7, criteria.MaxPerEAL);
        }

        [Test]
        public void BuildShouldApplyLACriteria()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100d);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 100);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 100);
            financeDoc.SetPropertyValue("% of pupils with EAL", 100);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, true, true, true, true, 10);

            Assert.AreEqual(financeDoc.GetPropertyValue<int>("LA"), criteria.LaCode);
        }

        [Test]
        public void BuildShouldNotApplyOptionalCriteriaIfNotIncluded()
        {
            dynamic model = new ExpandoObject();
            model.NumberOfPupils = 100;

            var financeDoc = new Microsoft.Azure.Documents.Document();
            financeDoc.SetPropertyValue("No Pupils", 100d);
            financeDoc.SetPropertyValue("Overall Phase", "Secondary");
            financeDoc.SetPropertyValue("UrbanRuralInner", "Urban and city");
            financeDoc.SetPropertyValue("% of pupils eligible for FSM", 100);
            financeDoc.SetPropertyValue("% of pupils with SEN Statement", 100);
            financeDoc.SetPropertyValue("% of pupils with EAL", 100);
            financeDoc.SetPropertyValue("LA", 831);

            var benchmarkSchool = new SchoolViewModel(model);
            benchmarkSchool.HistoricalSchoolDataModels = new List<SchoolDataModel>();
            benchmarkSchool.HistoricalSchoolDataModels.Add(new SchoolDataModel("123", "2014-2015", financeDoc, SchoolFinancialType.Maintained));

            var builder = new StatisticalCriteriaBuilderService();

            var criteria = builder.Build(benchmarkSchool, false, false, false, false, 10);

            Assert.AreEqual(null, criteria.MinPerFSM);
            Assert.AreEqual(null, criteria.MaxPerFSM);
            Assert.AreEqual(null, criteria.MinPerSEN);
            Assert.AreEqual(null, criteria.MaxPerSEN);
            Assert.AreEqual(null, criteria.MinPerEAL);
            Assert.AreEqual(null, criteria.MaxPerEAL);
            Assert.AreEqual(null, criteria.LaCode);
        }
    }
}
