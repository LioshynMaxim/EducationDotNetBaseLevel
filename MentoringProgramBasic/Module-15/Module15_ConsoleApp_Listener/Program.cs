using System.Net;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

string url = "http://localhost:8888/";
HttpListener listener = new HttpListener();
listener.Prefixes.Add(url);
listener.Start();

Dictionary<string, Func<HttpListenerResponse, string>> resourceHandlers = new()
{
    { "myname", (response) => GetMyName() },
    { "mynamebyheader", GetMyNameByHeader },
    { "mynamebycookies", GetMyNameByCookies },
    { "information", (response) => Information() },
    { "success", (response) => Success() },
    { "redirection", (response) => Redirection() },
    { "clienterror", (response) => ClientError() },
    { "servererror", (response) => ServerError() }
};

Console.WriteLine($"Listener started on {url}");
Console.WriteLine("Waiting for requests... Send 'exit' command to stop.");

bool isRunning = true;

while (isRunning)
{
    HttpListenerContext context = await listener.GetContextAsync();
    HttpListenerRequest request = context.Request;
    HttpListenerResponse response = context.Response;

    string requestBody = string.Empty;
    if (request.HasEntityBody)
    {
        using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            requestBody = await reader.ReadToEndAsync();
        }
    }

    Console.WriteLine($"\nReceived request:");
    Console.WriteLine($"Method: {request.HttpMethod}");
    Console.WriteLine($"URL: {request.Url}");
    Console.WriteLine($"Body: {requestBody}");

    string resourcePath = ParseResourcePath(request);
    Console.WriteLine($"Resource Path: {resourcePath}");

    string responseString;
    if (requestBody.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        responseString = "Listener shutting down...";
        isRunning = false;
    }
    else if (resourceHandlers.TryGetValue(resourcePath.ToLowerInvariant(), out Func<HttpListenerResponse, string> handler))
    {
        responseString = handler(response);
    }
    else
    {
        responseString = $"Request received: {request.HttpMethod} {request.Url?.PathAndQuery}\nBody: {requestBody}";
    }

    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
    response.ContentLength64 = buffer.Length;
    response.ContentType = "text/plain";
    await response.OutputStream.WriteAsync(buffer);
    response.OutputStream.Close();
}

listener.Stop();
listener.Close();
Console.WriteLine("Listener stopped.");

static string ParseResourcePath(HttpListenerRequest request)
{
    string path = request.Url?.AbsolutePath ?? "/";
    path = path.Trim('/');
    return path;
}

static string GetMyName() => "Test GetMyName";

static string GetMyNameByHeader(HttpListenerResponse response)
{
    response.Headers.Add("X-MyName", "Test GetMyName");
    return "Name added to X-MyName header";
}

static string GetMyNameByCookies(HttpListenerResponse response)
{
    Cookie nameCookie = new Cookie("MyName", "Test GetMyName");
    response.Cookies.Add(nameCookie);
    return "Name added to MyName cookie";
}

static string Information() => HttpStatusCode.Continue.ToString();
static string Success() => HttpStatusCode.OK.ToString();
static string Redirection() => HttpStatusCode.Ambiguous.ToString();
static string ClientError () => HttpStatusCode.NotFound.ToString();
static string ServerError () => HttpStatusCode.InternalServerError.ToString();