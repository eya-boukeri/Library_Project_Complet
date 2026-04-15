using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.DAL.Models
{
    public class Borrowing
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required, MaxLength(200)]
        public string BorrowerName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? BorrowerEmail { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReturnDate { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsReturned { get; set; } = false;

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation property
        [ForeignKey("BookId")]
        public Book? Book { get; set; }
    }
}
