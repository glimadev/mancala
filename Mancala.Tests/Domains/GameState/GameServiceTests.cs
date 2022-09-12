using Mancala.Domains.Game;
using Mancala.Domains.Game.Models;
using Mancala.Domains.Game.Repository;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Mancala.Tests.Domains.Game
{
    public class GameServiceTests
    {
        GameService gameService;
        private Mock<IGameStateRepository> gameStateRepositoryMock;
        private Mock<IHttpContextAccessor> httpContextAccessorMock;
        const string ipAddress = "192.168.1.1";

        [SetUp]
        public void Setup()
        {
            httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(a => a.HttpContext.Connection.RemoteIpAddress).Returns(IPAddress.Parse(ipAddress));

            gameStateRepositoryMock = new Mock<IGameStateRepository>();
            gameService = new GameService(gameStateRepositoryMock.Object, httpContextAccessorMock.Object);
        }

        [Test]
        [TestCase(4, 6)]
        [TestCase(1, 3)]
        [TestCase(3, 4)]
        [TestCase(4, 5)]
        [TestCase(5, 6)]
        [TestCase(9, 7)]
        public void Setup_When_Number_Of_Pits_Are_OK(int rocks, int pits)
        {
            Assert.DoesNotThrowAsync(() => gameService.Setup(rocks, pits));
        }

        [Test]
        [TestCase(0, 6)]
        [TestCase(1, 0)]
        public void Setup_When_Number_Of_Pits_Are_Not_OK(int rocks, int pits)
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => gameService.Setup(rocks, pits));
        }

        [Test]
        public async Task Setup_When_Number_Of_Pits_Are_OK_2()
        {
            var expectedGameState = GetMockGameState6();

            expectedGameState.Id = ipAddress;

            var resultGameState = await gameService.Setup(4, 6);

            gameStateRepositoryMock.Verify(x => x.Save(It.IsAny<GameStateModel>()));

            Assert.AreEqual(expectedGameState.ToString(), resultGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player1_Has_Last_Rock_In_Player2_Little_Pit()
        {
            var mockGameState = GetMockGameState1(Player.Player1);
            var expectedGameState = GetMockGameState1(Player.Player1);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(2);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.CurrentPlayer = Player.Player2;
            expectedGameState.Pits[2].Rocks = 0;
            expectedGameState.Pits[3].Rocks = 4;
            expectedGameState.Pits[4].Rocks = 4;
            expectedGameState.Pits[5].Rocks = 4;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player1_Has_Last_Rock_In_Big_Pit()
        {
            var mockGameState = GetMockGameState1(Player.Player1);
            var expectedGameState = GetMockGameState1(Player.Player1);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(0);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.CurrentPlayer = Player.Player1;
            expectedGameState.Pits[0].Rocks = 0;
            expectedGameState.Pits[1].Rocks = 4;
            expectedGameState.Pits[2].Rocks = 4;
            expectedGameState.Pits[3].Rocks = 4;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player2_Has_Last_Rock_In_Player1_Little_Pit()
        {
            var mockGameState = GetMockGameState1(Player.Player2);
            var expectedGameState = GetMockGameState1(Player.Player2);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(6);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.CurrentPlayer = Player.Player1;
            expectedGameState.Pits[6].Rocks = 0;
            expectedGameState.Pits[7].Rocks = 4;
            expectedGameState.Pits[0].Rocks = 4;
            expectedGameState.Pits[1].Rocks = 4;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player2_Has_Last_Rock_In_Big_Pit()
        {
            var mockGameState = GetMockGameState1(Player.Player2);
            var expectedGameState = GetMockGameState1(Player.Player2);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(4);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.CurrentPlayer = Player.Player2;
            expectedGameState.Pits[4].Rocks = 0;
            expectedGameState.Pits[5].Rocks = 4;
            expectedGameState.Pits[6].Rocks = 4;
            expectedGameState.Pits[7].Rocks = 4;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player1_Claims_Opponent_Rocks()
        {
            var mockGameState = GetMockGameState2();
            var expectedGameState = GetMockGameState2();

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(1);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.CurrentPlayer = Player.Player2;
            expectedGameState.Pits[1].Rocks = 0;
            expectedGameState.Pits[2].Rocks = 0;
            expectedGameState.Pits[3].Rocks = 13;
            expectedGameState.Pits[4].Rocks = 0;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player2_Claims_Opponent_Rocks()
        {
            var mockGameState = GetMockGameState3();
            var expectedGameState = GetMockGameState3();

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(5);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.CurrentPlayer = Player.Player1;
            expectedGameState.Pits[5].Rocks = 0;
            expectedGameState.Pits[6].Rocks = 0;
            expectedGameState.Pits[7].Rocks = 15;
            expectedGameState.Pits[0].Rocks = 0;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player1_Wins()
        {
            var mockGameState = GetMockGameState4();
            var expectedGameState = GetMockGameState4();

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(1);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.GameOver = true;
            expectedGameState.Winner = Player.Player1;
            expectedGameState.CurrentPlayer = Player.Player1;
            expectedGameState.Pits[1].Rocks = 0;
            expectedGameState.Pits[2].Rocks = 0;
            expectedGameState.Pits[3].Rocks = 14;
            expectedGameState.Pits[4].Rocks = 0;
            expectedGameState.Pits[5].Rocks = 0;
            expectedGameState.Pits[7].Rocks = 10;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public async Task Move_When_Player2_Wins()
        {
            var mockGameState = GetMockGameState5();
            var expectedGameState = GetMockGameState5();

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            await gameService.Move(4);

            gameStateRepositoryMock.Verify(x => x.Save(mockGameState));

            expectedGameState.GameOver = true;
            expectedGameState.Winner = Player.Player2;
            expectedGameState.CurrentPlayer = Player.Player2;
            expectedGameState.Pits[1].Rocks = 0;
            expectedGameState.Pits[3].Rocks = 8;
            expectedGameState.Pits[4].Rocks = 0;
            expectedGameState.Pits[6].Rocks = 0;
            expectedGameState.Pits[7].Rocks = 16;

            Assert.AreEqual(expectedGameState.ToString(), mockGameState.ToString());
        }

        [Test]
        public void Move_When_Player1_Tries_To_Move_Opponent_Pit()
        {
            var mockGameState = GetMockGameState1(Player.Player2);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            Assert.ThrowsAsync<InvalidOperationException>(() => gameService.Move(1));
        }

        [Test]
        public void Move_When_Player2_Tries_To_Move_Opponent_Pit()
        {
            var mockGameState = GetMockGameState1(Player.Player1);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            Assert.ThrowsAsync<InvalidOperationException>(() => gameService.Move(4));
        }

        [Test]
        public void Move_When_Player1_Tries_To_Move_BigPit()
        {
            var mockGameState = GetMockGameState1(Player.Player1);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            Assert.ThrowsAsync<InvalidOperationException>(() => gameService.Move(3));
        }

        [Test]
        public void Move_When_Player2_Tries_To_Move_BigPit()
        {
            var mockGameState = GetMockGameState1(Player.Player2);

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            Assert.ThrowsAsync<InvalidOperationException>(() => gameService.Move(7));
        }

        [Test]
        public void Move_When_Player_Tries_To_Move_Empty_Pit()
        {
            var mockGameState = GetMockGameState1(Player.Player1);

            mockGameState.Pits[0].Rocks = 0;

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            Assert.ThrowsAsync<InvalidOperationException>(() => gameService.Move(0));
        }

        [Test]
        public void Move_When_Game_Is_Over()
        {
            var mockGameState = GetMockGameState1(Player.Player1);

            mockGameState.GameOver = true;

            gameStateRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(mockGameState);

            Assert.ThrowsAsync<InvalidOperationException>(() => gameService.Move(0));
        }

        private static GameStateModel GetMockGameState1(Player player) => new()
        {
            CurrentPlayer = player,
            Pits = new GameStatePits[]
            {
                new GameStatePits { Rocks = 3,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 3,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 3,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 3,  Player= Player.Player1, IsBigPit = true },
                new GameStatePits { Rocks = 3,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 3,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 3,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 3,  Player= Player.Player2, IsBigPit = true }
            }
        };

        private static GameStateModel GetMockGameState2() => new()
        {
            CurrentPlayer = Player.Player1,
            Pits = new GameStatePits[]
            {
                new GameStatePits { Rocks = 1,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 1,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 7,  Player= Player.Player1, IsBigPit = true },
                new GameStatePits { Rocks = 5,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 1,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 9,  Player= Player.Player2, IsBigPit = true }
            }
        };

        private static GameStateModel GetMockGameState3() => new()
        {
            CurrentPlayer = Player.Player2,
            Pits = new GameStatePits[]
            {
                new GameStatePits { Rocks = 5,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 1,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 7,  Player= Player.Player1, IsBigPit = true },
                new GameStatePits { Rocks = 1,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 1,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 9,  Player= Player.Player2, IsBigPit = true }
            }
        };

        private static GameStateModel GetMockGameState4() => new()
        {
            CurrentPlayer = Player.Player1,
            Pits = new GameStatePits[]
            {
                new GameStatePits { Rocks = 0,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 1,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 8,  Player= Player.Player1, IsBigPit = true },
                new GameStatePits { Rocks = 5,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 1,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 9,  Player= Player.Player2, IsBigPit = true }
            }
        };

        private static GameStateModel GetMockGameState5() => new()
        {
            CurrentPlayer = Player.Player2,
            Pits = new GameStatePits[]
            {
                new GameStatePits { Rocks = 0,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 1,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 8,  Player= Player.Player1, IsBigPit = true },
                new GameStatePits { Rocks = 1,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 5,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 9,  Player= Player.Player2, IsBigPit = true }
            }
        };

        private static GameStateModel GetMockGameState6() => new()
        {
            CurrentPlayer = Player.Player1,
            Pits = new GameStatePits[]
            {
                new GameStatePits { Rocks = 4,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player1, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player1, IsBigPit = true },
                new GameStatePits { Rocks = 4,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 4,  Player= Player.Player2, IsBigPit = false },
                new GameStatePits { Rocks = 0,  Player= Player.Player2, IsBigPit = true }
            }
        };
    }
}