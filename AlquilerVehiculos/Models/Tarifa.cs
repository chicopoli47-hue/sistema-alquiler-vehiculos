using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Tarifa
{
    public int IdTarifa { get; set; }

    public int IdCategoria { get; set; }

    public decimal PrecioDia { get; set; }

    public bool Activa { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;
}
