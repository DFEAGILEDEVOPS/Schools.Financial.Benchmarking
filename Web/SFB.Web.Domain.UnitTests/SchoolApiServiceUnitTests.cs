//using Moq;
//using NUnit.Framework;
//using SFB.Web.Domain.Services;
//using System;
//using System.Collections.Generic;
//using SFB.Web.Domain.ApiWrappers;

//namespace SFB.Web.Domain.UnitTests
//{
//    public class SchoolApiServiceUnitTests
//    {
//        [SetUp]
//        public void Setup()
//        {}

//        //[Test]
//        //public void SchoolApiServiceShouldSearchSchoolByName()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" } );

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string,string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    service.SearchSchoolByName("Mock School", 0 , 50, string.Empty, null);

//        //    var parameters = new Dictionary<string, string>();
//        //    parameters.Add("name", "Mock School");
//        //    parameters.Add("skip", "0");
//        //    parameters.Add("take", "50");
//        //    parameters.Add("orderby", "");
//        //    parameters.Add("filter", "");

//        //    mockApiRequest.Verify(req => req.Get("school", new List<string> { "search" }, parameters), Times.Once());
//        //}


//        //[Test]
//        //public void SchoolApiServiceShouldFindSchoolsByName()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    var result = service.SearchSchoolByName("Mock School", 0, 50, string.Empty, null);

//        //    Assert.AreEqual("Test School", result.name);
//        //}

//        //[Test]
//        //public void SchoolApiServiceShouldRaiseExceptionIfSearchByNameApiFails()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.InternalServerError, null);

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);
            
//        //    Assert.Throws<ApplicationException>(() => service.SearchSchoolByName("Mock School", 0, 50, string.Empty, null));
//        //}

//        //[Test]
//        //public void SchoolApiServiceShouldGetSchoolByUrn()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    service.GetSchoolByUrn("123");

//        //    var parameters = new Dictionary<string, string>();

//        //    mockApiRequest.Verify(req => req.Get("school", new List<string> { "123" }, parameters), Times.Once());
//        //}

//        //[Test]
//        //public void SchoolApiServiceShouldFindSchoolByUrn()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    var result = service.GetSchoolByUrn("123");

//        //    Assert.AreEqual("Test School", result.name);
//        //}

//        //[Test]
//        //public void SchoolApiServiceShouldGetSchoolsByMultipleUrns()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { Count = 3 });

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    service.GetMultipleSchoolsByUrns(new List<string> { "123", "456", "789" });

//        //    var parameters = new Dictionary<string, string>();

//        //    mockApiRequest.Verify(req => req.Get("school", new List<string> { "123", "456", "789" }), Times.Once());
//        //}

//        //[Test]
//        //public void SchoolApiServiceShouldFindSchoolsByMultipleUrns()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { Count = 3 });

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    var result = service.GetMultipleSchoolsByUrns(new List<string> { "123", "456", "789" });

//        //    Assert.AreEqual(3, result.Count);
//        //}

//        //[Test]
//        //public void SchoolApiServiceShouldRaiseExceptionIfGetByUrnApiFails()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.InternalServerError, null);

//        //    mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    Assert.Throws<ApplicationException>(() => service.GetSchoolByUrn("123"));
//        //}

//        //[Test]
//        //public void SchoolApiServiceShouldSuggestSchoolByName()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//        //    mockApiRequest.Setup(a => a.Get("suggest", new List<string> { "school" }, It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    service.SuggestSchoolByName("Test");

//        //    var parameters = new Dictionary<string, string>();
//        //    parameters.Add("name", "Test");            

//        //    mockApiRequest.Verify(req => req.Get("suggest", new List<string> { "school" }, parameters), Times.Once());
//        //}


//        //[Test]
//        //public void SchoolApiServiceShouldSuggestSchoolsByName()
//        //{
//        //    var mockApiRequest = new Mock<IApiRequest>();
//        //    var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//        //    mockApiRequest.Setup(a => a.Get("suggest", new List<string> { "school" }, It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//        //    var service = new SchoolApiService(mockApiRequest.Object);

//        //    var result = service.SuggestSchoolByName("Test");

//        //    Assert.AreEqual("Test School", result.name);
//        //}

//        [Test]
//        public void SchoolApiServiceShouldSearchSchoolsBySponsorCode()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            service.GetSponsorByCode(1234);

//            var parameters = new Dictionary<string, string>();
//            parameters.Add("academysponsorcode", "1234");

//            mockApiRequest.Verify(req => req.Get("school", new List<string> { "search" }, parameters), Times.Once());
//        }


