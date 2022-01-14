using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct InterfaceModel
{
    /// <summary>
    /// The name of the interface
    /// </summary>
    [JsonPropertyName("classname")]
    public string Name { get; set; }

    /// <summary>
    /// The version of the interface
    /// </summary>
    [JsonPropertyName("version_string")]
    public string Version { get; set; }

    /// <summary>
    /// The fields of the interface
    /// </summary>
    [JsonPropertyName("fields")]
    public FieldModel[]? Fields { get; set; }

    /// <summary>
    /// The methods of the interface
    /// </summary>
    [JsonPropertyName("methods")]
    public MethodModel[]? Methods { get; set; }

    /// <summary>
    /// The accessors of the interface
    /// </summary>
    [JsonPropertyName("accessors")]
    public InterfaceAccessorModel[]? Accessors { get; set; }

    /// <summary>
    /// The enums of the interface
    /// </summary>
    [JsonPropertyName("enums")]
    public EnumModel[]? Enums { get; set; }
}