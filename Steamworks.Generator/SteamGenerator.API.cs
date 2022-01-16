﻿using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Extensions;

namespace Steamworks.Generator;

public partial class SteamGenerator
{
    public string GenerateApi()
    {
        Prepare();
        
        using (_writer.WriteClass("SteamAPI", "public static unsafe partial"))
        {
            if (_model.NativeMethods is {Length: > 0})
            {
                foreach (var nativeMethod in _model.NativeMethods)
                {
                    _writer.WriteMethodFacing(nativeMethod, false);
                    _writer.WriteLine();
                }
            }

            if (_model.Interfaces is {Length: > 0})
            {
                foreach (var @interface in _model.Interfaces)
                {
                    if (@interface.Accessors == null)
                        continue;

                    foreach (var accessor in @interface.Accessors)
                    {
                        // public static {InterfaceName} {AccessorName}() => SteamNative.{AccessorFlatName}();
                        using (_writer.AppendContext())
                        {
                            _writer
                                .Write("public static ")
                                .Write(@interface.Name).Write(' ')
                                .Write(accessor.Name).Write("() => ")
                                .Write("SteamNative.").Write(accessor.FlatName).Write("();");
                        }
                        _writer.WriteLine();
                    }
                }
            }
        }

        return _writer.ToString();
    }
}