using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHttpClient();

var clientId = builder.Configuration["Solteq:ClientId"];
var clientSecret = builder.Configuration["Solteq:ClientSecret"];

if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
{
    throw new InvalidOperationException("Solteq credentials are missing.");
}

var app = builder.Build();
var httpClientFactory = app.Services.GetRequiredService<IHttpClientFactory>();
var httpClient = httpClientFactory.CreateClient();

var values = new Dictionary<string, string>
{
    { "grant_type", "client_credentials" },
    { "scope", "giftcards-api" },
    { "client_id", clientId },
    { "client_secret", clientSecret }
};
var content = new FormUrlEncodedContent(values);
var response = await httpClient.PostAsync(
    "https://login-integration.smartretail.cloud/connect/token",
    content);

response.EnsureSuccessStatusCode();

var responseContent = await response.Content.ReadAsStringAsync();
var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

var accessToken = tokenResponse?.AccessToken;
if (string.IsNullOrEmpty(accessToken))
{
    throw new InvalidOperationException("Failed to retrieve access token.");
}

Console.WriteLine("Access token retrieved successfully.");

const int pageSize = 100;
var skip = 0;
var allGiftcards = new List<Giftcard>();

var apiUrl = "https://sq-giftcards-api-2-stage.azurewebsites.net/v1/Giftcard";

httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

while (true)
{
    var pagedUrl = $"{apiUrl}?Skip={skip}&Take={pageSize}";

    var giftcardResponse = await httpClient.GetAsync(pagedUrl);
    giftcardResponse.EnsureSuccessStatusCode();

    var giftcardContent = await giftcardResponse.Content.ReadAsStringAsync();

    var giftcards = JsonSerializer.Deserialize<List<Giftcard>>(giftcardContent);

    if (giftcards == null || giftcards.Count == 0)
    {
        break;
    }

    allGiftcards.AddRange(giftcards);

    Console.WriteLine($"Fetched {giftcards.Count} giftcards. Total: {allGiftcards.Count}");

    skip += pageSize;
}
Console.WriteLine("\n--- Giftcard statistics ---");

var statusGroups = allGiftcards
    .GroupBy(card => card.Status);

foreach (var group in statusGroups)
{
    Console.WriteLine($"Status '{group.Key}': {group.Count()}");
}

var expiredCount = allGiftcards.Count(card => card.IsExpired);

var notStartedCount = allGiftcards.Count(card =>
    card.ValidFromUtc > DateTimeOffset.UtcNow);

var currentlyValidCount = allGiftcards.Count(card =>
    card.ValidFromUtc <= DateTimeOffset.UtcNow &&
    card.ValidToUtc >= DateTimeOffset.UtcNow &&
    !card.IsExpired);

Console.WriteLine($"Total giftcards: {allGiftcards.Count}");
Console.WriteLine($"Expired: {expiredCount}");
Console.WriteLine($"Not started: {notStartedCount}");
Console.WriteLine($"Currently valid: {currentlyValidCount}");
Console.WriteLine($"Finished fetching. Total giftcards: {allGiftcards.Count}");

var validGiftcards = allGiftcards
    .Where(card =>
        card.Status == "Active" &&
        card.ValidFromUtc <= DateTimeOffset.UtcNow &&
        card.ValidToUtc >= DateTimeOffset.UtcNow &&
        !card.IsExpired)
    .ToList();

Console.WriteLine($"Open and valid giftcards: {validGiftcards.Count}");

var csvPath = Path.Combine(
    AppContext.BaseDirectory,
    "giftcards.csv");

var properties = typeof(Giftcard).GetProperties();

var csv = new StringBuilder();
Console.WriteLine($"Giftcard properties: {properties.Length}");

foreach (var property in properties)
{
    Console.WriteLine(property.Name);
}

// Otsikkorivi
csv.AppendLine(string.Join(",",
    properties.Select(property => property.Name)));

// Lahjakorttien rivit
foreach (var giftcard in validGiftcards)
{
    var rowValues = properties.Select(property =>
    {
        var value = property.GetValue(giftcard);

        if (value == null)
            return "";

        string stringValue;

        if (value is JsonElement jsonElement)
        {
            stringValue = jsonElement.GetRawText();
        }
        else if (value is System.Collections.IEnumerable enumerable
                 && value is not string)
        {
            stringValue = JsonSerializer.Serialize(value);
        }
        else
        {
            stringValue = value.ToString() ?? "";
        }

        return EscapeCsvValue(stringValue);
    });

    csv.AppendLine(string.Join(",", rowValues));
}

await File.WriteAllTextAsync(
    csvPath,
    csv.ToString(),
    Encoding.UTF8);

Console.WriteLine($"CSV exported to: {csvPath}");

static string EscapeCsvValue(string value)
{
    if (value.Contains(',') || value.Contains('"') ||
        value.Contains('\n') || value.Contains('\r'))
    {
        value = value.Replace("\"", "\"\"");
        return $"\"{value}\"";
    }

    return value;
}

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}
