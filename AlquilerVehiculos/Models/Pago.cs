using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdContrato { get; set; }

    public int IdMetodoPago { get; set; }

    public int IdUsuario { get; set; }

    public decimal Monto { get; set; }

    public string? Concepto { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual MetodoPago IdMetodoPagoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
