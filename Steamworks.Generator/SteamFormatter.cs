using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

namespace Steamworks.Generator;

/// <summary>
/// This class is responsible for formatting string to compilable C#.
/// Every hard-coded fixes are here
/// </summary>
public static partial class SteamFormatter
{
    /// <summary>
    /// Format type for fixed size array
    /// </summary>
    public static string FormatFixedSizeType(string type, TypeDefModel[]? typeDefs)
    {
        if (typeDefs != null)
        {
            foreach (var typeDef in typeDefs)
            {
                if (!typeDef.Name.Equals(type, StringComparison.Ordinal))
                    continue;

                return FormatFixedSizeType(typeDef.Type, typeDefs);
            }
        }

        return type switch
        {
            "CGameID" => "ulong",
            "CSteamID" => "ulong",
            _ => TypeConverter.ConvertType(type)
        };
    }
}