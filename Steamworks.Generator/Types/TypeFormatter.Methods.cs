using Steamworks.Generator.Models;

namespace Steamworks.Generator.Types;

public static partial class TypeFormatter
{
    public static void FormatMethodFacing(ref MethodModel method)
    {
        method.ReturnType = FormatMethodFacingReturnType(in method);

        var parameters = method.Parameters;
        if (parameters != null)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i].Type = TypeConverter.ConvertType(parameters[i].Type);
            }

            method.Parameters = parameters;
        }

        // TODO Proper implementation
        // Fix operator for now
        if (method.Name.Contains("operator", StringComparison.Ordinal))
            method.Name = method.FlatName[(method.FlatName.LastIndexOf('_') + 1)..];
    }

    private static string FormatMethodFacingReturnType(in MethodModel method)
    {
        var type = TypeConverter.ConvertType(method.ReturnType);

        var callResult = method.CallResult;
        if (type.Equals("SteamAPICall_t", StringComparison.Ordinal) && !string.IsNullOrEmpty(callResult))
        {
            return "CallHandle<" + callResult + ">";
        }

        return type;
    }

    public static void FormatMethodNative(ref MethodModel method)
    {
        method.ReturnType = FormatMethodNativeReturnType(in method);

        var parameters = method.Parameters;
        if (parameters != null)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i].Type = FormatMethodNativeParameterType(parameters[i]);
            }

            method.Parameters = parameters;
        }

        // Fix operator for now
        // if (method.Name.Contains("operator", StringComparison.Ordinal))
        //     method.Name = method.FlatName[(method.FlatName.LastIndexOf('_') + 1)..];
    }

    private static string FormatMethodNativeReturnType(in MethodModel method)
    {
        var type = TypeConverter.ConvertType(method.ReturnType);
        if (type.Equals("string", StringComparison.Ordinal))
            type = "NativeString";
        return type;
    }

    private static string FormatMethodNativeParameterType(in ParameterModel parameter)
    {
        var type = TypeConverter.ConvertType(parameter.Type);
        return type;
    }
}