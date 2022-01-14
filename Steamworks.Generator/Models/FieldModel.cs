using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct FieldModel
{
    /// <summary>
    /// The name of the field
    /// </summary>
    [JsonPropertyName("fieldname")]
    public string Name { get; set; }

    /// <summary>
    /// The type of the field
    /// </summary>
    [JsonPropertyName("fieldtype")]
    public string Type { get; set; }

    /// <summary>
    /// The visibility of the field
    /// </summary>
    [JsonPropertyName("private")]
    public bool IsPrivate { get; set; }
    
    /// <summary>
    /// The custom attribute of the field
    /// </summary>
    [JsonIgnore]
    public string? CustomAttribute { get; set; }
}