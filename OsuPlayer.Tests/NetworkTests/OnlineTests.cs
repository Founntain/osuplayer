using System;
using NUnit.Framework;
using OsuPlayer.Api.Data.API.Enums;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Online;

namespace OsuPlayer.Tests.NetworkTests;

public class OnlineTests
{
    [Test]
    public void UserTest()
    {
        var user = new User
        {
            UniqueId = Guid.Empty,
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
            SongsPlayed = default,
            TotalXp = default,
            CustomRoleColor = default,
            CustomBannerUrl = default,
            HasXpLock = default
        };

        Assert.That(user, Is.Not.Null);

        for (var i = 0; i <= 7; i++)
        {
            user.Role = (UserRole) i;

            Assert.That(user.RoleColor, Is.Not.Null);
            Assert.That(user.RoleString, Is.Not.Empty);
        }

        Assert.That(user.ToString(), Is.Not.Empty);
        Assert.That(user.SongsPlayedComplexString, Is.Empty);
        Assert.That(user.LevelString, Is.Not.Empty);
        Assert.That(user.LevelProgressString, Is.Empty);
        Assert.That(user.LevelAndTotalXpString, Is.Empty);
        Assert.That(user.DescriptionTitleString, Is.Not.Empty);
        Assert.That(user.JoinDateString, Is.Not.Empty);
        Assert.That(user.TotalXpString, Is.Not.Empty);

        Assert.That(User.GetXpNeededForNextLevel(1), Is.GreaterThanOrEqualTo(0));
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

        Assert.That(article, Is.Not.Null);

        Assert.That(article.Content, Is.Not.Null);
        Assert.That(article.Creator, Is.Not.Null);
        Assert.That(article.Title, Is.Not.Null);
        Assert.That(article.CreationTime, Is.GreaterThan(DateTime.MinValue));
        Assert.That(article.CreationTimeString, Is.Not.Empty);
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

        Assert.That(news.Content, Is.Not.Empty);
        Assert.That(news.Creator, Is.Not.Empty);
        Assert.That(news.Title, Is.Not.Empty);
        Assert.That(news.CreationTime, Is.GreaterThan(DateTime.MinValue));
        Assert.That(news.CreationTimeString, Is.Not.Empty);
    }
}