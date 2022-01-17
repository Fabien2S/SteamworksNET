using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateEnums()
    {
        if (_model.Enums == null)
            return string.Empty;

        using (CodeWriterContext())
        {
            foreach (var enumModel in _model.Enums)
            {
                _writer.WriteEnum(enumModel);
                _writer.WriteLine();
            }   
        }

        return _writer.ToString();
    }
}