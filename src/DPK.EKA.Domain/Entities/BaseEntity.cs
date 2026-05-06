using System.ComponentModel.DataAnnotations;

namespace DPK.EKA.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = default;
    }
}
