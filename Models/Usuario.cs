using System.ComponentModel.DataAnnotations;

namespace GastroMatch_Core.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } = "RestauranteManager";

        public decimal? Latitud { get; set; }

        public decimal? Longitud { get; set; }

        public string? HistorialBusqueda { get; set; }

        // Relación 1-a-1 con PreferenciaUsuario
        public virtual PreferenciaUsuario? Preferencia { get; set; }

        // Relación 1-a-N con Pedido
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
