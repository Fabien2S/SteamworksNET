global using static Steamworks.SteamAPI;
using System;
using System.Globalization;
using System.Text;
using NUnit.Framework;

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
        SteamCallResult<LobbyMatchList_t> lobbyMatchListCallResult = default;
        
        
        var call = SteamMatchmaking().RequestLobbyList();
        lobbyMatchListCallResult.Set(call, (lobbyList, ioFailure) =>
        {
            
        });
        
        SteamDispatcher.Subscribe((LobbyMatchList_t result) =>
        {
            var lobbyCount = result.m_nLobbiesMatching;
            
            var msg = new StringBuilder();
            for (var i = 0; i < lobbyCount; i++)
            {
                var friendId = SteamMatchmaking().GetLobbyByIndex(i);
                var friendName = SteamMatchmaking().getlobby(friendId);
                msg.AppendLine($"friend {i} (id: {friendId}, name: {friendName})");
            }
            
            Assert.Pass(lobbyCount.ToString(NumberFormatInfo.InvariantInfo));
        });
        var t = SteamCallback<LobbyMatchList_t>.Create(lobbyMatchList =>
        {
            Assert.Pass(lobbyMatchList.m_nLobbiesMatching.ToString(NumberFormatInfo.InvariantInfo));
        });

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