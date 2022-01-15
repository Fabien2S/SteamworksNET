using System.Runtime.InteropServices;
using static Steamworks.SteamNative;

namespace Steamworks;

public partial class SteamDispatcher
{
    // HSteamPipe hSteamPipe = SteamAPI_GetHSteamPipe(); // See also SteamGameServer_GetHSteamPipe()
    internal static unsafe void RunCallbacks(HSteamPipe hSteamPipe)
    {
        // from steam_api.h#L124 (see Manual callback loop)

        SteamAPI_ManualDispatch_RunFrame(hSteamPipe);

        CallbackMsg_t* pCallback = default;
        while (SteamAPI_ManualDispatch_GetNextCallback(hSteamPipe, pCallback))
        {
            try
            {
                // Check for dispatching API call results
                if (pCallback->m_iCallback == (int) SteamAPICallCompleted_t.k_iCallback)
                {
                    var pCallCompleted = (SteamAPICallCompleted_t*) pCallback;
                    var pTmpCallResult = NativeMemory.Alloc((nuint) pCallback->m_cubParam);
                    try
                    {
                        bool bFailed;
                        if (SteamAPI_ManualDispatch_GetAPICallResult(hSteamPipe, pCallCompleted->m_hAsyncCall, pTmpCallResult, pCallback->m_cubParam, pCallback->m_iCallback, &bFailed))
                        {
                            // Dispatch the call result to the registered handler(s) for the
                            // call identified by pCallCompleted->m_hAsyncCall
                            HandleCallResult(in pCallback, in bFailed);
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

    static unsafe partial void HandleCallResult(in CallbackMsg_t* pCallback, in bool bFailed);
    static unsafe partial void HandleCallback(in CallbackMsg_t* pCallback);

    // public static unsafe void Subscribe(SteamCallback<LobbyCreated_t> callback)
    // {
    //     const int callbackIndex = 4;
    //
    //     ref var listPtr = ref _callbacks[callbackIndex];
    //     listPtr ??= new List<IntPtr>();
    //     listPtr.Add(Marshal.GetFunctionPointerForDelegate(callback));
    // }
    //
    // public static unsafe void Subscribe(SteamCallback<LobbyMatchList_t> callback)
    // {
    //     const int callbackIndex = 4;
    //
    //     ref var listPtr = ref _callbacks[callbackIndex];
    //     listPtr ??= new List<IntPtr>();
    //     listPtr.Add(Marshal.GetFunctionPointerForDelegate(callback));
    // }
}