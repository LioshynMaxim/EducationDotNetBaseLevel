using System.Net;
using System.Net.Http;

namespace Module15_TestProject
{
    [TestClass]
    public sealed class HttpListenerIntegrationTests
    {
        private static HttpListener? _listener;
        private static Task? _listenerTask;
        private static CancellationTokenSource? _cancellationTokenSource;
        private static HttpClient? _client;
        private const string BaseUrl = "http://localhost:9999/";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _listener = new HttpListener();
            _listener.Prefixes.Add(BaseUrl);

            try
            {
                _listener.Start();
                Thread.Sleep(1000);
            }
            catch (HttpListenerException)
            {
                Thread.Sleep(3000);
                _listener.Start();
            }

            _listenerTask = Task.Run(async () => await RunListenerAsync(_cancellationTokenSource.Token));

            _client = new HttpClient 
            { 
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(60)
            };
            Thread.Sleep(2000);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _cancellationTokenSource?.Cancel();
            _client?.Dispose();
            Thread.Sleep(1000);
            try
            {
                _listener?.Stop();
                _listener?.Close();
            }
            catch { }
            try
            {
                _listenerTask?.Wait(TimeSpan.FromSeconds(5));
            }
            catch { }
        }

        private static async Task RunListenerAsync(CancellationToken cancellationToken)
        {
            Dictionary<string, Func<HttpListenerResponse, string>> resourceHandlers = new()
            {
                { "myname", (response) => "Test GetMyName" },
                { "mynamebyheader", GetMyNameByHeader },
                { "mynamebycookies", GetMyNameByCookies }
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_listener == null || !_listener.IsListening)
                        break;

                    HttpListenerContext context = await _listener.GetContextAsync();

                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    string resourcePath = request.Url?.AbsolutePath.Trim('/') ?? "";

                    string responseString;
                    if (resourceHandlers.TryGetValue(resourcePath.ToLowerInvariant(), out var handler))
                    {
                        responseString = handler(response);
                    }
                    else
                    {
                        responseString = $"Unknown resource: {resourcePath}";
                    }

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.ContentType = "text/plain";

                    await response.OutputStream.WriteAsync(buffer);
                    response.Close();
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            }
        }

        private static string GetMyNameByHeader(HttpListenerResponse response)
        {
            response.Headers.Add("X-MyName", "Test GetMyName");
            return "Name added to X-MyName header";
        }

        private static string GetMyNameByCookies(HttpListenerResponse response)
        {
            Cookie nameCookie = new Cookie("MyName", "Test GetMyName");
            response.Cookies.Add(nameCookie);
            return "Name added to MyName cookie";
        }

        [TestMethod]
        public async Task Test01_GetMyName_ReturnsName()
        {
            await Task.Delay(1000);
            var response = await _client!.GetAsync("MyName");

            Assert.IsTrue(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Test GetMyName", content);
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test02_GetMyNameByHeader_ReturnsHeaderWithName()
        {
            await Task.Delay(1000);
            var response = await _client!.GetAsync("MyNameByHeader");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.Headers.Contains("X-MyName"));
            var headerValue = response.Headers.GetValues("X-MyName").First();
            Assert.AreEqual("Test GetMyName", headerValue);
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test03_GetMyNameByCookies_ReturnsCookieWithName()
        {
            await Task.Delay(1000);
            var response = await _client!.GetAsync("MyNameByCookies");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(response.Headers.Contains("Set-Cookie"));

            var cookieValues = response.Headers.GetValues("Set-Cookie");
            var nameCookie = cookieValues.FirstOrDefault(c => c.StartsWith("MyName="));

            Assert.IsNotNull(nameCookie);
            StringAssert.Contains(nameCookie, "Test GetMyName");
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test04_ParseResourcePath_WithTrailingSlash()
        {
            await Task.Delay(1000);
            var response = await _client!.GetAsync("MyName/");

            Assert.IsTrue(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Test GetMyName", content);
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test05_ParseResourcePath_WithoutTrailingSlash()
        {
            await Task.Delay(1000);
            var response = await _client!.GetAsync("MyName");

            Assert.IsTrue(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Test GetMyName", content);
            await Task.Delay(500);
        }

        [TestMethod]
        public async Task Test06_UnknownResource_ReturnsUnknownMessage()
        {
            await Task.Delay(1000);
            var response = await _client!.GetAsync("UnknownPath");

            Assert.IsTrue(response.IsSuccessStatusCode);
            var content = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(content, "Unknown resource");
            await Task.Delay(500);
        }
    }
}
