namespace Steamworks.Callbacks;

public readonly struct CallHandle<T>
{
    private readonly SteamAPICall_t _handle;

    private CallHandle(SteamAPICall_t handle) => _handle = handle;

    public static implicit operator CallHandle<T>(SteamAPICall_t handle) => new(handle);
    public static implicit operator SteamAPICall_t(CallHandle<T> handle) => handle._handle;
}