using DPK.EKA.Application.Interfaces;
using DPK.EKA.Application.Models;
using DPK.EKA.Application.Services;
using DPK.EKA.Domain.Models;
using DPK.EKA.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace DPK.EKA.Application.Tests
{
    public class Tests
    {
        private Mock<IEmbeddingService> _embeddingServiceMock;
        private Mock<ISearchService> _searchServiceMock;
        private Mock<IConversationService> _conversationServiceMock;
        private Mock<IChatService> _chatServiceMock;
        private Mock<ILogger<RagService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _chatServiceMock = new ();
            _embeddingServiceMock = new ();
            _searchServiceMock = new ();
            _conversationServiceMock = new ();
            _loggerMock = new ();
        }

        [Test]
        public async Task Test1()
        {
            //Arrange
            _searchServiceMock.Setup(s => s.SearchAsync(It.IsAny<float[]>(), It.IsAny<string>()))
                              .ReturnsAsync(new List<DocumentChunk> { new DocumentChunk
                              {
                                  Id = "123",
                                  Content = "This",
                                  ContentVector = new float[] { 0.1f, 0.2f },
                                  Source = "TestSource.pdf",
                                  UploadedAt = DateTime.Now
                              }});
            _embeddingServiceMock.Setup(e => e.CreateEmbeddingAsync(It.IsAny<string>()))
                                 .ReturnsAsync(new float[] { 0.1f, 0.2f });
            _chatServiceMock.Setup(c => c.GetRagResponseAsync(It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync("This is a test response.");

            var ragService = new RagService(_embeddingServiceMock.Object,
                                            _chatServiceMock.Object,
                                            _conversationServiceMock.Object,
                                            _searchServiceMock.Object, 
                                            _loggerMock.Object);

            //Act
            var result = ragService.GetAnswerAsync("What is this?");

            //Assert
            _searchServiceMock.Verify(s => s.SearchAsync(It.IsAny<float[]>(), It.IsAny<string>()), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Result, Is.InstanceOf<RagResponse>());
        }
    }
}