//        [Test]
//        public void SchoolApiServiceShouldFindSchoolsBySponsorCode()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { NumberOfResults = 10 });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            var result = service.GetSponsorByCode(1234);

//            Assert.AreEqual(10, result.NumberOfResults);
//        }

//        [Test]
//        public void SchoolApiServiceShouldSearchSchoolByLocation()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            service.SearchSchoolByLocation("EC3R 8BT", 4.8, 0, 50, string.Empty, null);

//            var parameters = new Dictionary<string, string>();
//            parameters.Add("locationorpostcode", "EC3R 8BT");
//            parameters.Add("skip", "0");
//            parameters.Add("take", "50");
//            parameters.Add("orderby", "");
//            parameters.Add("distance", "4.8");
//            parameters.Add("filter","");

//            mockApiRequest.Verify(req => req.Get("school", new List<string> { "search", "geo"}, parameters), Times.Once());
//        }


//        [Test]
//        public void SchoolApiServiceShouldFindSchoolsByLocation()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            var result = service.SearchSchoolByLocation("EC3R 8BT", 4.8, 0, 50, string.Empty, null);

//            Assert.AreEqual("Test School", result.name);
//        }

//        [Test]
//        public void SchoolApiServiceShouldRaiseExceptionIfSearchByLocationApiFails()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.InternalServerError, null);

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            Assert.Throws<ApplicationException>(() => service.SearchSchoolByLocation("EC3R 8BT", 4.8, 0, 50, string.Empty, null));
//        }

//        [Test]
//        public void SchoolApiServiceShouldSearchSchoolByCoordinates()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            service.SearchSchoolByLocation("51.5098583", "-0.08462099999997008", 4.8, 0, 50, string.Empty, null);

//            var parameters = new Dictionary<string, string>();
//            parameters.Add("lat", "51.5098583");
//            parameters.Add("lon", "-0.08462099999997008");
//            parameters.Add("skip", "0");
//            parameters.Add("take", "50");
//            parameters.Add("orderby", "");
//            parameters.Add("distance", "4.8");
//            parameters.Add("filter", "");

//            mockApiRequest.Verify(req => req.Get("school", new List<string> { "search", "geo" }, parameters), Times.Once());
//        }


//        [Test]
//        public void SchoolApiServiceShouldFindSchoolsByCoordinates()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            var result = service.SearchSchoolByLocation("51.5098583", "-0.08462099999997008", 4.8, 0, 50, string.Empty, null);

//            Assert.AreEqual("Test School", result.name);
//        }

//        [Test]
//        public void SchoolApiServiceShouldSearchSchoolByLaCode()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            service.SearchSchoolByLaCode("319", 0, 50, string.Empty, null);

//            var parameters = new Dictionary<string, string>();
//            parameters.Add("lacode", "319");
//            parameters.Add("skip", "0");
//            parameters.Add("take", "50");
//            parameters.Add("orderby", "");
//            parameters.Add("filter", "");

//            mockApiRequest.Verify(req => req.Get("school", new List<string> { "search" }, parameters), Times.Once());
//        }


//        [Test]
//        public void SchoolApiServiceShouldFindSchoolsByLaCode()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);
//            var result = service.SearchSchoolByLaCode("319", 0, 50, string.Empty, null);

//            Assert.AreEqual("Test School", result.name);
//        }

//        [Test]
//        public void SchoolApiServiceShouldSearchSchoolByLaEstab()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);

//            service.SearchSchoolByLaEstab("102976", 0, 50, string.Empty, null);

//            var parameters = new Dictionary<string, string>();            
//            parameters.Add("skip", "0");
//            parameters.Add("take", "50");
//            parameters.Add("orderby", "");
//            parameters.Add("filter", "(LAESTAB eq '102976')");

//            mockApiRequest.Verify(req => req.Get("school", new List<string> { "search" }, parameters), Times.Once());
//        }


//        [Test]
//        public void SchoolApiServiceShouldFindSchoolsByLaEstab()
//        {
//            var mockApiRequest = new Mock<IApiRequest>();
//            var mockApiResponse = new ApiResponse(System.Net.HttpStatusCode.OK, new { name = "Test School" });

//            mockApiRequest.Setup(a => a.Get(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Dictionary<string, string>>())).Returns(mockApiResponse);

//            var service = new SchoolApiService(mockApiRequest.Object);
//            var result = service.SearchSchoolByLaEstab("102976", 0, 50, string.Empty, null);

//            Assert.AreEqual("Test School", result.name);
//        }

//    }
//}
