using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct ParameterModel
{
    /// <summary>
    /// The name of the parameter
    /// </summary>
    [JsonPropertyName("paramname")]
    public string Name { get; set; }

    /// <summary>
    /// The type of the parameter
    /// </summary>
    [JsonPropertyName("paramtype")]
    public string Type { get; set; }

    /// <summary>
    /// The flat type of the parameter
    /// </summary>
    [JsonPropertyName("paramtype_flat")]
    public string FlatType { get; set; }

    /// <summary>
    /// The description of the parameter
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; }

    /// <summary>
    /// The length of the array
    /// </summary>
    /// <remarks>Type must be a pointer</remarks>
    [JsonPropertyName("array_count")]
    public string ArrayCount { get; set; }

    /// <summary>
    /// The number of characters outputted
    /// </summary>
    /// <remarks>Type must be a pointer of <see cref="char"/></remarks>
    [JsonPropertyName("out_string_count")]
    public string OutStringCount { get; set; }

    /// <summary>
    /// The length of the array. Value is a magic string?
    /// </summary>
    [JsonPropertyName("out_array_count")]
    public string OutArrayCount { get; set; }
}