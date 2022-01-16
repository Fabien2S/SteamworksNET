using System.Runtime.InteropServices;
using static Steamworks.SteamNative;

namespace Steamworks.Callbacks;

internal delegate void CallResultDispatcher(in IntPtr result, in bool failed);
internal delegate void CallbackDispatcher(in IntPtr result);

internal static class SteamDispatcher
{
    private static readonly Dictionary<SteamAPICall_t, CallResultDispatcher> CallResultHandlers = new();
    
    // TODO Can this be generated at runtime with a switch-case?
    private static readonly Dictionary<int, List<CallbackDispatcher>> CallbackHandlers = new();
    private static readonly Dictionary<int, List<CallbackDispatcher>> CallbackGameServerHandlers = new();

    static SteamDispatcher()
    {
        // Initialize dispatch
        SteamAPI_ManualDispatch_Init();
    }

    public static void RunUserCallbacks()
    {
        var hSteamPipe = SteamAPI_GetHSteamPipe();
        RunCallbacks(hSteamPipe, CallResultHandlers, CallbackHandlers);
    }

    public static void RunGameServerCallbacks()
    {
        var hSteamPipe = SteamGameServer_GetHSteamPipe();
        RunCallbacks(hSteamPipe, CallResultHandlers, CallbackGameServerHandlers);
    }

    private static unsafe void RunCallbacks(
        HSteamPipe hSteamPipe,
        Dictionary<SteamAPICall_t, CallResultDispatcher> callResults,
        Dictionary<int, List<CallbackDispatcher>> callbacks
    )
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
                            if (callResults.Remove(pCallCompleted.m_hAsyncCall, out var callback))
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
                    if (callbacks.TryGetValue(pCallback.m_iCallback, out var handlers))
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

    public static void RegisterCallResult(SteamAPICall_t handle, CallResultDispatcher callback)
    {
        CallResultHandlers[handle] = callback;
    }

    public static void UnregisterCallResult(SteamAPICall_t handle)
    {
        CallResultHandlers.Remove(handle);
    }

    public static void RegisterCallback(int id, bool isGameServer, CallbackDispatcher callback)
    {
        var callbackHandlers = isGameServer ? CallbackGameServerHandlers : CallbackHandlers;
        if (!callbackHandlers.TryGetValue(id, out var list))
            callbackHandlers[id] = list = new List<CallbackDispatcher>();
        list.Add(callback);
    }

    public static void UnregisterCallback(int id, bool isGameServer, CallbackDispatcher callback)
    {
        var callbackHandlers = isGameServer ? CallbackGameServerHandlers : CallbackHandlers;
        if (callbackHandlers.TryGetValue(id, out var list))
            list.Remove(callback);
    }
}