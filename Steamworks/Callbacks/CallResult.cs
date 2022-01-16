using System.Runtime.InteropServices;

namespace Steamworks.Callbacks;

public delegate void CallResultHandler<T>(in T result, in bool failed);

public struct CallResult<T> where T : struct
{
    public void Set(CallHandle<T> handle, CallResultHandler<T> callback)
    {
        SteamDispatcher.RegisterCallResult(handle, (in IntPtr resultPtr, in bool failed) =>
        {
            var result = Marshal.PtrToStructure<T>(resultPtr);
            callback(result, failed);
        });
    }
}