// See https://aka.ms/new-console-template for more information
using ConsoleFrontend.Dtos;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

Console.WriteLine("transport hub \"TUI\"");

var serializeOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

Dictionary<int, (string name, Func<Task> action)> actions = new()
{
    { 1, ("Check connection", Ping) },
    { 2, ("Login", Login ) },
};

for (; ; )
{
    Console.WriteLine($"""
        What do you wish to do?
        {string.Join("\n", actions.Select(x => $"{x.Key}: {x.Value.name}"))}
        q: Quit
        """);

    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (string.Equals(input, "q", StringComparison.CurrentCultureIgnoreCase))
        break;

    if (!int.TryParse(input, out var actionIndex))
        continue;

    if (!actions.TryGetValue(actionIndex, out var value))
        continue;

    Console.WriteLine($"\nRunning `{value.name}`");

    try
    {
        await value.action();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("!!!AN ERROR OCCURED!!!\n{0}", ex);
    }
}


async Task Ping()
{
    var client = Client();
    var response = await client.GetAsync("check-connection");

    await PrintResponse(response);
}

async Task Login()
{
    var login = AskUser("Login: ");
    var password = AskUser("Password: ");

    var client = Client();

    var body = new LoginRequest(login ?? "", password ?? "");

    var response = await client.PostAsync("auth/login", GetContent(body));

    await PrintResponse(response);
}

string? AskUser(string prompt = "", bool newLine = false)
{
    Console.Write(prompt + (newLine ? "\n" : ""));
    return Console.ReadLine();
}


HttpClient Client(string? token = null)
{
    var client = new HttpClient
    {
        BaseAddress = new Uri("http://127.0.0.1:8080/api/v1/"),
        
    };

    if (token is not null)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", token);
    }

    return client;
}

async Task PrintResponse(HttpResponseMessage response)
{
    Console.WriteLine($"""
        Url: {response.RequestMessage?.RequestUri},
        Status code: {response.StatusCode}, {getTokenInfo()}

        Body:
        {await response.Content.ReadAsStringAsync()}


        """);

    string getTokenInfo()
        => (response.RequestMessage?.Headers?.Authorization is AuthenticationHeaderValue value ? $"\nToken: {value.Parameter}" : "");
}

HttpContent GetContent<T>(T value)
{
    var payload = JsonSerializer.Serialize(value, serializeOptions);
    return new StringContent(payload, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
}
