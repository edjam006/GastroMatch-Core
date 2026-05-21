using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GastroMatch_Core.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPagar { get; set; }

        [Required]
        public EstadoPedido Estado { get; set; } = EstadoPedido.PENDING;

        [Range(0, 1440)]
        public int TiempoEntregaEstimado { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        // Relación N-a-1 con Usuario
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        // Relación Muchos-a-Muchos con Plato
        public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();
    }
}
