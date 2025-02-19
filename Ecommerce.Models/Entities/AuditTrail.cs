using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Entities
{
    public class AuditTrail : BaseEntity
    {
        public string Id { get; set; }
        [Unicode(false)]
        [MaxLength(100)]
        public string Username { get; set; }
        [Unicode(false)]
        [MaxLength(100)]
        public string EntityName { get; set; }
        [Unicode(false)]
        [MaxLength(10)]
        public string Action { get; set; }
        [Unicode(false)]
        [MaxLength(10000)]
        public string Changes { get; set; }
    }
}
