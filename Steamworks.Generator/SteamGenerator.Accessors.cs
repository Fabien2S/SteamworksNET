namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateAccessors()
    {
        using (CodeWriterContext())
        {
            // SteamAPI
            GenerateFacing("SteamClientAPI", "global");
            GenerateFacing("SteamClientAPI", "user");

            // SteamGameServerAPI
            GenerateFacing("SteamServerAPI", "global");
            GenerateFacing("SteamServerAPI", "gameserver");
        }

        return _writer.ToString();
    }

    private void GenerateFacing(string name, string kind)
    {
        using (_writer.WriteBlock($"public static unsafe partial class {name}"))
        {
            if (_model.Interfaces is {Length: > 0})
            {
                foreach (var @interface in _model.Interfaces)
                {
                    if (@interface.Accessors == null)
                        continue;

                    foreach (var accessor in @interface.Accessors)
                    {
                        if (!kind.Equals(accessor.Kind, StringComparison.Ordinal))
                            continue;

                        // public static {Interface.Name} {Accessor.Name}() => SteamNative.{Accessor.FlatName}();
                        _writer.Write($"public static {@interface.Name} {accessor.Name}() => SteamNative.{accessor.FlatName}();");
                        _writer.WriteLine();
                    }
                }
            }
        }
    }
}