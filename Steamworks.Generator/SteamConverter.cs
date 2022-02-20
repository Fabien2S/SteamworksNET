using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Steamworks.Generator.Types;

namespace Steamworks.Generator;

public static class SteamConverter
{
    private static readonly Regex FixedSizeArrayPattern = new(@"^(\w+) \[(\d+)\]$");
    private static readonly Regex DelegatePattern = new(@"^void \(\*\)\((.+)\)$");
    private static readonly Regex PointerTypePattern = new(@"^(?:const )?(\w+) ([\*&]+)$");

    public static bool IsFixedSizeArrayType(string type, out string outType, out string outSize)
    {
        var match = FixedSizeArrayPattern.Match(type);
        outType = match.Groups[1].Value;
        outSize = match.Groups[2].Value;
        return match.Success;
    }

    public static bool IsDelegateType(string type, out string parameters)
    {
        var match = DelegatePattern.Match(type);
        parameters = match.Groups[1].Value;
        return match.Success;
    }

    public static bool IsPointer(string type, out string outType, out string outSuffix)
    {
        var match = PointerTypePattern.Match(type);
        outType = match.Groups[1].Value;
        outSuffix = match.Groups[2].Value.Replace('&', '*');
        return match.Success;
    }

    /// <summary>
    /// Tries to get the unmanaged type of a given type
    /// </summary>
    /// <param name="type">The type</param>
    /// <param name="unmanagedType">The unmanaged type</param>
    /// <returns>Whether an unmanaged type was found</returns>
    /// <remarks>The type must have been formatted first using <see cref="TypeConverter.ConvertType"/></remarks>
    public static bool TryGetUnmanagedType(string type, out UnmanagedType unmanagedType)
    {
        switch (type)
        {
            case "bool":
                unmanagedType = UnmanagedType.I1;
                return true;

            case "byte":
                unmanagedType = UnmanagedType.U1;
                return true;

            case "sbyte":
                unmanagedType = UnmanagedType.I1;
                return true;

            case "short":
                unmanagedType = UnmanagedType.I2;
                return true;

            case "ushort":
                unmanagedType = UnmanagedType.U2;
                return true;

            case "int":
                unmanagedType = UnmanagedType.I4;
                return true;

            case "uint":
                unmanagedType = UnmanagedType.U4;
                return true;

            case "long":
                unmanagedType = UnmanagedType.I8;
                return true;

            case "ulong":
                unmanagedType = UnmanagedType.U8;
                return true;

            case "float":
                unmanagedType = UnmanagedType.R4;
                return true;
            
            case "double":
                unmanagedType = UnmanagedType.R8;
                return true;

            case "string" or "char" or "char *":
                unmanagedType = UnmanagedType.LPUTF8Str;
                return true;

            default:
                unmanagedType = default;
                return false;
        }
    }
}