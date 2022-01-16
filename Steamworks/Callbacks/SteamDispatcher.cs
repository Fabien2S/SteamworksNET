using System.Runtime.InteropServices;
using static Steamworks.SteamAPI;

namespace Steamworks.Callbacks;

public static class SteamDispatcher
{
    private static readonly Dictionary<SteamAPICall_t, CallResultHandler<IntPtr>> CallResultHandlers = new();
    private static readonly Dictionary<int, List<CallbackHandler<IntPtr>>> CallbackHandlers = new(); // TODO Can this be generated at runtime with a switch-case?

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
                        if (SteamAPI_ManualDispatch_GetAPICallResult(hSteamPipe, pCallCompleted.m_hAsyncCall,
                                pTmpCallResult, (int) pCallCompleted.m_cubParam, pCallCompleted.m_iCallback,
                                &bFailed) || bFailed)
                        {
                            // Dispatch the call result to the registered handler(s) for the
                            // call identified by pCallCompleted->m_hAsyncCall
                            if (CallResultHandlers.Remove(pCallCompleted.m_hAsyncCall, out var callback))
                                callback((IntPtr) pTmpCallResult, in bFailed);
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
                    if (CallbackHandlers.TryGetValue(pCallback.m_iCallback, out var handlers))
                    {
                        foreach (var handler in handlers)
                            handler((IntPtr) pCallback.m_pubParam);
                    }
                }
            }
            finally
            {
                SteamAPI_ManualDispatch_FreeLastCallback(hSteamPipe);
            }
        }
    }

    internal static void RegisterCallResult(SteamAPICall_t handle, CallResultHandler<IntPtr> callback)
    {
        CallResultHandlers[handle] = callback;
    }

    internal static void UnregisterCallResult(SteamAPICall_t handle)
    {
        CallResultHandlers.Remove(handle);
    }

    internal static void RegisterCallback(int id, CallbackHandler<IntPtr> callback)
    {
        if (!CallbackHandlers.TryGetValue(id, out var list))
            CallbackHandlers[id] = list = new List<CallbackHandler<IntPtr>>();
        list.Add(callback);
    }

    internal static void UnregisterCallback(int id, CallbackHandler<IntPtr> callback)
    {
        if (CallbackHandlers.TryGetValue(id, out var list))
            list.Remove(callback);
    }
}