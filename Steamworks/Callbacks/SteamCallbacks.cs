namespace Steamworks.Callbacks;

public delegate void SteamCallback<T>(in T result, in bool failed);

internal delegate void SteamCallbackNative(IntPtr data, bool failed);