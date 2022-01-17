using Steamworks.Callbacks;

namespace Steamworks
{
    public static partial class SteamClientAPI
    {
        /// <summary>
        /// Initializes the Steamworks API.
        /// </summary>
        /// <returns>Whether all required interfaces have been acquired and are accessible.</returns>
        /// <seealso href="https://partner.steamgames.com/doc/api/steam_api#SteamAPI_Init"/>
        public static bool SteamAPI_Init() => SteamNative.SteamAPI_Init();

        /// <summary>
        /// SteamAPI_Shutdown
        /// </summary>
        /// <seealso href="https://partner.steamgames.com/doc/api/steam_api#SteamAPI_Shutdown"/>
        public static void SteamAPI_Shutdown()
        {
            SteamNative.SteamAPI_Shutdown();
            SteamDispatcher.ClearUserCallbacks();
            // TODO What about call results?
        }

        /// <summary>
        /// Checks if your executable was launched through Steam and relaunches it through Steam if it wasn't.
        /// </summary>
        /// <param name="unOwnAppID">The App ID of this title.</param>
        /// <returns>
        /// If this returns true then it starts the Steam client if required and launches your game again through it, and you should quit your process as soon as possible. This effectively runs steam://run/<see cref="unOwnAppID"/> so it may not relaunch the exact executable that called it, as it will always relaunch from the version installed in your Steam library folder.
        /// If it returns false, then your game was launched by the Steam client and no action needs to be taken. One exception is if a steam_appid.txt file is present then this will return false regardless. This allows you to develop and test without launching your game through the Steam client. Make sure to remove the steam_appid.txt file when uploading the game to your Steam depot
        /// </returns>
        /// <seealso href="https://partner.steamgames.com/doc/api/steam_api#SteamAPI_RestartAppIfNecessary"/>
        public static bool SteamAPI_RestartAppIfNecessary(AppId_t unOwnAppID) => SteamNative.SteamAPI_RestartAppIfNecessary(unOwnAppID);

        /// <summary>
        /// Dispatches callbacks and call results to all of the registered listeners.
        /// </summary>
        public static void SteamAPI_RunCallbacks() => SteamDispatcher.RunUserCallbacks();
    }
}