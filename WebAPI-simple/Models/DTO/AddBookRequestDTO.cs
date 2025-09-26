using WebAPI_simple.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.DTO
{
    public class AddBookRequestDTO
    {
        [Required]
        [MinLength(10)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Book Title cannot contain special characters")]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }

        [Range(0,5,ErrorMessage = "From 0 to 5")]
        public int? Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public int PublisherID { get; set; }
        public List<int> AuthorIds { get; set; }
    }
}
