using GeolocationApp.Methods;
using GeolocationUnitTest.Function;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GeolocationUnitTest
{
    [TestClass]
    public class FunctiontTest
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        [DataRow("https://www.google.com/   test", "https://www.google.com/test")]
        [DataRow("https://www.google.com/", "https://www.google.com/")]
        [DataRow("https://www.google,com/", "https://www.google.com/")]
        [DataRow("https://www.google..com/", "https://www.google.com/")]
        public void CheckIfUrIsCleanUp(string url, string expect)
        {
            var cleanString = Validate.removeNoise(url);
            TestContext.WriteLine($"{nameof(cleanString)} :{cleanString}  {nameof(expect)}{expect}");
            Assert.IsTrue(cleanString == expect);
        }


        [TestMethod]
        [DataRow("google.com")]
        [DataRow("www.google.com")]
        [DataRow("http://google.com")]
        [DataRow("http://www.google.com")]
        [DataRow("https://google.com/test/test")]
        [DataRow("https://www.google.com/test")]
        [DataRow("https://www.google.com/test/")]
        [DataRow("https://nl.search.yahoo.com/web?")]
        [DataRow("https://www.eventdata.crossref.org/guide/data/ids-and-urls/#removing-tracking-urls")]
        public void CheckisValidURL_ByCleanUrl_ReturnTrue(string url)
        {
            var isvalid = Validate.isValidURL(url.Trim());
            Assert.IsTrue(isvalid);
        }

        [TestMethod]
        [DataRow(@"http://google.co\m")]
        [DataRow(@"http://googlecom")]
        [DataRow(@"googlecom")]
        [DataRow("www.googlecom")]
        public void CheckIsvalid_ByCleanUrl_ReturnFalse(string url)
        {
            var isvalid = Validate.isValidURL(url);
            Assert.IsFalse(isvalid);
        }




        [TestClass]
        public class getIPAdress_byinput
        {
            public TestContext TestContext { get; set; }


            [TestMethod]
            [DataRow("65535", "0.0.255.255")]
            [DataRow("20.2", "20.0.0.2")]
            [DataRow("20.65535", "20.0.255.255")]
            [DataRow("128.1.2", "128.1.0.2")]
            [DataRow("1.1.1.10", "1.1.1.10")]
            [DataRow("1.1.1.010", "1.1.1.8")]
            [DataRow("2345:0425:2CA1:0000:0000:0567:5673:23b5", "2345:425:2ca1::567:5673:23b5")]
            // get IPAdress by input
            public void getIPAdress_byinput_ReturnIPAdress(string input, string expected)
            {
                var result = Validate.getIPAddress(input);
                TestContext.WriteLine(Helper.DebugText(input, result, expected));
                Assert.IsTrue(result.Equals(expected),
                    $"{nameof(result)} :{result} ==  {nameof(expected)} :{expected}");
            }
        }
    }
}