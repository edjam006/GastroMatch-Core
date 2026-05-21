using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroMatch_Core.Models
{
    public class PreferenciaUsuario
    {
        [Key]
        public int IdPref { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoCocinaFavorita { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RangoPrecioMax { get; set; }

        [StringLength(255)]
        public string? Alergias { get; set; }

        [StringLength(255)]
        public string? RestriccionesDieteticas { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        // Relación 1-a-1 con Usuario
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
