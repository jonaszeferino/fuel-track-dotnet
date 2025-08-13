using System;
using System.ComponentModel.DataAnnotations;

namespace MeuProjetoBlazorServer.Data
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public decimal TankCapacity { get; set; }
        
        [Required]
        public int Year { get; set; }
        
        [StringLength(200)]
        public string? Subtitle { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
}
