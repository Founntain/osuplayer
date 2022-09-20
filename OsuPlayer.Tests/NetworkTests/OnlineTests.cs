﻿using System;
using System.Threading.Tasks;
using NUnit.Framework;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Network.Online;

namespace OsuPlayer.Tests;

public class OnlineTests
{
    [Test]
    public void UserTest()
    {
        var user = new User
        {
            Id = Guid.Empty,
            Description = default,
            Level = default,
            Name = default,
            Role = UserRole.User,
            Version = default,
            Xp = default,
            AmountDonated = default,
            IsDonator = default,
            CustomRolename = default,
            JoinDate = default,
            LastSeen = default,
            OsuProfile = default,
            ProfilePicture = default,
            SongsPlayed = default,
            TotalXp = default,
            VersionDate = default,
            CustomRoleColor = default,
            CustomWebBackground = default,
            HasXpLock = default
        };

        Assert.IsNotNull(user);

        for (var i = 0; i <= 7; i++)
        {
            user.Role = (UserRole) i;

            Assert.IsNotNull(user.RoleColor);
            Assert.IsNotEmpty(user.RoleString);
        }

        Assert.IsNotEmpty(user.ToString());
        Assert.IsEmpty(user.SongsPlayedString);
        Assert.IsNotEmpty(user.LevelString);
        Assert.IsEmpty(user.LevelProgressString);
        Assert.IsEmpty(user.LevelAndTotalXpString);
        Assert.IsNotEmpty(user.DescriptionTitleString);
        Assert.IsNotEmpty(user.JoinDateString);
        Assert.IsNotEmpty(user.TotalXpString);

        Assert.GreaterOrEqual(user.GetXpNeededForNextLevel(), 0);
        Assert.GreaterOrEqual(User.GetXpNeededForNextLevel(1), 0);
    }

    [Test]
    public void ArticleTest()
    {
        var article = new Article
        {
            Content = "Content",
            Creator = "Creator",
            Title = "Title",
            CreationTime = DateTime.UtcNow
        };

        Assert.IsNotNull(article);

        Assert.IsNotEmpty(article.Content);
        Assert.IsNotEmpty(article.Creator);
        Assert.IsNotEmpty(article.Title);
        Assert.Greater(article.CreationTime, DateTime.MinValue);
        Assert.IsNotEmpty(article.CreationTimeString);
    }

    [Test]
    public void NewsTest()
    {
        var news = new News
        {
            Content = "Content",
            Creator = "Creator",
            Title = "Title",
            CreationTime = DateTime.UtcNow
        };

        Assert.IsNotEmpty(news.Content);
        Assert.IsNotEmpty(news.Creator);
        Assert.IsNotEmpty(news.Title);
        Assert.Greater(news.CreationTime, DateTime.MinValue);
        Assert.IsNotEmpty(news.CreationTimeString);
    }

    [Test]
    public void OnlineStatusModel()
    {
        var model = new OnlineUserStatusModelExtended();

        Assert.IsNotEmpty(model.Status);

        model.StatusType = UserOnlineStatusType.Listening;

        Assert.IsNotEmpty(model.Status);

        model.StatusType = UserOnlineStatusType.InParty;

        Assert.IsNotEmpty(model.Status);
    }

    [Test]
    public async Task ProfileManagerTest()
    {
        var result = await ProfileManager.LoadProfile("TestUser", "TestPassword");

        Assert.IsNull(result);
    }
}