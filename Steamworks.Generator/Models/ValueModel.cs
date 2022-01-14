using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct ValueModel
{
    /// <summary>
    /// The name of the enum value
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// The actual value of the enum value
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }
}