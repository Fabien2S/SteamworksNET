global using static Steamworks.SteamAPI;
using System;
using System.Text;
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
        var waitUntil = false;

        var handle = SteamMatchmaking().RequestLobbyList();

        CallResult<LobbyMatchList_t> lobbyMatchListCallResult;
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

            Assert.Pass(msg.ToString());
            waitUntil = true;
        });


        while (true)
        {
            SteamDispatcher.RunCallbacks(SteamAPI_GetHSteamPipe());
            if (waitUntil) break;
        }
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