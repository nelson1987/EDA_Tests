using EDA.Api.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EDA.Unit.Tests;

public class Tests
{
    private Mock<ILogger<ContaController>> MockLogger { get; set; }
    private Mock<IMediator> MockMediator { get; set; }
    private ContaController Controller { get; set; }

    [SetUp]
    public void Setup()
    {
        MockLogger = new Mock<ILogger<ContaController>>();
        MockMediator = new Mock<IMediator>();
        Controller = new ContaController(MockLogger.Object, MockMediator.Object);
    }

    [Test]
    public async Task Test1()
    {
        var myMockDependency = new Mock<IMyInterface>();
        myMockDependency.Setup(x => x.MyMethodAsync(5))
                .ReturnsAsync(10);
        int res = await myMockDependency.Object.MyMethodAsync(5);
        myMockDependency.Verify(x => x.MyMethodAsync(5), Times.AtLeast(1));

        Assert.That(ClassToTest.MethodToTest(1), Is.EqualTo(5));
    }
    [Test]
    public async Task PostMethod_Has_Invalid_ModelState()
    {
        Controller.ModelState.AddModelError("Name", "Fake Error");
        AberturaContaCommand vm = new()
        {
            ClientName = "Teste",
            ClientDocument = "28394"
        };
        ObjectResult result = (ObjectResult)await Controller.Post(vm, new CancellationToken());
        result.StatusCode.Should().Be(500);
    }
    [Test]
    public async Task PostMethod_Successfully()
    {
        AberturaContaCommandResponse mockResponse = new(Guid.NewGuid(), "Teste", "28394");
        MockMediator.Setup(x => x.Send(It.IsAny<AberturaContaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);
        ObjectResult result = (ObjectResult)await Controller.Post(It.IsAny<AberturaContaCommand>(), new CancellationToken());
        result.Value.Should().BeOfType<AberturaContaCommandResponse>();
        result.StatusCode.Should().Be(200);

        AberturaContaCommandResponse response = (AberturaContaCommandResponse)result.Value;
        response.ClientName.Should().Be(mockResponse.ClientName);
        response.ClientDocument.Should().Be(mockResponse.ClientDocument);
    }
}
public class ClassToTest
{
    public static int MethodToTest(int v)
    {
        return v + 4;
    }
}
public interface IMyInterface
{
    Task<int> MyMethodAsync(int v);
}