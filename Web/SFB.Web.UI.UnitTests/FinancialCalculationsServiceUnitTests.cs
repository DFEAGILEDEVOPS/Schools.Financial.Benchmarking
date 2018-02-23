using NUnit.Framework;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using SFB.Web.UI.Services;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using SFB.Web.Common;
using SFB.Web.Domain.Models;

namespace SFB.Web.UI.UnitTests
{
    public class FinancialCalculationsServiceUnitTests
    {
        [Test]
        public void PopulateHistoricalChartsWithFinancialDataCalculatesAbsoluteAmounts()
        {
            List<ChartViewModel> historicalCharts = new List<ChartViewModel>() {
                new ChartViewModel()
                {
                    Name = "Special Facilities",
                    FieldName =  "Special facilities" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.SpecialFacilities,
                },
                new ChartViewModel()
                {
                    Name = "Education Support Staff",
                    FieldName =   "Education support staff" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.Staff,
                } };

            var doc = new Document();
            doc.SetPropertyValue("Special facilities", "1000");
            doc.SetPropertyValue("Education support staff", "2000");
            doc.SetPropertyValue("No Pupils", "100");
            doc.SetPropertyValue("No Teachers", "10");            

            var dataModels = new List<SchoolDataModel>()
            {
                new SchoolDataModel("123", "2014 / 2015", doc, SchoolFinancialType.Maintained)
            };

            var service = new FinancialCalculationsService();

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.AbsoluteMoney, SchoolFinancialType.Academies);

            Assert.AreEqual(1000, historicalCharts[0].LastYearBalance);
            Assert.AreEqual(2000, historicalCharts[1].LastYearBalance);
        }

        [Test]
        public void PopulateHistoricalChartsWithFinancialDataCalculatesAmountsPerPupil()
        {
            List<ChartViewModel> historicalCharts = new List<ChartViewModel>() {
                new ChartViewModel()
                {
                    Name = "Special Facilities",
                    FieldName =  "Special facilities" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.SpecialFacilities,
                },
                new ChartViewModel()
                {
                    Name = "Education Support Staff",
                    FieldName =   "Education support staff" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.Staff,
                } };

            var doc = new Document();
            doc.SetPropertyValue("Special facilities", "1000");
            doc.SetPropertyValue("Education support staff", "2000");
            doc.SetPropertyValue("No Pupils", "100");
            doc.SetPropertyValue("No Teachers", "10");

            var dataModels = new List<SchoolDataModel>()
            {
                new SchoolDataModel("123", "2014 / 2015", doc, SchoolFinancialType.Maintained)
            };

            var service = new FinancialCalculationsService();

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PerPupil, SchoolFinancialType.Academies);

            Assert.AreEqual(10, historicalCharts[0].LastYearBalance);
            Assert.AreEqual(20, historicalCharts[1].LastYearBalance);
        }

        [Test]
        public void PopulateHistoricalChartsWithFinancialDataCalculatesAmountsPerTeacher()
        {
            List<ChartViewModel> historicalCharts = new List<ChartViewModel>() {
                new ChartViewModel()
                {
                    Name = "Special Facilities",
                    FieldName =  "Special facilities" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.SpecialFacilities,
                },
                new ChartViewModel()
                {
                    Name = "Education Support Staff",
                    FieldName =   "Education support staff" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.Staff,
                } };

            var doc = new Document();
            doc.SetPropertyValue("Special facilities", "1000");
            doc.SetPropertyValue("Education support staff", "2000");
            doc.SetPropertyValue("No Pupils", "100");
            doc.SetPropertyValue("No Teachers", "10");

            var dataModels = new List<SchoolDataModel>()
            {
                new SchoolDataModel("123", "2014 / 2015", doc, SchoolFinancialType.Maintained)
            };

            var service = new FinancialCalculationsService();

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PerTeacher, SchoolFinancialType.Academies);

            Assert.AreEqual(100, historicalCharts[0].LastYearBalance);
            Assert.AreEqual(200, historicalCharts[1].LastYearBalance);
        }


        [Test]
        public void PopulateHistoricalChartsWithFinancialDataCalculatesAmountsByPercentage()
        {
            List<ChartViewModel> historicalCharts = new List<ChartViewModel>() {
                new ChartViewModel()
                {
                    Name = "Special Facilities",
                    FieldName =  "Special facilities" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.SpecialFacilities,
                },
                new ChartViewModel()
                {
                    Name = "Education Support Staff",
                    FieldName =   "Education support staff" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.Staff,
                } };

            var doc = new Document();
            doc.SetPropertyValue("Special facilities", "1000");
            doc.SetPropertyValue("Education support staff", "2000");
            doc.SetPropertyValue("No Pupils", "100");
            doc.SetPropertyValue("No Teachers", "10");
            doc.SetPropertyValue("Total Expenditure", "5000");

            var dataModels = new List<SchoolDataModel>()
            {
                new SchoolDataModel("123", "2014 / 2015", doc, SchoolFinancialType.Maintained)
            };

            var service = new FinancialCalculationsService();

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PercentageOfTotal, SchoolFinancialType.Academies);

            Assert.AreEqual(20, historicalCharts[0].LastYearBalance);
            Assert.AreEqual(40, historicalCharts[1].LastYearBalance);
        }

        [Test]
        public void PopulateHistoricalChartsWithFinancialDataGeneratesJSONWithCorrectYearLabels()
        {
            List<ChartViewModel> historicalCharts = new List<ChartViewModel>() {
                new ChartViewModel()
                {
                    Name = "Special Facilities",
                    FieldName =  "Special facilities" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.SpecialFacilities,
                },
                new ChartViewModel()
                {
                    Name = "Education Support Staff",
                    FieldName =   "Education support staff" ,
                    RevenueGroup = Helpers.Enums.RevenueGroupType.Expenditure,
                    ChartGroup = Helpers.Enums.ChartGroupType.Staff,
                } };

            var doc = new Document();
            doc.SetPropertyValue("Special facilities", "1000");
            doc.SetPropertyValue("Education support staff", "2000");
            doc.SetPropertyValue("No Pupils", "100");
            doc.SetPropertyValue("No Teachers", "10");            

            var dataModels = new List<SchoolDataModel>()
            {
                new SchoolDataModel("123", "2014 / 2015", doc, SchoolFinancialType.Maintained)
            };

            var service = new FinancialCalculationsService();

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.AbsoluteMoney, SchoolFinancialType.Maintained);
            
            var historicalChartData = JsonConvert.DeserializeObject<List<HistoricalChartData>>(historicalCharts[0].DataJson);
            Assert.AreEqual("2014-15", historicalChartData[0].Year);
        }
    }
}
