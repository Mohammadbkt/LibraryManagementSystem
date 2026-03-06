using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class Bookmark
    {
        public string UserId { get; set; } = string.Empty;
        public int BookId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? Notes { get; set; }
        
        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}