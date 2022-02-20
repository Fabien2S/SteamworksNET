using System.Runtime.InteropServices;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateStructs()
    {
        if (_model.Structs == null)
            return string.Empty;

        using (CodeWriterContext())
        {
            foreach (var structModel in _model.Structs)
            {
                _writer.WriteStructLayoutAttribute(LayoutKind.Sequential);
                using (_writer.WriteBlock($"public unsafe struct {structModel.Name}"))
                {
                    if (structModel.Constants != null)
                    {
                        foreach (var constant in structModel.Constants)
                            _writer.WriteConstant(constant);
                        _writer.WriteLine();
                    }

                    if (structModel.Fields != null)
                    {
                        foreach (var field in structModel.Fields)
                            _writer.WriteField(field, _model.TypeDefs);
                        _writer.WriteLine();
                    }

                    if (structModel.Methods != null)
                    {
                        foreach (var method in structModel.Methods)
                        {
                            _writer.WriteMethodFacing(method, "ref this");
                            _writer.WriteLine();
                        }
                    }
                }

                _writer.WriteLine();
            }
        }

        return _writer.ToString();
    }
}