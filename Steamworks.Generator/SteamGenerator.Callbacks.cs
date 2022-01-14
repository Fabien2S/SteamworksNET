using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateCallbackStructs()
    {
        if (_model.CallbackStructs == null)
            return string.Empty;

        Prepare();

        foreach (var callbackStruct in _model.CallbackStructs)
        {
            _writer.WriteStructLayoutAttribute(LayoutKind.Sequential, _pack);
            using (_writer.WriteStruct(callbackStruct.Name, "public unsafe"))
            {
                if (callbackStruct.Constants != null)
                {
                    foreach (var constant in callbackStruct.Constants)
                        _writer.WriteConstant(constant);
                }

                if (callbackStruct.Fields != null)
                {
                    foreach (var field in callbackStruct.Fields)
                    {
                        _writer.WriteField(field, _model.TypeDefs);
                    }
                }

                if (callbackStruct.Enums != null)
                {
                    foreach (var enumModel in callbackStruct.Enums)
                    {
                        _writer.WriteEnum(enumModel);
                        _writer.WriteLine();
                    }
                }
            }

            _writer.WriteLine();
        }

        return _writer.ToString();
    }
}