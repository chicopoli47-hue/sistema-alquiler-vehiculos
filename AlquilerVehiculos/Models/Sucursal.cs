using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Sucursal
{
    public int IdSucursal { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public bool Activa { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; } = new List<Contrato>();

    public virtual ICollection<EntregaDevolucion> EntregaDevolucion { get; set; } = new List<EntregaDevolucion>();

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();
}
