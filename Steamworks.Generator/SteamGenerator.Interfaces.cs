using System.Runtime.InteropServices;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateInterfaces()
    {
        if (_model.Interfaces == null)
            return string.Empty;

        using (CodeWriterContext())
        {
            foreach (var interfaceModel in _model.Interfaces)
            {
                _writer.WriteStructLayoutAttribute(LayoutKind.Sequential);
                using (_writer.WriteBlock($"public unsafe ref struct {interfaceModel.Name}"))
                {
                    // pointer
                    _writer.Write("private readonly IntPtr _self;");
                    _writer.WriteLine();

                    if (interfaceModel.Fields != null)
                    {
                        foreach (var field in interfaceModel.Fields)
                            _writer.WriteField(field, _model.TypeDefs);
                        _writer.WriteLine();
                    }

                    if (interfaceModel.Methods != null)
                    {
                        foreach (var method in interfaceModel.Methods)
                        {
                            _writer.WriteMethodFacing(method, "_self");
                            _writer.WriteLine();
                        }
                    }

                    if (interfaceModel.Enums != null)
                    {
                        foreach (var @enum in interfaceModel.Enums)
                        {
                            _writer.WriteEnum(@enum);
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