using Mancala.Config;
using Mancala.Controllers;
using Mancala.Domains.Game;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Mancala.Tests.Controllers;

public class GameControllerTests
{
    GameController gameController;
    private Mock<IGameService> gameService;
    private Mock<IOptions<MancalaOptions>> mancalaOptions;

    [SetUp]
    public void Setup()
    {
        gameService = new Mock<IGameService>();
        mancalaOptions = new Mock<IOptions<MancalaOptions>>();
        gameController = new GameController(gameService.Object, mancalaOptions.Object);
    }

    [Test]
    public async Task Get_When_OK()
    {
        await gameController.Get();

        gameService.Verify(x => x.GetState(), times: Times.Once);
    }

    [Test]
    public async Task Post_When_Created()
    {
        mancalaOptions.Setup(x => x.Value).Returns(new MancalaOptions { Pits = 1, Rocks = 1 });

        await gameController.Post();

        gameService.Verify(x => x.Setup(1, 1), times: Times.Once);
    }

    [Test]
    public async Task Put_When_NoContent()
    {
        await gameController.Put(1);

        gameService.Verify(x => x.Move(1), times: Times.Once);
    }
}