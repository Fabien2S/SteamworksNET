using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateInterfaces()
    {
        if (_model.Interfaces == null)
            return string.Empty;

        Prepare();

        foreach (var interfaceModel in _model.Interfaces)
        {
            _writer.WriteStructLayoutAttribute(LayoutKind.Sequential, _dllPack);
            using (_writer.WriteStruct(interfaceModel.Name, "public readonly unsafe ref"))
            {
                // pointer
                _writer.Write("private readonly IntPtr _self;");
                _writer.WriteLine();

                // TODO Does interface even have fields?
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
                        _writer.WriteMethodFacing(method, true);
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

        return _writer.ToString();
    }
}