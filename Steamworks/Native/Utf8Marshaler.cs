using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Native;

public class Utf8Marshaler : ICustomMarshaler
{
    private static readonly ICustomMarshaler Instance = new Utf8Marshaler();

    private static readonly UTF8Encoding Encoding = new(false, false);

    private Utf8Marshaler()
    {
    }

    public unsafe IntPtr MarshalManagedToNative(object? managedObj)
    {
        if (managedObj == null)
            return IntPtr.Zero;

        if (managedObj is not string str)
            throw new MarshalDirectiveException(nameof(Utf8Marshaler) + " must be used on a string");

        var byteCount = Encoding.GetByteCount(str);

        var bytes = (byte*) Marshal.AllocHGlobal(byteCount + 1);
        fixed (char* c = str)
            Encoding.GetBytes(c, str.Length, bytes, byteCount);
        
        // set null character
        bytes[byteCount] = 0;

        return (IntPtr) bytes;
    }

    public unsafe object MarshalNativeToManaged(IntPtr pNativeData)
    {
        var walk = (byte*) pNativeData;
        while (*walk != 0)
            walk++;

        var length = (int) (walk - (byte*) pNativeData);
        return Encoding.GetString((byte*) pNativeData, length);
    }

    public void CleanUpNativeData(IntPtr pNativeData)
    {
        Marshal.FreeHGlobal(pNativeData);
    }

    public void CleanUpManagedData(object managedObj)
    {
    }

    public int GetNativeDataSize() => -1;

#if UNITY
    [UnityEngine.PreserveAttribute]
#endif
    public static ICustomMarshaler GetInstance(string cookie) => Instance;
}