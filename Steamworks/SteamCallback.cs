namespace Steamworks;

public delegate void SteamCallback<in T>(T result);

public readonly struct SteamCallbackHandle<T> : IDisposable
{
    private readonly SteamCallback<T> _callback;

    public SteamCallbackHandle(SteamCallback<T> callback)
    {
        _callback = callback;
    }

    public void Dispose()
    {
    }
}