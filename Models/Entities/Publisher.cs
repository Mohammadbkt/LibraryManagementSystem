using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class Publisher
    {
        public int Id { get; set; }


        public string Name { get; set; } = string.Empty;

        

        [Url]
        public string? Website { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}