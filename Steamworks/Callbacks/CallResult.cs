using System.Runtime.InteropServices;

namespace Steamworks.Callbacks;

public delegate void CallResultHandler<T>(in T result, in bool failed);

public struct CallResult<T> : IDisposable where T : struct
{
    private SteamAPICall_t _handle;
    private CallResultHandler<T> _callback;

    private void _Handler(in IntPtr resultPtr, in bool failed)
    {
        _handle = SteamConstants.k_uAPICallInvalid;
        var result = Marshal.PtrToStructure<T>(resultPtr);
        _callback(result, failed);
    }

    public void Set(CallHandle<T> handle, CallResultHandler<T> callback)
    {
        if (_handle != default)
            SteamDispatcher.UnregisterCallResult(_handle);

        _handle = handle;
        _callback = callback;

        if (_handle != default)
            SteamDispatcher.RegisterCallResult(handle, _Handler);
    }

    public void Dispose()
    {
        if (_handle != default)
            SteamDispatcher.UnregisterCallResult(_handle);
    }

    public static CallResult<T> Create() => default;
}