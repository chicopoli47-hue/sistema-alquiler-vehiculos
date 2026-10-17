using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int IdCliente { get; set; }

    public int IdCategoria { get; set; }

    public int? IdVehiculo { get; set; }

    public int IdSucursal { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string Estado { get; set; } = null!;

    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Contrato? Contrato { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual Vehiculo? IdVehiculoNavigation { get; set; }
}
