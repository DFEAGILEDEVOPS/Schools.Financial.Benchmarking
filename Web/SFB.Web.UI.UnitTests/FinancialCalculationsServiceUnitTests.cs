using NUnit.Framework;
using SFB.Web.UI.Models;
using System.Collections.Generic;
using SFB.Web.UI.Services;
using Moq;
using Newtonsoft.Json;
using SFB.Web.Common;
using SFB.Web.Domain.Models;
using SFB.Web.Domain.Services;
using SFB.Web.Common.DataObjects;

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

            var financialObj = new SchoolTrustFinancialDataObject();
            financialObj.Specialfacilities = 1000;
            financialObj.EducationSupportStaff = 2000;
            financialObj.NoPupils = 100;
            financialObj.NoTeachers = 10;            
            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", financialObj, EstablishmentType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.AbsoluteMoney, EstablishmentType.Academies);

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

            var financialObj = new SchoolTrustFinancialDataObject();
            financialObj.Specialfacilities = 1000;
            financialObj.EducationSupportStaff = 2000;
            financialObj.NoPupils = 100;
            financialObj.NoTeachers = 10;

            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", financialObj, EstablishmentType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PerPupil, EstablishmentType.Academies);

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

            var financialObj = new SchoolTrustFinancialDataObject();
            financialObj.Specialfacilities = 1000;
            financialObj.EducationSupportStaff = 2000;
            financialObj.NoPupils = 100;
            financialObj.NoTeachers = 10;

            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", financialObj, EstablishmentType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PerTeacher, EstablishmentType.Academies);

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

            var financialObj = new SchoolTrustFinancialDataObject();
            financialObj.Specialfacilities = 1000;
            financialObj.EducationSupportStaff = 2000;
            financialObj.NoPupils = 100;
            financialObj.NoTeachers = 10;
            financialObj.TotalExpenditure = 5000;
            
            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", financialObj, EstablishmentType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.PercentageOfTotal, EstablishmentType.Academies);

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

            var financialObj = new SchoolTrustFinancialDataObject();
            financialObj.Specialfacilities = 1000;
            financialObj.EducationSupportStaff = 2000;
            financialObj.NoPupils = 100;
            financialObj.NoTeachers = 10;

            var dataModels = new List<FinancialDataModel>()
            {
                new FinancialDataModel("123", "2014 / 2015", financialObj, EstablishmentType.Maintained)
            };

            var mockLaService = new Mock<ILocalAuthoritiesService>();
            mockLaService.Setup(m => m.GetLocalAuthorities()).Returns(() => "[{\"id\": \"0\",\"LANAME\": \"Hartlepool\",\"REGION\": \"1\",\"REGIONNAME\": \"North East A\"}]");

            var service = new FinancialCalculationsService(mockLaService.Object);

            service.PopulateHistoricalChartsWithSchoolData(historicalCharts, dataModels, "2014 / 2015", Helpers.Enums.RevenueGroupType.Expenditure, Helpers.Enums.UnitType.AbsoluteMoney, EstablishmentType.Maintained);
            
            var historicalChartData = JsonConvert.DeserializeObject<List<HistoricalChartData>>(historicalCharts[0].DataJson);
            Assert.AreEqual("2014-15", historicalChartData[0].Year);
        }
    }
}
