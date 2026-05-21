using System.ComponentModel.DataAnnotations;

namespace GastroMatch_Core.Models
{
    public class Ingrediente
    {
        [Key]
        public int IdIngr { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreIngr { get; set; } = string.Empty;

        [Required]
        public bool EsAlergeno { get; set; }

        // Relación Muchos-a-Muchos con Plato
        public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();
    }
}
