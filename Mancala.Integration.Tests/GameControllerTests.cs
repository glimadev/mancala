using NUnit.Framework;
using Snapshooter.NUnit;
using System.Net;
using System.Threading.Tasks;

namespace Mancala.Integration.Tests;

public class GameControllerTests : BaseFixture
{
    private const string RootRoute = "api/game";

    [OneTimeSetUp]
    public void Setup() { }

    [Test]
    [Order(1)]
    public async Task NewGame_When_OK()
    {
        // Act
        var response = await Client.PostAsync(RootRoute, null);

        // Assert
        Snapshot.Match(await response.Content.ReadAsStringAsync());
    }

    [Test]
    [Order(2)]
    public async Task Move_When_Player1_NoContent()
    {
        // Act
        var response = await Client.PutAsync(RootRoute + "/1", null);
        var responseState = await Client.GetAsync(RootRoute);

        // Assert
        Snapshot.Match(await responseState.Content.ReadAsStringAsync());
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    [Order(3)]
    public async Task Move_When_Player1_Not_Current_Player_BadRequest()
    {
        // Act
        var response = await Client.PutAsync(RootRoute + "/1", null);

        // Assert
        Snapshot.Match(await response.Content.ReadAsStringAsync());
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Test]
    [Order(4)]
    public async Task Move_When_Player2_NoContent()
    {
        // Act
        var response = await Client.PutAsync(RootRoute + "/8", null);
        var responseState = await Client.GetAsync(RootRoute);

        // Assert
        Snapshot.Match(await responseState.Content.ReadAsStringAsync());
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Test]
    [Order(5)]
    public async Task Move_When_Player1_Try_To_Move_Big_Pit_BadRequest()
    {
        // Act
        var response = await Client.PutAsync(RootRoute + "/6", null);

        // Assert
        Snapshot.Match(await response.Content.ReadAsStringAsync());
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}