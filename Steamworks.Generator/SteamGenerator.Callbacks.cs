using System.Globalization;
using System.Runtime.InteropServices;
using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;
using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

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
        var callbackStructs = _model.CallbackStructs;
        if (callbackStructs == null)
            return string.Empty;

        Prepare();

        // There are known duplicated callbacks, so we use a HashSet to prevent
        // duplication in switch cases
        var distinctCallbackIDs = new HashSet<int>();

        // SteamDispatcher handlers
        // using (_writer.WriteClass("SteamDispatcher", "public static partial"))
        // {
        //     _writer.Write(
        //         "static unsafe partial void HandleCallResult(in CallbackMsg_t* pCallback, in SteamAPICall_t callHandle, in void* dataPtr, in bool bFailed)");
        //     using (_writer.BlockContext())
        //     {
        //         _writer.Write("if (!CallResultHandlers.TryGetValue(callHandle, out var listenerPtr)) return;");
        //         _writer.Write("switch (pCallback->m_iCallback)");
        //         using (_writer.BlockContext())
        //         {
        //             distinctCallbackIDs.Clear();
        //             foreach (var callbackStruct in callbackStructs)
        //             {
        //                 if (!TypePredicate.ShouldIncludeCallbackStruct(in callbackStruct))
        //                     continue;
        //                 if (!distinctCallbackIDs.Add(callbackStruct.CallbackId))
        //                     continue;
        //
        //                 var callbackId = callbackStruct.CallbackId.ToString(NumberFormatInfo.InvariantInfo);
        //                 using (_writer.AppendContext())
        //                     _writer.Write("case ").Write(callbackId).Write(':');
        //
        //                 using (_writer.BlockContext())
        //                 {
        //                     using (_writer.AppendContext())
        //                     {
        //                         _writer
        //                             .Write("var data = Marshal.PtrToStructure<")
        //                             .Write(callbackStruct.Name)
        //                             .Write(">((IntPtr) dataPtr);");
        //                     }
        //
        //                     using (_writer.AppendContext())
        //                     {
        //                         _writer
        //                             .Write("var listener = Marshal.GetDelegateForFunctionPointer<SteamCallback<")
        //                             .Write(callbackStruct.Name)
        //                             .Write(">>(listenerPtr);");
        //                     }
        //
        //                     _writer.Write("listener(in data, in bFailed);");
        //                     _writer.Write("break;");
        //                 }
        //             }
        //         }
        //     }
        //
        //     _writer.WriteLine();
        //
        //     _writer.Write("static unsafe partial void HandleCallback(in CallbackMsg_t* pCallback)");
        //     using (_writer.BlockContext())
        //     {
        //         _writer.Write("// Do something with the call result");
        //     }
        //
        //     _writer.WriteLine();
        // }

        _writer.WriteLine();

        foreach (var callbackStruct in callbackStructs)
        {
            if (!TypePredicate.ShouldIncludeCallbackStruct(in callbackStruct))
                continue;

            _writer.WriteStructLayoutAttribute(LayoutKind.Sequential, _dllPack);
            using (_writer.WriteStruct(callbackStruct.Name, "public unsafe"))
            {
                _writer.WriteConstant(new ConstantModel
                {
                    Name = "k_iCallback",
                    Type = "int",
                    Value = callbackStruct.CallbackId.ToString(NumberFormatInfo.InvariantInfo)
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
}