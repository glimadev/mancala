using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System.Net.Http;

namespace Mancala.Integration.Tests;

public class BaseFixture
{
    public static WebApplicationFactory<Program> Factory;
    public static HttpClient Client { get; private set; }

    [OneTimeSetUp]
    public static void RunBeforeEachTestFixture()
    {
        Factory = new WebApplicationFactory<Program>();
        Client = Factory.CreateClient();
    }
}