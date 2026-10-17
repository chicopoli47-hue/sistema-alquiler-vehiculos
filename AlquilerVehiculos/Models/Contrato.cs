using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Contrato
{
    public int IdContrato { get; set; }

    public int? IdReserva { get; set; }

    public int IdCliente { get; set; }

    public int IdVehiculo { get; set; }

    public int IdSucursal { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFinPactada { get; set; }

    public decimal PrecioDiaAplicado { get; set; }

    public int DiasPactados { get; set; }

    public decimal TotalAlquiler { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Cargo> Cargo { get; set; } = new List<Cargo>();

    public virtual ICollection<Dano> Dano { get; set; } = new List<Dano>();

    public virtual Deposito? Deposito { get; set; }

    public virtual ICollection<EntregaDevolucion> EntregaDevolucion { get; set; } = new List<EntregaDevolucion>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Reserva? IdReservaNavigation { get; set; }

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual Vehiculo IdVehiculoNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();
}
