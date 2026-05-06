using DPK.EKA.Api.Controllers;
using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DPK.EKA.Api.Tests
{
    public class RagControllerTests
    {
        private Mock<IRagService> _ragServiceMock;
        private Mock<ILogger<RagV1Controller>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new();
            _ragServiceMock = new();
        }

        [Test]
        public void Query_OnSuccess_ReturnsRagResponse200()
        {
            // Arrange
            var ragResp = new RagResponse(
                                     Answer: "Answer 1",
                                     Sources: new List<string> { "Source 1", "Source 2" });
            _ragServiceMock.Setup(s => s.GetAnswerAsync(It.IsAny<string>()))
                           .ReturnsAsync(ragResp);

            RagV1Controller ragController = new RagV1Controller(_ragServiceMock.Object, _loggerMock.Object);

            // Act
            var result = ragController.Query(new RagRequest("What is EKA?"));

            // Assert
            _ragServiceMock.Verify(s => s.GetAnswerAsync(It.IsAny<string>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }
    }
}
