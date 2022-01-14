using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct StructModel
{
    [JsonPropertyName("struct")] public string Name { get; set; }

    [JsonPropertyName("consts")] public ConstantModel[]? Constants { get; set; }  
    [JsonPropertyName("fields")] public FieldModel[]? Fields { get; set; }
    [JsonPropertyName("methods")] public MethodModel[]? Methods { get; set; }
}