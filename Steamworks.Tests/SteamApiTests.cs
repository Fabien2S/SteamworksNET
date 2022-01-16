global using static Steamworks.SteamAPI;
using System;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using Steamworks.Callbacks;

namespace Steamworks.Tests;

public class SteamApiTests
{
    [SetUp]
    public void Initialize()
    {
        Environment.SetEnvironmentVariable("SteamAppId", "480");
        Environment.SetEnvironmentVariable("SteamGameId", "480");

        var init = SteamAPI_Init();
        Assert.IsTrue(init, "SteamAPI.Init()");
    }

    [TearDown]
    public void Shutdown()
    {
        SteamAPI_Shutdown();
    }

    [Test]
    public void ISteamUser_GetSteamID()
    {
        var steamId = SteamUser().GetSteamID();
        Assert.NotZero(steamId);
        Assert.Pass(steamId.ToString());
    }

    [Test]
    public void ISteamFriends_GetPersonaName()
    {
        var personaName = SteamFriends().GetPersonaName();
        Assert.Pass(personaName);
    }

    [Test]
    public void ISteamFriends_SetPersonaName()
    {
        var wait = true;

        var setPersonaNameCallResult = CallResult<SetPersonaNameResponse_t>.Create();

        var personaName = SteamFriends().GetPersonaName();
        var handle = SteamFriends().SetPersonaName(personaName);
        setPersonaNameCallResult.Set(handle, (in SetPersonaNameResponse_t t, in bool failed) =>
        {
            wait = false;
            Assert.IsFalse(failed);
            Assert.Pass(JsonSerializer.Serialize(t));
        });

        while (wait) SteamDispatcher.RunCallbacks(SteamAPI_GetHSteamPipe());
    }

    [Test]
    public void ISteamFriends_GetFriendList()
    {
        var count = SteamFriends().GetFriendCount((int) EFriendFlags.k_EFriendFlagAll);

        var msg = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            var friendId = SteamFriends().GetFriendByIndex(i, (int) EFriendFlags.k_EFriendFlagAll);
            var friendName = SteamFriends().GetFriendPersonaName(friendId);
            msg.AppendLine($"friend {i} (id: {friendId}, name: {friendName})");
        }

        Assert.Pass(msg.ToString());
    }

    [Test]
    public void SteamMatchmaking_RequestLobbyList()
    {
        var wait = true;

        var handle = SteamMatchmaking().RequestLobbyList();

        var lobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create();
        lobbyMatchListCallResult.Set(handle, (in LobbyMatchList_t lobbyMatchList, in bool failed) =>
        {
            if (failed)
                Assert.Fail("SteamAPI_ISteamMatchmaking_RequestLobbyList failed!");

            var lobbyCount = lobbyMatchList.m_nLobbiesMatching;
            var msg = new StringBuilder();
            for (var i = 0; i < lobbyCount; i++)
            {
                var lobbyId = SteamMatchmaking().GetLobbyByIndex(i);
                msg.AppendLine($"lobby " + lobbyId);
            }

            var lobbyEnterCallback = Callback<LobbyEnter_t>.Create((in LobbyEnter_t result) =>
            {
                msg.AppendLine();
                msg.Append(result.m_ulSteamIDLobby);

                wait = false;
                Assert.Pass(msg.ToString());
            });
            SteamMatchmaking().JoinLobby(SteamMatchmaking().GetLobbyByIndex(0));
        });


        while (wait) SteamDispatcher.RunCallbacks(SteamAPI_GetHSteamPipe());
    }

    [Test]
    public void IsBlenderInstalled()
    {
        SteamMatchmaking().RequestLobbyList();
        var blenderApp = new AppId_t(365670);

        var isAppInstalled = SteamApps().BIsAppInstalled(blenderApp);
        Assert.Pass(isAppInstalled.ToString());
    }
}