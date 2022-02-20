using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateConstants()
    {
        if (_model.Constants == null)
            return string.Empty;

        using (CodeWriterContext())
        {
            using (_writer.WriteBlock("public static partial class SteamConstants"))
            {
                foreach (var constant in _model.Constants)
                    _writer.WriteConstant(constant);
            }
        }

        return _writer.ToString();
    }
}