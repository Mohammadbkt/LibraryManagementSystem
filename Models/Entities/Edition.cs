using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class Edition
    {
        public int Id {get; set;}
        public int? PublicationYear {get; set;}

        public string ISBN {get; set;} = string.Empty;

        public string? CoverImageUrl { get; set; } 
        
        public int? PageCount { get; set; }

        
        public string Language { get; set; } = "English";

        public string? Format { get; set; } 

        public int BookId { get; set; }
        
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

        public Book Book { get; set; } = null!;
        
        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}