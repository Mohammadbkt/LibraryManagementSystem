using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class Author
    {
        public int Id {get; set;}

        public string FullName {get; set;} =string.Empty;

        public string? Biography {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

        public ICollection<BookAuthor> BookAuthors {get; set;} = new List<BookAuthor>();


    }
}