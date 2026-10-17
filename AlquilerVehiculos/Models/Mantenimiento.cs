using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Mantenimiento
{
    public int IdMantenimiento { get; set; }

    public int IdVehiculo { get; set; }

    public string Tipo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string? Taller { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public decimal Costo { get; set; }

    public int? Kilometraje { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;
}
