using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Dano
{
    public int IdDano { get; set; }

    public int IdContrato { get; set; }

    public int IdUsuario { get; set; }

    public int? IdCargo { get; set; }

    public string Descripcion { get; set; } = null!;

    public decimal Costo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Cargo? IdCargoNavigation { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
