using System.Runtime.InteropServices;

namespace Steamworks.Callbacks;

public delegate void CallbackHandler<T>(in T result);

public readonly struct Callback<T> where T : struct, ICallbackResult
{
    private readonly CallbackHandler<T> _callback;

    private Callback(CallbackHandler<T> callback)
    {
        _callback = callback;

        // TODO Use static interface when released
        var id = default(T).Id;
        SteamDispatcher.RegisterCallback(id, Call);
    }

    private void Call(in IntPtr data)
    {
        var result = Marshal.PtrToStructure<T>(data);
        _callback(result);
    }

    public static Callback<T> Create(CallbackHandler<T> callback) => new(callback);
}