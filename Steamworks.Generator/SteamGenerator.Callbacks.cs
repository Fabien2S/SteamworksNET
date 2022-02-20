using System.Globalization;
using System.Runtime.InteropServices;
using Steamworks.Generator.Extensions;
using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateCallbackStructs()
    {
        var callbackStructs = _model.CallbackStructs;
        if (callbackStructs == null)
            return string.Empty;

        using (CodeWriterContext())
        {
            foreach (var callbackStruct in callbackStructs)
            {
                if (!TypePredicate.ShouldIncludeCallbackStruct(in callbackStruct))
                    continue;

                _writer.WriteStructLayoutAttribute(LayoutKind.Sequential);
                using (_writer.WriteBlock($"public unsafe struct {callbackStruct.Name} : ICallbackResult"))
                {
                    _writer.WriteConstant(new ConstantModel
                    {
                        Name = "k_iCallback",
                        Type = "int",
                        Value = callbackStruct.CallbackId.ToString(NumberFormatInfo.InvariantInfo)
                    });
                    _writer.WriteLine();

                    if (callbackStruct.Constants is {Length: > 0})
                    {
                        foreach (var constant in callbackStruct.Constants)
                            _writer.WriteConstant(constant);
                        _writer.WriteLine();
                    }

                    _writer.Write("public int Id => k_iCallback;");
                    _writer.WriteLine();

                    if (callbackStruct.Fields is {Length: > 0})
                    {
                        foreach (var field in callbackStruct.Fields)
                            _writer.WriteField(field, _model.TypeDefs);
                        _writer.WriteLine();
                    }

                    if (callbackStruct.Enums is {Length: > 0})
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
        }

        return _writer.ToString();
    }
}