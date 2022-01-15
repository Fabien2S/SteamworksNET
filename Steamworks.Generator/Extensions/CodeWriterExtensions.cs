using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

namespace Steamworks.Generator.Extensions;

public static class CodeWriterExtensions
{
    public static void WriteConstant(this CodeWriter writer, ConstantModel constant, bool useConst)
    {
        if (!TypeFormatter.TryFormatConstant(ref constant))
            return;

        using (writer.AppendContext())
        {
            writer
                .Write(useConst ? "public const" : "public static readonly")
                .Write(' ')
                .Write(constant.Type)
                .Write(' ')
                .Write(constant.Name)
                .Write(" = ")
                .Write(constant.Value)
                .Write(';');
        }
    }

    public static void WriteEnum(this CodeWriter writer, EnumModel @enum)
    {
        if (!TypeFormatter.TryFormatEnum(ref @enum))
            return;

        writer.Write("public enum " + @enum.Name);
        using (writer.BlockContext())
        {
            foreach (var enumValue in @enum.Values)
            {
                using (writer.AppendContext())
                    writer.Write(enumValue.Name).Write(" = ").Write(enumValue.Value).Write(",");
            }
        }
    }

    public static void WriteField(this CodeWriter writer, FieldModel field, TypeDefModel[]? typeDefs)
    {
        if (!TypeFormatter.TryFormatField(ref field))
            return;

        if (field.CustomAttribute != null)
            writer.Write(field.CustomAttribute);

        using (writer.AppendContext())
        {
            writer.Write(field.IsPrivate ? "private" : "public").Write(' ');

            if (SteamConverter.IsFixedSizeArrayType(field.Type, out var fixedType, out var fixedSize))
            {
                writer.Write("fixed").Write(' ');

                var formatType = SteamFormatter.FormatFixedSizeType(fixedType, typeDefs);
                writer.Write(formatType).Write(' ');
                writer.Write(field.Name).Write('[').Write(fixedSize).Write(']');
                writer.Write(';');
            }
            else
            {
                writer.Write(field.Type).Write(' ');
                writer.Write(field.Name);
                writer.Write(';');
            }
        }
    }

    public static void WriteMethodNative(this CodeWriter writer, string dllName, MethodModel method, bool hasSelfPtr)
    {
        if (!TypeFormatter.TryFormatMethodNative(ref method))
            return;

        writer.WriteDllImportAttribute(dllName, method.FlatName);

        // TODO Rework TryGetUnmanagedType
        if (SteamConverter.TryGetUnmanagedType(method.ReturnType, out var unmanagedType))
            writer.WriteMarshalAsAttribute(unmanagedType, "return");

        using (writer.AppendContext())
        {
            writer
                .Write("public static extern")
                .Write(' ')
                .Write(method.ReturnType)
                .Write(' ')
                .Write(method.FlatName)
                .Write('(');

            if (hasSelfPtr)
                writer.Write("IntPtr self");

            var parameters = method.Parameters;
            if (parameters is {Length: > 0})
            {
                if (hasSelfPtr) writer.Write(", ");
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    // TODO Rework TryGetUnmanagedType
                    if (SteamConverter.TryGetUnmanagedType(parameter.Type, out var parameterUnmanagedType))
                        writer.WriteMarshalAsAttribute(parameterUnmanagedType);

                    // {ParamType} {ParamName}
                    writer.Write(parameter.Type).Write(' ').Write(parameter.Name);

                    if (i != parameters.Length - 1)
                        writer.Write(", ");
                }
            }

            writer.Write(");");
        }
    }

    public static void WriteMethodFacing(this CodeWriter writer, MethodModel method, bool hasSelfPtr)
    {
        if (!TypeFormatter.TryFormatMethodFacing(ref method))
            return;

        using (writer.AppendContext())
        {
            // Method definition

            // public {ReturnType} {Name}(
            writer
                .Write(hasSelfPtr ? "public" : "public static").Write(' ')
                .Write(method.ReturnType).Write(' ')
                .Write(method.Name).Write('(');

            // {Params}
            var parameters = method.Parameters;
            if (parameters is {Length: > 0})
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    writer
                        .Write(parameter.Type)
                        .Write(' ')
                        .Write(parameter.Name);

                    if (i != parameters.Length - 1)
                        writer.Write(", ");
                }
            }

            // ) => 
            writer.Write(") => ");

            // Method implementation

            // SteamNative.{MethodFlatName}(
            writer.Write("SteamNative.").Write(method.FlatName).Write('(');

            if (hasSelfPtr)
                writer.Write("_self");

            // {Params}
            if (parameters is {Length: > 0})
            {
                if (hasSelfPtr) writer.Write(", ");
                for (var i = 0; i < parameters.Length; i++)
                {
                    writer.Write(parameters[i].Name);
                    if (i != parameters.Length - 1)
                        writer.Write(", ");
                }
            }

            // );
            writer.Write(");");
        }
    }
}