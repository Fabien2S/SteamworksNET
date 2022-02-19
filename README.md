## What is SteamworksNET

SteamworksNET is a really low-level C# wrapper generator for the Steamworks SDK.


## Getting Started

### The first best way
For the nicest of the nicest, you should make use of the `using static` instruction.  
Why ? By doing that, you can use the wrapper exactly like the native one. Literally. Try it, you won't regret it.
<details>
  <summary>View sample</summary>

```c#
using static Steamworks.SteamClientAPI;
using static Steamworks.SteamConstants;

if ( SteamAPI_RestartAppIfNecessary( k_uAppIdInvalid ) )
{
    return 1;
}

if ( !SteamAPI_Init() )
{
    return 1;
}

// do your things
[...]

SteamAPI_Shutdown();
```
</details>

### The second best and last way
If you can't (or don't want to) use the `using static`,
you can map one-to-one the native functions with the prefix `SteamClientAPI.`
<details>
  <summary>View sample</summary>

```c#
using Steamworks;

if ( SteamClientAPI.SteamAPI_RestartAppIfNecessary( SteamConstants.k_uAppIdInvalid ) )
{
    return 1;
}

if ( !SteamClientAPI.SteamAPI_Init() )
{
    return 1;
}

// do your things
[...]

SteamClientAPI.SteamAPI_Shutdown();
```
</details>

### The [Game Server](https://partner.steamgames.com/doc/sdk/api#steam_game_servers) way

It works the same way **EXCEPT** you **MUST** use `Steamworks.SteamServerAPI` instead of `Steamworks.SteamClientAPI`.


## Things you should know

### [Call Results](https://partner.steamgames.com/doc/sdk/api#callresults)

Creating CallResult is a must while using the Steamworks SDK
<details>
  <summary>View sample</summary>

```c#
// Initialization code is omitted
[...]

// Creating a CallResult only requires CallResult<T>.Create() where T is the type of the call result
var callResult = CallResult<LobbyMatchList_t>.Create();

// Then we are calling a function that will invoke a call result at some point
// Fortunately, this wrapper is typed, so it returns a CallHandle<T>
var handle = SteamMatchmaking().RequestLobbyList();

// As for the base sdk, we are setting the callback
callResult.Set(handle, (in LobbyMatchList_t lobbyMatchList, in bool failed) =>
{
    // do your things
    [...]
});

[...]
// When you no longer need the event, you can call .Dispose() on it
// You do not need to dispose the call results when shutting down the API
callResult.Dispose()
```
</details>


### [Callback](https://partner.steamgames.com/doc/sdk/api#callbacks)

Creating Callback is also a must while using the Steamworks SDK
<details>
  <summary>View sample</summary>

```c#
// Initialization code is omitted
[...]

// Creating a Callback only requires Callback<T>.Create({function}) where T is the type of the callback
var lobbyEnterCallback = Callback<LobbyEnter_t>.Create((in LobbyEnter_t result) =>
{
    // do your things
    [...]
});

// Then we are calling a function that will invoke a callback
// The callback will be called (do not forget to call SteamAPI_RunCallbacks() every frames)
SteamMatchmaking().CreateLobby(ELobbyType.k_ELobbyTypePrivate, 1);

[...]
// When you no longer need the event, you can call .Dispose() on it
// You do not need to dispose the callbacks when shutting down the API
lobbyEnterCallback.Dispose()

```
</details>
