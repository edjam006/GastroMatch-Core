using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroMatch_Core.Models
{
    public class Plato
    {
        [Key]
        public int IdPlato { get; set; }

        [Required]
        [StringLength(150)]
        public string NombrePlato { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        [Range(0, 10000)]
        public int Calorias { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public int RestauranteId { get; set; }

        // Relación N-a-1 con Restaurante
        [ForeignKey("RestauranteId")]
        public virtual Restaurante Restaurante { get; set; } = null!;

        // Relación Muchos-a-Muchos con Ingrediente
        public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();

        // Relación Muchos-a-Muchos con Pedido
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
