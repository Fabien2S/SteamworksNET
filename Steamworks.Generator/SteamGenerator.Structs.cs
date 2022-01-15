using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateStructs()
    {
        if (_model.Structs == null)
            return string.Empty;

        Prepare();

        foreach (var structModel in _model.Structs)
        {
            _writer.WriteStructLayoutAttribute(LayoutKind.Sequential, _dllPack);
            using (_writer.WriteStruct(structModel.Name, "public unsafe"))
            {
                if (structModel.Constants != null)
                {
                    foreach (var constant in structModel.Constants)
                    {
                        _writer.WriteConstant(constant, false);
                    }

                    _writer.WriteLine();
                }

                if (structModel.Fields != null)
                {
                    foreach (var field in structModel.Fields)
                    {
                        _writer.WriteField(field, _model.TypeDefs);
                    }

                    _writer.WriteLine();
                }

                // TODO What about that?
                // if (structModel.Methods != null)
                // {
                //     foreach (var method in structModel.Methods)
                //     {
                //         _writer.WriteMethodNative(_dllName, method);
                //         _writer.WriteLine();
                //     }
                // }
            }

            _writer.WriteLine();
        }

        return _writer.ToString();
    }
}