using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Vehiculo
{
    public int IdVehiculo { get; set; }

    public int IdCategoria { get; set; }

    public string Matricula { get; set; } = null!;

    public string Marca { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public short Anio { get; set; }

    public int Kilometraje { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Contrato? Contrato { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimiento { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();
}
