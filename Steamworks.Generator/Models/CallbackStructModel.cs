using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct CallbackStructModel
{
    /// <summary>
    /// The name of the callback struct
    /// </summary>
    [JsonPropertyName("struct")]
    public string Name { get; set; }

    /// <summary>
    /// The id of the callback struct
    /// </summary>
    [JsonPropertyName("callback_id")]
    public int CallbackId { get; set; }

    /// <summary>
    /// The constants of the callback struct
    /// </summary>
    [JsonPropertyName("consts")]
    public ConstantModel[]? Constants { get; set; }

    /// <summary>
    /// The enums of the callback struct
    /// </summary>
    [JsonPropertyName("enums")]
    public EnumModel[]? Enums { get; set; }

    /// <summary>
    /// The fields of the callback struct
    /// </summary>
    [JsonPropertyName("fields")]
    public FieldModel[]? Fields { get; set; }
}