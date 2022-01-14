using System.Runtime.InteropServices;

namespace Steamworks.Native;

[StructLayout(LayoutKind.Sequential)]
public readonly ref struct Utf8String
{
    private readonly IntPtr _handle;

    public override string? ToString()
    {
        return Marshal.PtrToStringUTF8(_handle);
    }

    public static implicit operator string?(Utf8String ptr)
    {
        return Marshal.PtrToStringUTF8(ptr._handle);
    }

    public static explicit operator IntPtr(Utf8String ptr)
    {
        return ptr._handle;
    }
}