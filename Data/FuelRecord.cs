using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeuProjetoBlazorServer.Data
{
    public class FuelRecord
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Column(TypeName = "numeric")]
        public decimal Odometer { get; set; }
        
        [Required]
        [Column(TypeName = "numeric")]
        public decimal FuelAmount { get; set; }
        
        [Required]
        [Column(TypeName = "numeric")]
        public decimal FuelPricePerUnit { get; set; }
        
        [Required]
        [Column(TypeName = "numeric")]
        public decimal TotalCost { get; set; }
        
        [Required]
        [StringLength(20)]
        public string FuelType { get; set; } = string.Empty; // Gasolina, Alcool, Diesel
        
        [Required]
        public bool FullTank { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public int VehicleId { get; set; }
        
        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; } = null!;
        
        public bool IsDeleted { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
