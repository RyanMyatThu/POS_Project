using System.Text.Json.Serialization;

namespace YaungMel_POS.Shared;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomerTier
{
    None,
    Silver,
    Gold,
    Platinum
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RedemptionStatus
{
    Pending,
    Fulfilled,
    Cancelled
}
