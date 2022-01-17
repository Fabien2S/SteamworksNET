using System.Runtime.InteropServices;

namespace Steamworks;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    #define VALVE_LIBRARY_PACK_LARGE
#if UNITY_64
    #define VALVE_LIBRARY_NAME_STEAM_API64
#else
    #define VALVE_LIBRARY_NAME_STEAM_API
#endif
#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) || (UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX)
    #define VALVE_LIBRARY_NAME_LIBSTEAM_API
    #define VALVE_LIBRARY_PACK_SMALL
#endif

internal static class SteamPlatform
{
    public const CallingConvention CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl;

#if VALVE_LIBRARY_NAME_STEAM_API64
    public const string LibraryName = "steam_api64";
#elif VALVE_LIBRARY_NAME_STEAM_API
    public const string LibraryName = "steam_api";
#elif VALVE_LIBRARY_NAME_LIBSTEAM_API
    public const string LibraryName = "steam_api";
#else
    public const string LibraryName = "Failed to determine platform library name";
#endif

#if VALVE_LIBRARY_PACK_SMALL
    public const int LibraryPack = 4;
#elif VALVE_LIBRARY_PACK_LARGE
    public const int LibraryPack = 8;
#else
    public const int LibraryPack = "Failed to determine platform pack size";
#endif
}