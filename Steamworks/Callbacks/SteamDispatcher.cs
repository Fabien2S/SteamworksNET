using System.Runtime.InteropServices;
using static Steamworks.SteamAPI;

namespace Steamworks.Callbacks;

public static class SteamDispatcher
{
    private static readonly Dictionary<SteamAPICall_t, SteamCallbackNative> ResultCallbacks = new();
    
    static SteamDispatcher()
    {
        // Initialize dispatch
        SteamAPI_ManualDispatch_Init();
    }

    // SteamAPI_GetHSteamPipe() / SteamGameServer_GetHSteamPipe()
    public static unsafe void RunCallbacks(HSteamPipe hSteamPipe)
    {
        // from steam_api.h#L124 (see Manual callback loop)

        SteamAPI_ManualDispatch_RunFrame(hSteamPipe);

        CallbackMsg_t pCallback;
        while (SteamAPI_ManualDispatch_GetNextCallback(hSteamPipe, &pCallback))
        {
            try
            {
                // Check for dispatching API call results
                if (pCallback.m_iCallback == SteamAPICallCompleted_t.k_iCallback)
                {
                    var pCallCompleted = Marshal.PtrToStructure<SteamAPICallCompleted_t>((IntPtr) pCallback.m_pubParam);
                    var pTmpCallResult = NativeMemory.Alloc(pCallCompleted.m_cubParam);
                    try
                    {
                        bool bFailed;
                        if (SteamAPI_ManualDispatch_GetAPICallResult(hSteamPipe, pCallCompleted.m_hAsyncCall, pTmpCallResult, (int) pCallCompleted.m_cubParam, pCallCompleted.m_iCallback, &bFailed) || bFailed)
                        {
                            // Dispatch the call result to the registered handler(s) for the
                            // call identified by pCallCompleted->m_hAsyncCall
                            if (ResultCallbacks.Remove(pCallCompleted.m_hAsyncCall, out var callback))
                                callback((IntPtr) pTmpCallResult, bFailed);
                        }
                    }
                    finally
                    {
                        NativeMemory.Free(pTmpCallResult);
                    }
                }
                else
                {
                    // Look at callback.m_iCallback to see what kind of callback it is,
                    // and dispatch to appropriate handler(s)
                    HandleCallback(in pCallback);
                }
            }
            finally
            {
                SteamAPI_ManualDispatch_FreeLastCallback(hSteamPipe);
            }
        }
    }

    private static unsafe void HandleCallback(in CallbackMsg_t pCallback)
    {
    }

    internal static void RegisterCallResult(SteamAPICall_t handle, SteamCallbackNative callback)
    {
        ResultCallbacks[handle] = callback;
    }
}