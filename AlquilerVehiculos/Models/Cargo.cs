using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Cargo
{
    public int IdCargo { get; set; }

    public int IdContrato { get; set; }

    public int IdTipoCargo { get; set; }

    public string? Descripcion { get; set; }

    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; }

    public virtual ICollection<Dano> Dano { get; set; } = new List<Dano>();

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

    public virtual TipoCargo IdTipoCargoNavigation { get; set; } = null!;
}
