using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct InterfaceAccessorModel
{
    /// <summary>
    /// The kind of the accessor 
    /// </summary>
    /// <remarks>Known values: "user", "gameserver" and "global"</remarks>
    [JsonPropertyName("kind")] public string Kind { get; set; }
    
    /// <summary>
    /// The name of the accessor
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set; }
    
    /// <summary>
    /// The flat name of the accessor
    /// </summary>
    [JsonPropertyName("name_flat")] public string FlatName { get; set; }
}