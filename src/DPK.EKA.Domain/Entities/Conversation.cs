using System.ComponentModel.DataAnnotations;

namespace DPK.EKA.Domain.Entities
{
    public class Conversation : BaseEntity
    {
        [MaxLength(1000)]
        public string? Query { get; set; } = default;

        [MaxLength(1000)]
        public string? Answer { get; set; } = default;

        [MaxLength(10)]
        public string? ApiVersion { get; set; } = default;

        [MaxLength(1000)]
        public string? Source { get; set; } = default;
    }
}
