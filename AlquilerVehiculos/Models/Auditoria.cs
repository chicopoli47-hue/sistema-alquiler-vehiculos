using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Auditoria
{
    public long IdAuditoria { get; set; }

    public int? IdUsuario { get; set; }

    public string Accion { get; set; } = null!;

    public string TablaAfectada { get; set; } = null!;

    public string? IdRegistro { get; set; }

    public string? ValorAnterior { get; set; }

    public string? ValorNuevo { get; set; }

    public string? Descripcion { get; set; }

    public DateTime FechaHora { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
