using System.Net.Http;
using System.Text;

string baseUrl = "http://localhost:8888/";

using HttpClient client = new HttpClient();
client.BaseAddress = new Uri(baseUrl);

Console.WriteLine($"HTTP Client started. Connecting to {baseUrl}");

try
{
    HttpResponseMessage nameResponse = await client.GetAsync("MyName/");
    if (nameResponse.IsSuccessStatusCode)
    {
        string name = await nameResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Name from server: {name}");
    }
    else
    {
        Console.WriteLine($"Error: {nameResponse.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error connecting to server: {ex.Message}");
}

try
{
    HttpResponseMessage headerResponse = await client.GetAsync("MyNameByHeader/");
    if (headerResponse.IsSuccessStatusCode)
    {
        if (headerResponse.Headers.TryGetValues("X-MyName", out var headerValues))
        {
            string nameFromHeader = headerValues.FirstOrDefault() ?? "Header not found";
            Console.WriteLine($"Name from X-MyName header: {nameFromHeader}");
        }
        else
        {
            Console.WriteLine("X-MyName header not found in response");
        }
    }
    else
    {
        Console.WriteLine($"Error: {headerResponse.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error connecting to server: {ex.Message}");
}

try
{
    HttpResponseMessage cookieResponse = await client.GetAsync("MyNameByCookies/");
    if (cookieResponse.IsSuccessStatusCode)
    {
        if (cookieResponse.Headers.TryGetValues("Set-Cookie", out var cookieValues))
        {
            string cookies = string.Join("; ", cookieValues);
            Console.WriteLine($"Cookies received: {cookies}");

            var nameCookie = cookieValues.FirstOrDefault(c => c.StartsWith("MyName="));
            if (nameCookie != null)
            {
                string nameValue = nameCookie.Split('=', ';')[1].Trim();
                Console.WriteLine($"Name from MyName cookie: {nameValue}");
            }
        }
        else
        {
            Console.WriteLine("No cookies found in response");
        }
    }
    else
    {
        Console.WriteLine($"Error: {cookieResponse.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error connecting to server: {ex.Message}");
}

Console.WriteLine("\n--- Interactive mode ---");
Console.WriteLine("Commands:");
Console.WriteLine("  - Type 'exit' to stop the listener and client");

bool isRunning = true;

while (isRunning)
{
    Console.Write("Enter request: ");
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            StringContent content = new StringContent("exit", Encoding.UTF8, "text/plain");
            HttpResponseMessage exitResponse = await client.PostAsync("/", content);
            string responseBody = await exitResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"\nResponse: {responseBody}");
            Console.WriteLine("Client shutting down...");
            isRunning = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        continue;
    }

    try
    {
        HttpResponseMessage response = await client.GetAsync(input);

        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"\nResponse from server:");
        Console.WriteLine(responseBody);
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error connecting to server: {ex.Message}");
    }
}

Console.WriteLine("Client stopped.");
