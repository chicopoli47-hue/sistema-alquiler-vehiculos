using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class EntregaDevolucion
{
    public int IdEntregaDevolucion { get; set; }

    public int IdContrato { get; set; }

    public int IdSucursal { get; set; }

    public int IdUsuario { get; set; }

    public string Tipo { get; set; } = null!;

    public DateTime FechaHora { get; set; }

    public int Kilometraje { get; set; }

    public string Combustible { get; set; } = null!;

    public string? Observaciones { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
