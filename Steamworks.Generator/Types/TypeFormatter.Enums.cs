using Steamworks.Generator.Models;

namespace Steamworks.Generator.Types;

public static partial class TypeFormatter
{
    public static void FormatEnum(ref EnumModel @enum)
    {
        const string commonPrefix = "k_";
        var enumNamePrefix = commonPrefix + @enum.Name;
        var enumEntryPrefix = FindCommonPrefix(ref @enum);

        var valueList = new List<ValueModel>();
        foreach (var entry in @enum.Values)
        {
            var entryName = entry.Name;

            if (entryName.Contains("Force32Bit", StringComparison.Ordinal))
                continue;

            if (entryName.StartsWith(enumNamePrefix, StringComparison.OrdinalIgnoreCase))
                entryName = entryName[enumNamePrefix.Length..];
            else if (entryName.StartsWith(enumEntryPrefix, StringComparison.OrdinalIgnoreCase))
                entryName = entryName[enumEntryPrefix.Length..];
            else if (entryName.StartsWith(commonPrefix, StringComparison.OrdinalIgnoreCase))
                entryName = entryName[commonPrefix.Length..];

            if (entryName.StartsWith('_'))
                entryName = entryName[1..];

            if (entryName.Length > 0)
            {
                var firstChar = entryName[0];
                if (!char.IsLetter(firstChar) && firstChar != '_')
                    entryName = '_' + entryName;
            }

            valueList.Add(entry with {Name = entryName});
        }

        @enum.Values = valueList.ToArray();
    }

    private static string FindCommonPrefix(ref EnumModel @enum)
    {
        var prefix = ReadOnlySpan<char>.Empty;
        foreach (var value in @enum.Values)
        {
            var valueName = value.Name;

            if (prefix.IsEmpty)
            {
                prefix = valueName;
                continue;
            }

            var min = Math.Min(valueName.Length, prefix.Length);
            for (var i = 0; i < min; i++)
            {
                if (prefix[i] == valueName[i])
                    continue;

                prefix = prefix[..i];
                break;
            }
        }

        return prefix.ToString();
    }
}