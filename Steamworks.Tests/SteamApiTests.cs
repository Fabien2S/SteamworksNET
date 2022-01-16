using System;
using System.Text;
using System.Text.Json;
using NUnit.Framework;
using Steamworks.Callbacks;
using static Steamworks.SteamClientAPI;

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

        while (wait) SteamAPI_RunCallbacks();
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

        var lobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create();
        
        var handle = SteamMatchmaking().RequestLobbyList();
        lobbyMatchListCallResult.Set(handle, (in LobbyMatchList_t lobbyMatchList, in bool failed) =>
        {
            if (failed)
                Assert.Fail("SteamAPI_ISteamMatchmaking_RequestLobbyList failed!");

            var msg = new StringBuilder();
            for (var i = 0; i < lobbyMatchList.m_nLobbiesMatching; i++)
            {
                var lobbyId = SteamMatchmaking().GetLobbyByIndex(i);
                msg.AppendLine("lobby " + lobbyId);
            }

            wait = true;
            Assert.Pass(msg.ToString());
        });

        while (wait) SteamAPI_RunCallbacks();
    }

    [Test]
    public void SteamMatchmaking_CreateLobby()
    {
        var wait = true;

        var lobbyEnterCallback = Callback<LobbyEnter_t>.Create((in LobbyEnter_t result) =>
        {
            wait = false;
            Assert.Pass(result.m_ulSteamIDLobby.ToString());
        });

        SteamMatchmaking().CreateLobby(ELobbyType.k_ELobbyTypePrivate, 1);

        while (wait) SteamAPI_RunCallbacks();
    }
}