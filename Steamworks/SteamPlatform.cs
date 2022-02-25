#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    #define VALVE_LIBRARY_PACK_LARGE

    #if UNITY_64
        #define VALVE_LIBRARY_PLATFORM_WIN64
    #else
        #define VALVE_LIBRARY_PLATFORM_WIN32
    #endif
#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX) || (UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX)
    #define VALVE_LIBRARY_PACK_SMALL
    #define VALVE_LIBRARY_PLATFORM_POSIX
#endif

using System.Runtime.InteropServices;

namespace Steamworks
{
    internal static class SteamPlatform
    {
        public const CallingConvention CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl;

#if VALVE_LIBRARY_PLATFORM_WIN32
        public const string LibraryName = "steam_api";
#elif VALVE_LIBRARY_PLATFORM_WIN64
        public const string LibraryName = "steam_api64";
#elif VALVE_LIBRARY_PLATFORM_POSIX
        public const string LibraryName = "libsteam_api";
#else
        #error VALVE_LIBRARY_PLATFORM_WIN32, VALVE_LIBRARY_PLATFORM_WIN64 or VALVE_LIBRARY_PLATFORM_POSIX must be defined
        public const string LibraryName = "";
#endif

#if VALVE_LIBRARY_PACK_SMALL
        public const int LibraryPack = 4;
#elif VALVE_LIBRARY_PACK_LARGE
        public const int LibraryPack = 8;
#else
        #error VALVE_LIBRARY_PACK_SMALL or VALVE_LIBRARY_PACK_LARGE must be defined
        public const int LibraryPack = 0;
#endif
    }
}