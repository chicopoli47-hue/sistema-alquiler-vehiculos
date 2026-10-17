using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class ParametroSistema
{
    public string Clave { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public string? Descripcion { get; set; }
}
