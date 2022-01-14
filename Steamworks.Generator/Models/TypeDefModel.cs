using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct TypeDefModel
{
    [JsonPropertyName("typedef")] public string Name { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
}