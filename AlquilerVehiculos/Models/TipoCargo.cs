using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class TipoCargo
{
    public int IdTipoCargo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? MontoFijo { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Cargo> Cargo { get; set; } = new List<Cargo>();
}
