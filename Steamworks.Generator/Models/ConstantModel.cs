using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct ConstantModel
{
    /// <summary>
    /// The constant name
    /// </summary>
    [JsonPropertyName("constname")] public string Name { get; set; }
    
    /// <summary>
    /// The constant type
    /// </summary>
    [JsonPropertyName("consttype")] public string Type { get; set; }
    
    /// <summary>
    /// The constant value
    /// </summary>
    [JsonPropertyName("constval")] public string Value { get; set; }
}