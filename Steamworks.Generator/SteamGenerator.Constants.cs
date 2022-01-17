using Steamworks.Generator.CodeGeneration;
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
            using (_writer.WriteClass("SteamConstants", "public static partial"))
            {
                foreach (var constant in _model.Constants)
                    _writer.WriteConstant(constant, false);
            }
        }

        return _writer.ToString();
    }
}