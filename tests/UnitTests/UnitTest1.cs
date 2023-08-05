using EDA.Api.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EDA.Unit.Tests;

public class Tests
{
    public Mock<ILogger<ContaController>> logger { get; private set; }
    public Mock<IMediator> mediator { get; private set; }
    public ContaController controller { get; private set; }

    [SetUp]
    public void Setup()
    {
        logger = new Mock<ILogger<ContaController>>();
        mediator = new Mock<IMediator>();
        controller = new ContaController(logger.Object, mediator.Object);
    }

    [Test]
    public async Task Test1()
    {
        var myMockDependency = new Mock<IMyInterface>();
        myMockDependency.Setup(x => x.MyMethodAsync(5))
                .ReturnsAsync(10);
        int res = await myMockDependency.Object.MyMethodAsync(5);
        myMockDependency.Verify(x => x.MyMethodAsync(5), Times.AtLeast(1));


        var myInstanceToTest = new ClassToTest();
        Assert.That(myInstanceToTest.MethodToTest(1), Is.EqualTo(5));
    }
    [Test]
    public async Task PostMethod_Has_Invalid_ModelState()
    {
        controller.ModelState.AddModelError("Name", "Fake Error");
        AberturaContaCommand vm = new AberturaContaCommand()
        {
            ClientName = "Teste",
            ClientDocument = "28394"
        };
        ObjectResult result = (ObjectResult)await controller.Post(vm, new CancellationToken());
        result.StatusCode.Should().Be(500);
    }
    [Test]
    public async Task PostMethod_Successfully()
    {
        AberturaContaCommandResponse mockResponse = new AberturaContaCommandResponse("Teste", "28394");
        mediator.Setup(x => x.Send(It.IsAny<AberturaContaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse);
        ObjectResult result = (ObjectResult)await controller.Post(It.IsAny<AberturaContaCommand>(), new CancellationToken());
        result.Value.Should().BeOfType<AberturaContaCommandResponse>();
        result.StatusCode.Should().Be(200);

        AberturaContaCommandResponse response = (AberturaContaCommandResponse)result.Value;
        response.ClientName.Should().Be(mockResponse.ClientName);
        response.ClientDocument.Should().Be(mockResponse.ClientDocument);
    }
}
public class ClassToTest
{
    public int MethodToTest(int v)
    {
        return v + 4;
    }
}
public interface IMyInterface
{
    Task<int> MyMethodAsync(int v);
}