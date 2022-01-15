using System.Runtime.InteropServices;

namespace Steamworks.Callbacks;

public struct CallResult<T> where T : struct
{
    public void Set(CallHandle<T> handle, SteamCallback<T> callback)
    {
        SteamDispatcher.RegisterCallResult(handle, (dataPtr, failed) =>
        {
            var data = Marshal.PtrToStructure<T>(dataPtr);
            callback(data, failed);
        });
    }
}