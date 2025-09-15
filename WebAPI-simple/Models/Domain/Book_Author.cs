﻿using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.Domain
{
    public class Book_Author
    {
        public int Id { get; set; }
        public int BookId { get; set; }

        //navigation properties - one book has many book-author
        public Book Book { get; set; }
        public int AuthorId { get; set; }

        //navigation properties - one author has many book-author
        public Author Author { get; set; }
    }
}
