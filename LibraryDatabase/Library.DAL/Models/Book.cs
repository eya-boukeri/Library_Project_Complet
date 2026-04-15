using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.DAL.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Author { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ISBN { get; set; }

        [MaxLength(100)]
        public string? Genre { get; set; }

        public int? PublicationYear { get; set; }

        [MaxLength(100)]
        public string? Publisher { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? CoverImageUrl { get; set; }

        // Navigation property
        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    }
}
