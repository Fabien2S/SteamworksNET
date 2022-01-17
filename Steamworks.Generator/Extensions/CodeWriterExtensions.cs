using Steamworks.Generator.CodeGeneration;
using Steamworks.Generator.Models;
using Steamworks.Generator.Types;

namespace Steamworks.Generator.Extensions;

public static class CodeWriterExtensions
{
    public static void WriteConstant(this CodeWriter writer, ConstantModel constant, bool useConst)
    {
        TypeFormatter.FormatConstant(ref constant);

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
        TypeFormatter.FormatEnum(ref @enum);

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
        if (!TypePredicate.ShouldIncludeField(in field))
            return;

        TypeFormatter.FormatField(ref field);

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
        TypeFormatter.FormatMethodNative(ref method);

        writer.WriteDllImportAttribute(method.FlatName);

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
        TypeFormatter.FormatMethodFacing(ref method);

        writer.Write("/// <summary>");
        using (writer.AppendContext())
            writer.Write("/// ").Write(method.FlatName);
        writer.Write("/// </summary>");

        if (method.Parameters != null)
        {
            foreach (var parameter in method.Parameters)
            {
                using (writer.AppendContext())
                {
                    writer
                        .Write("/// <param name=\"").Write(parameter.Name).Write("\">")
                        .Write(parameter.Description)
                        .Write("</param>");
                }
            }
        }

        if (!string.IsNullOrEmpty(method.CallResult))
        {
            using (writer.AppendContext())
                writer.Write("/// <seealso cref=\"").Write(method.CallResult).Write("\"/>");
        }

        if (!string.IsNullOrEmpty(method.Callback))
        {
            using (writer.AppendContext())
                writer.Write("/// <seealso cref=\"").Write(method.Callback).Write("\"/>");
        }

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