using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapy.Domain.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column("name")]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime? EmailVerified { get; set; }

        public string? Image { get; set; }
    }
}