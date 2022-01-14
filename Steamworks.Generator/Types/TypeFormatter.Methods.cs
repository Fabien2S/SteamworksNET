using Steamworks.Generator.Models;

namespace Steamworks.Generator.Types;

public static partial class TypeFormatter
{
    public static bool TryFormatMethod(ref MethodModel method)
    {
        method.ReturnType = FormatMethodReturnType(in method);

        var parameters = method.Parameters;
        if (parameters != null)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i].Type = FormatMethodParameterType(parameters[i]);
            }

            method.Parameters = parameters;
        }

        // Fix operator for now
        if (method.Name.Contains("operator", StringComparison.Ordinal))
            method.Name = method.FlatName[(method.FlatName.LastIndexOf('_') + 1)..];

        return true;
    }

    private static string FormatMethodReturnType(in MethodModel method)
    {
        var type = TypeConverter.ConvertType(method.ReturnType);
        if (type.Equals("string", StringComparison.Ordinal))
            type = "Utf8String";
        return type;
    }

    private static string FormatMethodParameterType(in ParameterModel parameter)
    {
        var type = TypeConverter.ConvertType(parameter.Type);
        return type;
    }
}