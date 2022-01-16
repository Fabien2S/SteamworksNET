using System.Runtime.InteropServices;

namespace Steamworks.Callbacks;

public delegate void CallbackHandler<T>(in T result);

public readonly struct Callback<T> : IDisposable where T : struct, ICallbackResult
{
    // TODO Rewrite with static interfaces
    private static readonly int Id = default(T).Id;

    private readonly CallbackHandler<T> _callback;

    private Callback(CallbackHandler<T> callback)
    {
        _callback = callback;

        SteamDispatcher.RegisterCallback(Id, _Handler);
    }

    private void _Handler(in IntPtr data)
    {
        var result = Marshal.PtrToStructure<T>(data);
        _callback(result);
    }

    public void Dispose()
    {
        SteamDispatcher.UnregisterCallback(Id, _Handler);
    }

    public static Callback<T> Create(CallbackHandler<T> callback) => new(callback);
}

[Flags]
public enum CallbackFlags
{
    Registered = 1 << 0,
    IsGameServer = 1 << 1
}