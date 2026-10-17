using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Auditoria> Auditoria { get; set; } = new List<Auditoria>();

    public virtual ICollection<Contrato> Contrato { get; set; } = new List<Contrato>();

    public virtual ICollection<Dano> Dano { get; set; } = new List<Dano>();

    public virtual ICollection<EntregaDevolucion> EntregaDevolucion { get; set; } = new List<EntregaDevolucion>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();
}
