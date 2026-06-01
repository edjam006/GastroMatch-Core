using System.ComponentModel.DataAnnotations;

namespace GastroMatch_Core.Models
{
    public class Restaurante
    {
        [Key]
        public int IdRest { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreLocal { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Direccion { get; set; } = string.Empty;

        [Range(0, 5)]
        public double Calificacion { get; set; }

        [Required]
        public TimeSpan HorarioApertura { get; set; }

        [Required]
        public TimeSpan HorarioCierre { get; set; }

        public decimal? Latitud { get; set; }

        public decimal? Longitud { get; set; }

        public string TipoCocina { get; set; } = string.Empty;

        // Relación 1-a-N con Plato
        public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();
    }
}
