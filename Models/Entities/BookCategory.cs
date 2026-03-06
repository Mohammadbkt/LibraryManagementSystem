using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class BookCategory
    {
        public int BookId {get; set;}
        public int CategoryId {get; set;}

        
        public Book Book {get; set;} = null!;
        public Category Category {get; set;} = null!;
    }
}