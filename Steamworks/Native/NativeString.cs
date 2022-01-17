using System.Runtime.InteropServices;

namespace Steamworks.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly ref struct NativeString
    {
        private readonly IntPtr _handle;

        public override string ToString()
        {
            return Marshal.PtrToStringUTF8(_handle)!;
        }

        public static implicit operator string(NativeString ptr)
        {
            return Marshal.PtrToStringUTF8(ptr._handle)!;
        }

        public static explicit operator IntPtr(NativeString ptr)
        {
            return ptr._handle;
        }
    }
}