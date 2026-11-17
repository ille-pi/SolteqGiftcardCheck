using System.Text.Json;
using System.Text.Json.Serialization;

public class Giftcard
{
    [JsonPropertyName("giftcardTransactions")]
    public List<JsonElement> GiftcardTransactions { get; set; } = [];

    [JsonPropertyName("giftcardDeliveries")]
    public List<JsonElement> GiftcardDeliveries { get; set; } = [];

    [JsonPropertyName("companyCode")]
    public string CompanyCode { get; set; } = string.Empty;

    [JsonPropertyName("giftcardId")]
    public string GiftcardId { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("templateName")]
    public string TemplateName { get; set; } = string.Empty;

    [JsonPropertyName("createdDateTimeUtc")]
    public DateTimeOffset CreatedDateTimeUtc { get; set; }

    [JsonPropertyName("initialValue")]
    public decimal InitialValue { get; set; }

    [JsonPropertyName("leftValue")]
    public decimal LeftValue { get; set; }

    [JsonPropertyName("initialUsages")]
    public int InitialUsages { get; set; }

    [JsonPropertyName("leftUsages")]
    public int LeftUsages { get; set; }

    [JsonPropertyName("customerReference")]
    public string? CustomerReference { get; set; }

    [JsonPropertyName("customer")]
    public JsonElement? Customer { get; set; }

    [JsonPropertyName("customerLink")]
    public string? CustomerLink { get; set; }

    [JsonPropertyName("validFromUtc")]
    public DateTimeOffset ValidFromUtc { get; set; }

    [JsonPropertyName("validToUtc")]
    public DateTimeOffset ValidToUtc { get; set; }

    [JsonPropertyName("createdBy")]
    public string CreatedBy { get; set; } = string.Empty;

    [JsonPropertyName("createdAtWorkstation")]
    public string CreatedAtWorkstation { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("revokeReason")]
    public string? RevokeReason { get; set; }

    [JsonPropertyName("revokeDateTimeUtc")]
    public DateTimeOffset? RevokeDateTimeUtc { get; set; }

    [JsonPropertyName("lastUsedWorkstation")]
    public string? LastUsedWorkstation { get; set; }

    [JsonPropertyName("isPreCreated")]
    public bool IsPreCreated { get; set; }

    [JsonPropertyName("isExpired")]
    public bool IsExpired { get; set; }
}