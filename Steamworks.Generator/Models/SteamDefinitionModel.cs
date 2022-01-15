using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct SteamDefinitionModel
{
    [JsonPropertyName("consts")] public ConstantModel[]? Constants { get; set; }
    [JsonPropertyName("typedefs")] public TypeDefModel[]? TypeDefs { get; set; }
    [JsonPropertyName("enums")] public EnumModel[]? Enums { get; set; }
    [JsonPropertyName("interfaces")] public InterfaceModel[]? Interfaces { get; set; }
    [JsonPropertyName("structs")] public StructModel[]? Structs { get; set; }
    [JsonPropertyName("callback_structs")] public CallbackStructModel[]? CallbackStructs { get; set; }

    /// <summary>
    /// Additional method to places in SteamNative
    /// </summary>
    [JsonPropertyName("native_methods")] public MethodModel[]? NativeMethods { get; set; }
}