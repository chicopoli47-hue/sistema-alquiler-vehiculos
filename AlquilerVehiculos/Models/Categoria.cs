using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();

    public virtual Tarifa? Tarifa { get; set; }

    public virtual ICollection<Vehiculo> Vehiculo { get; set; } = new List<Vehiculo>();
}
