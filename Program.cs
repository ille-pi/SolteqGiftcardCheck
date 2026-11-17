using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
var allGiftcards = new List<JsonElement>();

var apiUrl = "https://sq-giftcards-api-2-stage.azurewebsites.net/v1/Giftcard";

httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

while (true)
{
    var pagedUrl = $"{apiUrl}?Skip={skip}&Take={pageSize}";

    var giftcardResponse = await httpClient.GetAsync(pagedUrl);
    giftcardResponse.EnsureSuccessStatusCode();

    var giftcardContent = await giftcardResponse.Content.ReadAsStringAsync();

    var giftcards = JsonSerializer.Deserialize<List<JsonElement>>(giftcardContent);

    if (giftcards == null || giftcards.Count == 0)
    {
        break;
    }

    allGiftcards.AddRange(giftcards);

    Console.WriteLine($"Fetched {giftcards.Count} giftcards. Total: {allGiftcards.Count}");

    skip += pageSize;
}

Console.WriteLine($"Finished fetching. Total giftcards: {allGiftcards.Count}");


public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}
