using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class MetodoPago
{
    public int IdMetodoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Deposito> Deposito { get; set; } = new List<Deposito>();

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
