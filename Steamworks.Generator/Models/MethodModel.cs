using System.Text.Json.Serialization;

namespace Steamworks.Generator.Models;

public struct MethodModel
{
    /// <summary>
    /// The name of the method
    /// </summary>
    [JsonPropertyName("methodname")]
    public string Name { get; set; }

    /// <summary>
    /// The flat name of the method
    /// </summary>
    [JsonPropertyName("methodname_flat")]
    public string FlatName { get; set; }

    /// <summary>
    /// The parameters of the method
    /// </summary>
    [JsonPropertyName("params")]
    public ParameterModel[]? Parameters { get; set; }

    /// <summary>
    /// The return type of the method
    /// </summary>
    [JsonPropertyName("returntype")]
    public string ReturnType { get; set; }

    /// <summary>
    /// The flat return type of the method
    /// </summary>
    [JsonPropertyName("returntype_flat")]
    public string FlatReturnType { get; set; }

    /// <summary>
    /// <a href="https://partner.steamgames.com/doc/sdk/api#callresults">CallResults</a> on Steamworks Docs
    /// </summary>
    [JsonPropertyName("callresult")]
    public string? CallResult { get; set; }

    // 
    /// <summary>
    /// <a href="https://partner.steamgames.com/doc/sdk/api#callbacks">Callbacks</a> on Steamworks Docs
    /// </summary>
    [JsonPropertyName("callback")]
    public string? Callback { get; set; }
}