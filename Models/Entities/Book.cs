using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models.Entities
{
    public class Book
    {
        public int Id {get; set;}
        
        public string Title {get; set;} = string.Empty;
        
        public string? Description {get; set;}
        public int? PublisherId {get; set;}
        
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;


        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        public Publisher? Publisher { get; set; }
        public ICollection<Edition> Editions { get; set; } = new List<Edition>();
        
        public ICollection<BookAuthor> BookAuthors {get; set;} = new List<BookAuthor>();
        public ICollection<Review> Reviews {get; set;} = new List<Review>();
        public ICollection<Bookmark> Bookmarks {get; set;} = new List<Bookmark>();


    }
}