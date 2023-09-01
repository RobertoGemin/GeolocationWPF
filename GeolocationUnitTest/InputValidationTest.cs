using GeolocationApp.Interface;
using GeolocationApp.Model;
using GeolocationApp.Service;
using GeolocationUnitTest.Function;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GeolocationUnitTest
{
    [TestClass]
    public class InputValidationTest
    {
        public IInputValidationService InputValidationService;

        public Mock<ILoggingService> MockILoggingService;
        public Mock<INotificationService> MockNotificationService;
        public Mock<IHealthService> MockHealthService;
        public TestContext TestContext { get; set; }


        [TestInitialize]
        public void TestInitialize()
        {
            // Setup the MockILoggingService database service
            MockILoggingService = new Mock<ILoggingService>();
            MockNotificationService = new Mock<INotificationService>();
            MockHealthService = new Mock<IHealthService>();

            InputValidationService =
                new InputValidationService(MockNotificationService.Object, MockILoggingService.Object, MockHealthService.Object);
        }


        [TestMethod]
        [DataRow("1.1.1.10", "1.1.1.10")]
        [DataRow("1.1.1.010", "1.1.1.8")]
        public void getIPAdressOrHostName_bySearchText_ReturnIpAdres(string input, string expected)
        {
            var searchModel = InputValidationService.GetIpDomainSearchModel(input);

            TestContext.WriteLine(Helper.DebugText(input, searchModel.Id, expected));
            Assert.IsTrue(searchModel.Id.Contains(expected));
        }


        [TestMethod]
        [DataRow("1.1.1.10", nameof(Enums.EntryType.IPAddress))]
        [DataRow("1.1.1.010", nameof(Enums.EntryType.IPAddress))]
        public void getIPAdressOrHostName_bySearchText_Return(string input, string expected)
        {
            var searchModel = InputValidationService.GetIpDomainSearchModel(input);

            var valuType = searchModel.EntryDataType.ToString();
            TestContext.WriteLine(Helper.DebugText(input, valuType, expected));
            ;
            Assert.IsTrue(valuType.Contains(expected));
        }


        [TestMethod]
        [DataRow("www.google.com", "www.google.com")]
        [DataRow("https://www.youtube.com", "www.youtube.com")]
        public void getIPAdressOrHostName_bySearchText_ReturnHostName(string input, string expected)
        {
            var searchModel = InputValidationService.GetIpDomainSearchModel(input);

            // SearchModel
            TestContext.WriteLine(Helper.DebugText(input, searchModel.Id, expected));
            Assert.IsTrue(searchModel.Id.Contains(expected));
        }

        [TestMethod]
        [DataRow("wwwgooglecom", "")]
        [DataRow("  ", "")]
        public void getIPAdressOrHostName_bySearchText_ReturnNull(string input, string expected)
        {
            var searchModel = InputValidationService.GetIpDomainSearchModel(input);
            // SearchModel
            TestContext.WriteLine(Helper.DebugText(input, searchModel.Id, expected));
            Assert.IsTrue(searchModel.Id.Contains(expected));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MockILoggingService = null;
            MockNotificationService = null;
            InputValidationService = null;
        }
    }
}