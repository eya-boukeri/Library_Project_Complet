using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.BL.DTOs
{
    // ─── BOOK DTOs ───────────────────────────────────────────────
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public string? Genre { get; set; }
        public int? PublicationYear { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CoverImageUrl { get; set; }
        public int BorrowingsCount { get; set; }
    }

    public class CreateBookDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [MaxLength(300)]
        [Display(Name = "Titre")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'auteur est obligatoire")]
        [MaxLength(200)]
        [Display(Name = "Auteur")]
        public string Author { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "ISBN")]
        public string? ISBN { get; set; }

        [MaxLength(100)]
        [Display(Name = "Genre")]
        public string? Genre { get; set; }

        [Range(1000, 2100, ErrorMessage = "Année invalide")]
        [Display(Name = "Année de publication")]
        public int? PublicationYear { get; set; }

        [MaxLength(100)]
        [Display(Name = "Éditeur")]
        public string? Publisher { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Image de couverture (URL)")]
        public string? CoverImageUrl { get; set; }
    }

    public class UpdateBookDto : CreateBookDto
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
    }

    // ─── BORROWING DTOs ──────────────────────────────────────────
    public class BorrowingDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public string BorrowerName { get; set; } = string.Empty;
        public string? BorrowerEmail { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
        public string? Notes { get; set; }
        public bool IsOverdue => !IsReturned && DueDate < DateTime.UtcNow;
    }

    public class CreateBorrowingDto
    {
        [Required]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Le nom de l'emprunteur est obligatoire")]
        [MaxLength(200)]
        [Display(Name = "Nom de l'emprunteur")]
        public string BorrowerName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email invalide")]
        [MaxLength(200)]
        [Display(Name = "Email")]
        public string? BorrowerEmail { get; set; }

        [Required(ErrorMessage = "La date de retour est obligatoire")]
        [Display(Name = "Date de retour prévue")]
        public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(14);

        [MaxLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }

    public class UpdateBorrowingDto
    {
        public int Id { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? ReturnDate { get; set; }

        [MaxLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }

    // ─── DASHBOARD DTO ───────────────────────────────────────────
    public class DashboardDto
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int ActiveBorrowings { get; set; }
        public int OverdueBorrowings { get; set; }
        public List<BorrowingDto> RecentBorrowings { get; set; } = new();
        public List<BookDto> RecentBooks { get; set; } = new();
    }
}
