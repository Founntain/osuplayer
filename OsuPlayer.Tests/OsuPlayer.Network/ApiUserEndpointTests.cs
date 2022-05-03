using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using Constants = OsuPlayer.Network.Constants;

namespace OsuPlayer.Tests.OsuPlayer.Network;

public class ApiUserEndpointTests
{
    [Test]
    public async Task OfflineModeTests()
    {
        Constants.OfflineMode = true;

        var offlineResult = await ApiAsync.GetRequestAsync<User>("nonExistingController", "nonExistingAction");
        var offlineResult2 = await ApiAsync.ApiRequestAsync<User>("nonExistingController", "nonExistingAction", "");
        var offlineResult3 = await ApiAsync.GetRequestWithParameterAsync<User>("nonExistingController", "nonExistingAction", "");
        
        Assert.IsNull(offlineResult);
        Assert.IsNull(offlineResult2);
        Assert.IsNull(offlineResult3);
        
        Constants.OfflineMode = false;

        var onlineResult = await ApiAsync.GetRequestAsync<User>("nonExistingController", "nonExistingAction");
        var onlineResult2 = await ApiAsync.ApiRequestAsync<(int, bool, string)>("nonExistingController", "nonExistingAction", "");
        var onlineResult3 = await ApiAsync.GetRequestWithParameterAsync<User>("nonExistingController", "nonExistingAction", "");
        
        Assert.IsNull(onlineResult);
        Assert.IsInstanceOf<(int, bool, string)>(onlineResult2);
        Assert.IsNull(onlineResult3);
    }
    
    [Test]
    public async Task FalseGetRequestTest()
    {
        var result = await ApiAsync.GetRequestAsync<Exception>("nonExistingController", "nonExistingAction");

        Assert.IsNull(result);
    }
    
    [Test]
    public async Task GetRequestTest()
    {
        var result = await ApiAsync.GetRequestAsync<List<User>>("Users", "GetUsersWithData");

        Assert.IsNotNull(result);
    }

    [Test]
    public async Task ApiRequestTest()
    {
        var result = await ApiAsync.ApiRequestAsync<User>("users", "edituser");
        
        Assert.AreEqual(Guid.Empty, result?.Id);
    }

    [Test]
    public async Task GetProfilePictureTest()
    {
        var falseResult = await ApiAsync.GetProfilePictureAsync("");
        
        Assert.IsNull(falseResult);
        
        var result = await ApiAsync.GetProfilePictureAsync("Founntain");
        
        Assert.IsNotNull(result);
    }

    // [Test]
    // public async Task GetProfileBannerTest()
    // {
    //     var falseResult = await ApiAsync.GetProfileBannerAsync("");
    //     
    //     Assert.IsNull(falseResult);
    //     
    //     var result = await ApiAsync.GetProfileBannerAsync("https://7.founntain.dev/HzYj2AXt.png");
    //     
    //     Assert.IsNotNull(result);
    // }

    [Test]
    public async Task GetProfileByNameTest()
    {
        var falseResult = await ApiAsync.GetProfileByNameAsync("");
        
        Assert.IsNull(falseResult);
        
        var result = await ApiAsync.GetProfileByNameAsync("Founntain");
        
        Assert.IsNotNull(result);
    }

    [Test]
    public async Task LoadUserWithCredentialsTest()
    {
        var result = await ApiAsync.LoadUserWithCredentialsAsync("Test", "Test");
        
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetBeatmapsPlayedByUserTest()
    {
        var result = await ApiAsync.GetBeatmapsPlayedByUser("Founntain");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Count, 10);
    }
}