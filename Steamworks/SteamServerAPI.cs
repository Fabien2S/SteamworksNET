using Steamworks.Callbacks;

namespace Steamworks
{
    public static partial class SteamServerAPI
    {
        /// <summary>
        /// Initializes the ISteamGameServer interface object, and set server properties which may not be changed.<br/>
        /// After calling this function, you should set any additional server parameters, and then call ISteamGameServer::LogOnAnonymous or ISteamGameServer::LogOn.<br/>
        /// If you pass in MASTERSERVERUPDATERPORT_USEGAMESOCKETSHARE into usQueryPort, then the game server will use GameSocketShare mode, which means that the game is responsible for sending and receiving UDP packets for the master server updater.
        /// </summary>
        /// <param name="unIP">The IP address you are going to bind to. (This should be in host order, i.e 127.0.0.1 == 0x7f000001). You can use INADDR_ANY to bind to all local IPv4 addresses.</param>
        /// <param name="usSteamPort">The local port used to communicate with the steam servers.</param>
        /// <param name="usGamePort">The port that clients will connect to for gameplay.</param>
        /// <param name="usQueryPort">The port that will manage server browser related duties and info pings from clients.</param>
        /// <param name="eServerMode">Sets the authentication method of the server.</param>
        /// <param name="pchVersionString">The version string is usually in the form x.x.x.x, and is used by the master server to detect when the server is out of date. (Only servers with the latest version will be listed.)</param>
        public static bool SteamGameServer_Init(uint unIP, ushort usSteamPort, ushort usGamePort, ushort usQueryPort, EServerMode eServerMode, string pchVersionString) => SteamNative.SteamGameServer_Init(unIP, usSteamPort, usGamePort, usQueryPort, eServerMode, pchVersionString);

        /// <summary>
        /// Shuts down the Steamworks GameServer API, releases pointers and frees memory.
        /// </summary>
        public static void SteamGameServer_Shutdown()
        {
            SteamNative.SteamGameServer_Shutdown();
            SteamDispatcher.ClearServerCallbacks();
            // TODO What about call results?
        }

        /// <summary>
        /// Dispatches callbacks created with STEAM_GAMESERVER_CALLBACK and call results to all of the registered listeners.
        /// </summary>
        public static void SteamGameServer_RunCallbacks() => SteamDispatcher.RunGameServerCallbacks();
    }
}