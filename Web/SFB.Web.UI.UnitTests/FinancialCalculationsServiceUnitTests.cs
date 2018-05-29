using NUnit.Framework;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using SFB.Web.UI.Services;
using Microsoft.Azure.Documents;
using Moq;
using Newtonsoft.Json;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;

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
            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", doc, EstabType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.AbsoluteMoney, EstabType.Academies);

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

            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", doc, EstabType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PerPupil, EstabType.Academies);

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

            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", doc, EstabType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PerTeacher, EstabType.Academies);

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

            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", doc, EstabType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PercentageOfTotal, EstabType.Academies);

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

            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", doc, EstabType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.AbsoluteMoney, EstabType.Maintained);
            
            var historicalChartData = JsonConvert.DeserializeObject<List<HistoricalChartData>>(historicalCharts[0].DataJson);
            Assert.AreEqual("2014-15", historicalChartData[0].Year);
        }
    }
}
