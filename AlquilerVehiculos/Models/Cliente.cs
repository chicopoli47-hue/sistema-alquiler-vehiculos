using System;
using System.Collections.Generic;

namespace AlquilerVehiculos.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public string LicenciaNumero { get; set; } = null!;

    public DateOnly LicenciaVencimiento { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Contrato> Contrato { get; set; } = new List<Contrato>();

    public virtual ICollection<Reserva> Reserva { get; set; } = new List<Reserva>();
}
