using Steamworks.Generator.CodeGeneration;

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
        using (_writer.WriteClass(name, "public static unsafe partial"))
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

                        // public static {InterfaceName} {AccessorName}() => SteamNative.{AccessorFlatName}();
                        using (_writer.AppendContext())
                        {
                            _writer
                                .Write("public static ")
                                .Write(@interface.Name).Write(' ')
                                .Write(accessor.Name).Write("() => ")
                                .Write("SteamNative.").Write(accessor.FlatName).Write("();");
                        }

                        _writer.WriteLine();
                    }
                }
            }
        }
    }
}