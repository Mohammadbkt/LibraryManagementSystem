using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class BookAuthor
    {
        public int BookId {get; set;}
        public int AuthorId {get; set;}
        public int AuthorOrder { get; set; }

        public Book Book {get; set;} = null!;
        public Author Author {get; set;} = null!;
    }
}