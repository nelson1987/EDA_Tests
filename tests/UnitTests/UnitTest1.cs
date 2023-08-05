using EDA.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EDA.Unit.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
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
    public async Task Test2()
    {
        var logger = new Mock<ILogger<ContaController>>();
        var mediator = new Mock<IMediator>();
        var controller = new ContaController(logger.Object, mediator.Object);
        controller.ModelState.AddModelError("Name", "Fake Error");
        AberturaContaCommand vm = new AberturaContaCommand()
        {
            ClientName = "Teste",
            ClientDocument = "28394"
        };
        IActionResult result = await controller.Post(vm, new CancellationToken());
        //var viewResult = (AberturaContaCommandResponse)result;
        //Assert.That(viewResult.ClientName, Is.EqualTo(vm.ClientName));
    }
    [Test]
    public void Test3()
    {
        var logger = new Mock<ILogger<ContaController>>();
        var mediator = new Mock<IMediator>();
        var controller = new ContaController(logger.Object, mediator.Object);
        AberturaContaCommandResponse response = new AberturaContaCommandResponse("Teste", "28394");
        mediator.Setup(x => x.Send(It.IsAny<AberturaContaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

        //controller.ModelState.AddModelError("Name", "Fake Error");
        //AberturaContaCommand vm = new AberturaContaCommand()
        //{
        //    ClientName = "Teste",
        //    ClientDocument = "28394"
        //};
        //IActionResult result = await controller.Post(vm, new CancellationToken());
        //var viewResult = (AberturaContaCommandResponse)result;
        //Assert.That(viewResult.ClientName, Is.EqualTo(vm.ClientName));
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