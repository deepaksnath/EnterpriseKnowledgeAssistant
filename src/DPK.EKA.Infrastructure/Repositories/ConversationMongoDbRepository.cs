using DPK.EKA.Domain.Entities;
using DPK.EKA.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace DPK.EKA.Infrastructure.Repositories
{
    public class ConversationMongoDbRepository : IConversationRepository
    {
        private readonly IMongoCollection<MongoConversationDocument> _collection;

        public ConversationMongoDbRepository(IMongoClient mongoClient,
                                             IConfiguration configuration)
        {
            var database = mongoClient.GetDatabase(configuration.GetSection("MongoDbSettings").GetValue<string>("DatabaseName"));
            _collection = database.GetCollection<MongoConversationDocument>("Conversations");
        }

        public async Task<Conversation> CreateConversationAsync(Conversation conversation)
        {
            var doc = new MongoConversationDocument
            {
                ConId = conversation.Id.ToString(),
                Query = conversation.Query,
                Answer = conversation.Answer,
                Source = conversation.Source,
                ApiVersion = conversation.ApiVersion,
                CreatedAt = conversation.CreatedAt
            };

            await _collection.InsertOneAsync(doc);

            return conversation;
        }
    }
    internal class MongoConversationDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? ConId { get; set; }

        public DateTime CreatedAt { get; set; } = default;

        public string? Query { get; set; } = default;

        public string? Answer { get; set; } = default;

        public string? ApiVersion { get; set; } = default;

        public string? Source { get; set; } = default;
    }
}
