using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct EnumModel
{
    /// <summary>
    /// The name of the enum
    /// </summary>
    [JsonPropertyName("enumname")]
    public string Name { get; set; }

    /// <summary>
    /// The fq name of the enum
    /// </summary>
    [JsonPropertyName("fqname")]
    public string FqName { get; set; }

    /// <summary>
    /// The values of the enum
    /// </summary>
    [JsonPropertyName("values")]
    public ValueModel[] Values { get; set; }
}