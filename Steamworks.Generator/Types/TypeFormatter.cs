namespace Steamworks.Generator.Types;

public static partial class TypeFormatter
{
    private static string FormatValue(string value)
    {
        // GENERIC: long long became long in C#
        if (value.EndsWith("ll"))
            value = value[..1];

        return value;
    }
}