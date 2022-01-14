using System;
using System.Text;
using NUnit.Framework;

namespace Steamworks.Tests;

public class SteamApiTests
{
    [SetUp]
    public void SteamAPI_Init()
    {
        Environment.SetEnvironmentVariable("SteamAppId", "480");
        Environment.SetEnvironmentVariable("SteamGameId", "480");

        var init = SteamAPI.Init();
        Assert.IsTrue(init, "SteamAPI.Init()");
    }

    [Test]
    public void ISteamUser_GetSteamID()
    {
        var steamUser = ISteamUser.SteamAPI_SteamUser_v021();
        var steamId = ISteamUser.GetSteamID(steamUser);
        Assert.NotZero(steamId);
        Assert.Pass(steamId.ToString());
    }

    [Test]
    public void ISteamFriends_GetPersonaName()
    {
        var steamFriends = ISteamFriends.SteamAPI_SteamFriends_v017();
        var personaName = ISteamFriends.GetPersonaName(steamFriends);
        Assert.Pass(personaName);
    }

    [Test]
    public void ISteamFriends_GetFriendList()
    {
        var steamFriends = ISteamFriends.SteamAPI_SteamFriends_v017();
        
        var count = ISteamFriends.GetFriendCount(steamFriends, (int) EFriendFlags.k_EFriendFlagAll);

        var msg = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            var friendId = ISteamFriends.GetFriendByIndex(steamFriends, i, (int) EFriendFlags.k_EFriendFlagAll);
            var friendName = ISteamFriends.GetFriendPersonaName(steamFriends, friendId);
            msg.AppendLine($"friend {i} (id: {friendId}, name: {friendName})");
        }
        
        Assert.Pass(msg.ToString());
    }
}