using System.Runtime.CompilerServices;

namespace Steamworks.Callbacks
{
    public delegate void CallbackHandler<T>(in T result);

    public readonly struct Callback<T> : IDisposable where T : struct, ICallbackResult
    {
        // TODO Rewrite with static interfaces
        private static readonly int Id = default(T).Id;

        private readonly CallbackHandler<T> _callback;
        private readonly bool _isGameServer;

        private Callback(CallbackHandler<T> callback, bool isGameServer)
        {
            _callback = callback;
            _isGameServer = isGameServer;

            SteamDispatcher.RegisterCallback(Id, _isGameServer, _Handler);
        }

        private unsafe void _Handler(in IntPtr resultPtr)
        {
            var result = Unsafe.Read<T>((void*) resultPtr);
            _callback(result);
        }

        public void Dispose()
        {
            SteamDispatcher.UnregisterCallback(Id, _isGameServer, _Handler);
        }

        public static Callback<T> Create(CallbackHandler<T> callback) => new(callback, false);
        public static Callback<T> CreateGameServer(CallbackHandler<T> callback) => new(callback, true);
    }
}