using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Deposito
{
    public int IdDeposito { get; set; }

    public int IdContrato { get; set; }

    public int IdMetodoPago { get; set; }

    public decimal Monto { get; set; }

    public decimal MontoAplicado { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public DateTime? FechaResolucion { get; set; }

    public string? Observaciones { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual MetodoPago IdMetodoPagoNavigation { get; set; } = null!;
}
