using System.Globalization;
using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;
using Steamworks.Generator.Models;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public static int GetCallbackId<T>(in T callback)
    {
        return callback switch
        {
            int => 0,
            bool => -1,
            _ => 0
        };
    }

    public string GenerateCallbackStructs()
    {
        if (_model.CallbackStructs == null)
            return string.Empty;

        Prepare();

        using (_writer.WriteClass("SteamCallbacks", "public static"))
        {
            _writer.WriteConstant(new ConstantModel
            {
                Name = "Count",
                Type = "int",
                Value = _model.CallbackStructs.Length.ToString(NumberFormatInfo.InvariantInfo)
            }, true);
            _writer.WriteLine();

            _writer.Write("public enum Type");
            using (_writer.BlockContext())
            {
                foreach (var callbackStruct in _model.CallbackStructs)
                {
                    using (_writer.AppendContext())
                    {
                        _writer
                            .Write(callbackStruct.Name)
                            .Write(" = ")
                            .Write(callbackStruct.CallbackId.ToString(NumberFormatInfo.InvariantInfo))
                            .Write(',');
                    }
                }
            }
        }
        _writer.WriteLine();
        
        // SteamDispatcher handlers
        using (_writer.WriteClass("SteamDispatcher", "public static partial"))
        {
            _writer.Write("static unsafe partial void HandleCallback(in CallbackMsg_t* pCallback)");
            using (_writer.BlockContext())
            {
                _writer.Write("// Do something with the call result");
            }
            _writer.WriteLine();

            _writer.Write("static unsafe partial void HandleCallResult(in CallbackMsg_t* pCallback, in bool bFailed)");
            using (_writer.BlockContext())
            {
                _writer.Write("// Do something with the call result");
            }
            _writer.WriteLine();
        }
        _writer.WriteLine();
        
        foreach (var callbackStruct in _model.CallbackStructs)
        {
            _writer.WriteStructLayoutAttribute(LayoutKind.Sequential, _dllPack);
            using (_writer.WriteStruct(callbackStruct.Name, "public unsafe"))
            {
                _writer.WriteConstant(new ConstantModel
                {
                    Name = "k_iCallback",
                    Type = "SteamCallbacks.Type",
                    Value = "SteamCallbacks.Type." + callbackStruct.Name
                }, true);
                _writer.WriteLine();

                if (callbackStruct.Constants != null)
                {
                    foreach (var constant in callbackStruct.Constants)
                        _writer.WriteConstant(constant, false);
                    _writer.WriteLine();
                }

                if (callbackStruct.Fields != null)
                {
                    foreach (var field in callbackStruct.Fields)
                        _writer.WriteField(field, _model.TypeDefs);
                    _writer.WriteLine();
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

    // private void GenerateCallbackTypeMethod(in CallbackStructModel[] callbackStructs)
    // {
    //     _writer.Write("public static Type GetCallbackType<T>(in T callback) where T : struct");
    //     using (_writer.BlockContext())
    //     {
    //         _writer.Write("return callback switch");
    //         using (_writer.BlockContext())
    //         {
    //             foreach (var callbackStruct in callbackStructs)
    //             {
    //                 using (_writer.AppendContext())
    //                 {
    //                     _writer
    //                         .Write(callbackStruct.Name).Write(" => ")
    //                         .Write("Type.").Write(callbackStruct.Name).Write(',');
    //                 }
    //             }
    //
    //             _writer.Write("_ => (Type) (-1)");
    //         }
    //
    //         _writer.Write(';');
    //     }
    //
    //     _writer.WriteLine();
    // }
    //
    // private void GenerateCallbackIndexMethod(in CallbackStructModel[] callbackStructs)
    // {
    //     _writer.Write("public static int GetCallbackIndex(in Type callbackType)");
    //     using (_writer.BlockContext())
    //     {
    //         _writer.Write("return callbackType switch");
    //         using (_writer.BlockContext())
    //         {
    //             for (var i = 0; i < callbackStructs.Length; i++)
    //             {
    //                 var callbackStruct = callbackStructs[i];
    //                 using (_writer.AppendContext())
    //                 {
    //                     _writer
    //                         .Write("Type.").Write(callbackStruct.Name).Write(" => ")
    //                         .Write(i.ToString(NumberFormatInfo.InvariantInfo)).Write(',');
    //                 }
    //             }
    //
    //             _writer.Write("_ => (Type) -1");
    //         }
    //
    //         _writer.Write(';');
    //     }
    //
    //     _writer.WriteLine();
    // }
    //
    // private void GenerateCallbackTypeEnum(CallbackStructModel[] callbackStructs)
    // {
    //     _writer.Write("public enum Type");
    //     using (_writer.BlockContext())
    //     {
    //         foreach (var callbackStruct in callbackStructs)
    //         {
    //             using (_writer.AppendContext())
    //             {
    //                 _writer
    //                     .Write(callbackStruct.Name)
    //                     .Write(" = ")
    //                     .Write(callbackStruct.CallbackId.ToString(NumberFormatInfo.InvariantInfo))
    //                     .Write(',');
    //             }
    //         }
    //     }
    //
    //     _writer.WriteLine();
    // }
}