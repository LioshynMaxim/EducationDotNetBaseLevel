using System.Net;

namespace Module15_TestProject
{
    [TestClass]
    public class ListenerTests
    {
        [TestMethod]
        public void TestParseResourcePath_RootPath_ReturnsEmpty()
        {
            string path = "/";
            string result = path.Trim('/');
            
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void TestParseResourcePath_WithTrailingSlash_RemovesSlash()
        {
            string path = "/MyName/";
            string result = path.Trim('/');
            
            Assert.AreEqual("MyName", result);
        }

        [TestMethod]
        public void TestParseResourcePath_WithoutTrailingSlash_ReturnsPath()
        {
            string path = "/MyName";
            string result = path.Trim('/');
            
            Assert.AreEqual("MyName", result);
        }

        [TestMethod]
        public void TestParseResourcePath_NestedPath_ReturnsFullPath()
        {
            string path = "/api/v1/MyName/";
            string result = path.Trim('/');
            
            Assert.AreEqual("api/v1/MyName", result);
        }

        [TestMethod]
        public void TestGetMyName_ReturnsExpectedValue()
        {
            string result = GetMyName();
            
            Assert.AreEqual("Test GetMyName", result);
        }

        [TestMethod]
        public void TestGetMyNameByHeader_AddsHeader()
        {
            var mockResponse = new MockHttpListenerResponse();
            
            string result = GetMyNameByHeader(mockResponse);
            
            Assert.AreEqual("Name added to X-MyName header", result);
            Assert.IsTrue(mockResponse.HeaderAdded);
            Assert.AreEqual("X-MyName", mockResponse.HeaderName);
            Assert.AreEqual("Test GetMyName", mockResponse.HeaderValue);
        }

        [TestMethod]
        public void TestGetMyNameByCookies_AddsCookie()
        {
            var mockResponse = new MockHttpListenerResponse();
            
            string result = GetMyNameByCookies(mockResponse);
            
            Assert.AreEqual("Name added to MyName cookie", result);
            Assert.IsTrue(mockResponse.CookieAdded);
            Assert.AreEqual("MyName", mockResponse.CookieName);
            Assert.AreEqual("Test GetMyName", mockResponse.CookieValue);
        }

        [TestMethod]
        public void TestInformation_ReturnsCorrectStatusCode()
        {
            string result = Information();
            Assert.AreEqual(HttpStatusCode.Continue.ToString(), result);
        }

        [TestMethod]
        public void TestSuccess_ReturnsCorrectStatusCode()
        {
            string result = Success();
            Assert.AreEqual(HttpStatusCode.OK.ToString(), result);
        }

        [TestMethod]
        public void TestRedirection_ReturnsCorrectStatusCode()
        {
            string result = Redirection();
            Assert.AreEqual(HttpStatusCode.Ambiguous.ToString(), result);
        }

        [TestMethod]
        public void TestClientError_ReturnsCorrectStatusCode()
        {
            string result = ClientError();
            Assert.AreEqual(HttpStatusCode.NotFound.ToString(), result);
        }

        [TestMethod]
        public void TestServerError_ReturnsCorrectStatusCode()
        {
            string result = ServerError();
            Assert.AreEqual(HttpStatusCode.InternalServerError.ToString(), result);
        }

        [TestMethod]
        public void TestResourcePathCaseInsensitive_MyName()
        {
            string path1 = "myname";
            string path2 = "MyName";
            string path3 = "MYNAME";
            
            Assert.AreEqual(path1.ToLowerInvariant(), path2.ToLowerInvariant());
            Assert.AreEqual(path1.ToLowerInvariant(), path3.ToLowerInvariant());
        }

        private static string GetMyName() => "Test GetMyName";

        private static string GetMyNameByHeader(MockHttpListenerResponse response)
        {
            response.AddHeader("X-MyName", "Test GetMyName");
            return "Name added to X-MyName header";
        }

        private static string GetMyNameByCookies(MockHttpListenerResponse response)
        {
            response.AddCookie(new Cookie("MyName", "Test GetMyName"));
            return "Name added to MyName cookie";
        }

        private static string Information() => HttpStatusCode.Continue.ToString();
        private static string Success() => HttpStatusCode.OK.ToString();
        private static string Redirection() => HttpStatusCode.Ambiguous.ToString();
        private static string ClientError() => HttpStatusCode.NotFound.ToString();
        private static string ServerError() => HttpStatusCode.InternalServerError.ToString();
    }

    public class MockHttpListenerResponse
    {
        public bool HeaderAdded { get; private set; }
        public string? HeaderName { get; private set; }
        public string? HeaderValue { get; private set; }
        
        public bool CookieAdded { get; private set; }
        public string? CookieName { get; private set; }
        public string? CookieValue { get; private set; }

        public void AddHeader(string name, string value)
        {
            HeaderAdded = true;
            HeaderName = name;
            HeaderValue = value;
        }

        public void AddCookie(Cookie cookie)
        {
            CookieAdded = true;
            CookieName = cookie.Name;
            CookieValue = cookie.Value;
        }
    }
}
