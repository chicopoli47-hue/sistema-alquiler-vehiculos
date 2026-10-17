using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class VwSaldoContrato
{
    public int IdContrato { get; set; }

    public decimal TotalAlquiler { get; set; }

    public decimal TotalCargos { get; set; }

    public decimal TotalPagado { get; set; }

    public decimal? Saldo { get; set; }
}
