using System;
using System.Collections.Generic;
using AlquilerVehiculos.Models;
using Microsoft.EntityFrameworkCore;

namespace AlquilerVehiculos.Data;

public partial class AlquilerDbContext : DbContext
{
    public AlquilerDbContext(DbContextOptions<AlquilerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auditoria> Auditoria { get; set; }

    public virtual DbSet<Cargo> Cargo { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<Contrato> Contrato { get; set; }

    public virtual DbSet<Dano> Dano { get; set; }

    public virtual DbSet<Deposito> Deposito { get; set; }

    public virtual DbSet<EntregaDevolucion> EntregaDevolucion { get; set; }

    public virtual DbSet<Mantenimiento> Mantenimiento { get; set; }

    public virtual DbSet<MetodoPago> MetodoPago { get; set; }

    public virtual DbSet<Pago> Pago { get; set; }

    public virtual DbSet<ParametroSistema> ParametroSistema { get; set; }

    public virtual DbSet<Reserva> Reserva { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Sucursal> Sucursal { get; set; }

    public virtual DbSet<Tarifa> Tarifa { get; set; }

    public virtual DbSet<TipoCargo> TipoCargo { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<Vehiculo> Vehiculo { get; set; }

    public virtual DbSet<VwSaldoContrato> VwSaldoContrato { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(e => e.IdAuditoria);

            entity.ToTable("auditoria", tb => tb.HasTrigger("trg_auditoria_inmutable"));

            entity.HasIndex(e => e.FechaHora, "IX_auditoria_fecha");

            entity.HasIndex(e => new { e.TablaAfectada, e.IdRegistro }, "IX_auditoria_tabla");

            entity.Property(e => e.IdAuditoria).HasColumnName("id_auditoria");
            entity.Property(e => e.Accion)
                .HasMaxLength(30)
                .HasColumnName("accion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("(sysdatetime())", "DF_auditoria_fecha")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.IdRegistro)
                .HasMaxLength(50)
                .HasColumnName("id_registro");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.TablaAfectada)
                .HasMaxLength(50)
                .HasColumnName("tabla_afectada");
            entity.Property(e => e.ValorAnterior).HasColumnName("valor_anterior");
            entity.Property(e => e.ValorNuevo).HasColumnName("valor_nuevo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Auditoria)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_auditoria_usuario");
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.HasKey(e => e.IdCargo);

            entity.ToTable("cargo");

            entity.HasIndex(e => e.IdContrato, "IX_cargo_contrato");

            entity.Property(e => e.IdCargo).HasColumnName("id_cargo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(sysdatetime())", "DF_cargo_fecha")
                .HasColumnName("fecha");
            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.IdTipoCargo).HasColumnName("id_tipo_cargo");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");

            entity.HasOne(d => d.IdContratoNavigation).WithMany(p => p.Cargo)
                .HasForeignKey(d => d.IdContrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cargo_contrato");

            entity.HasOne(d => d.IdTipoCargoNavigation).WithMany(p => p.Cargo)
                .HasForeignKey(d => d.IdTipoCargo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cargo_tipo");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria);

            entity.ToTable("categoria");

            entity.HasIndex(e => e.Nombre, "UQ_categoria_nombre").IsUnique();

            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente);

            entity.ToTable("cliente");

            entity.HasIndex(e => e.Documento, "UQ_cliente_documento").IsUnique();

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Apellido)
                .HasMaxLength(60)
                .HasColumnName("apellido");
            entity.Property(e => e.Documento)
                .HasMaxLength(30)
                .HasColumnName("documento");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .HasDefaultValue("activo", "DF_cliente_estado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(sysdatetime())", "DF_cliente_fecha")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.LicenciaNumero)
                .HasMaxLength(30)
                .HasColumnName("licencia_numero");
            entity.Property(e => e.LicenciaVencimiento).HasColumnName("licencia_vencimiento");
            entity.Property(e => e.Nombre)
                .HasMaxLength(60)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(e => e.IdContrato);

            entity.ToTable("contrato");

            entity.HasIndex(e => e.IdCliente, "IX_contrato_cliente");

            entity.HasIndex(e => e.IdReserva, "UX_contrato_reserva")
                .IsUnique()
                .HasFilter("([id_reserva] IS NOT NULL)");

            entity.HasIndex(e => e.IdVehiculo, "UX_contrato_vehiculo_abierto")
                .IsUnique()
                .HasFilter("([estado]='abierto')");

            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.DiasPactados).HasColumnName("dias_pactados");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .HasDefaultValue("abierto", "DF_contrato_estado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(sysdatetime())", "DF_contrato_fecha")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaFinPactada).HasColumnName("fecha_fin_pactada");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdReserva).HasColumnName("id_reserva");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.PrecioDiaAplicado)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_dia_aplicado");
            entity.Property(e => e.TotalAlquiler)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_alquiler");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Contrato)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_contrato_cliente");

            entity.HasOne(d => d.IdReservaNavigation).WithOne(p => p.Contrato)
                .HasForeignKey<Contrato>(d => d.IdReserva)
                .HasConstraintName("FK_contrato_reserva");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Contrato)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_contrato_sucursal");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Contrato)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_contrato_usuario");

            entity.HasOne(d => d.IdVehiculoNavigation).WithOne(p => p.Contrato)
                .HasForeignKey<Contrato>(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_contrato_vehiculo");
        });

        modelBuilder.Entity<Dano>(entity =>
        {
            entity.HasKey(e => e.IdDano);

            entity.ToTable("dano");

            entity.Property(e => e.IdDano).HasColumnName("id_dano");
            entity.Property(e => e.Costo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("costo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(sysdatetime())", "DF_dano_fecha")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdCargo).HasColumnName("id_cargo");
            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdCargoNavigation).WithMany(p => p.Dano)
                .HasForeignKey(d => d.IdCargo)
                .HasConstraintName("FK_dano_cargo");

            entity.HasOne(d => d.IdContratoNavigation).WithMany(p => p.Dano)
                .HasForeignKey(d => d.IdContrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dano_contrato");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Dano)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_dano_usuario");
        });

        modelBuilder.Entity<Deposito>(entity =>
        {
            entity.HasKey(e => e.IdDeposito);

            entity.ToTable("deposito");

            entity.HasIndex(e => e.IdContrato, "UQ_deposito_contrato").IsUnique();

            entity.Property(e => e.IdDeposito).HasColumnName("id_deposito");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .HasDefaultValue("retenido", "DF_deposito_estado")
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(sysdatetime())", "DF_deposito_fecha")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaResolucion).HasColumnName("fecha_resolucion");
            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.IdMetodoPago).HasColumnName("id_metodo_pago");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.MontoAplicado)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_aplicado");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(200)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.IdContratoNavigation).WithOne(p => p.Deposito)
                .HasForeignKey<Deposito>(d => d.IdContrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_deposito_contrato");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Deposito)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_deposito_metodo");
        });

        modelBuilder.Entity<EntregaDevolucion>(entity =>
        {
            entity.HasKey(e => e.IdEntregaDevolucion).HasName("PK_entrega_dev");

            entity.ToTable("entrega_devolucion");

            entity.HasIndex(e => new { e.IdContrato, e.Tipo }, "UQ_entdev_contrato_tipo").IsUnique();

            entity.Property(e => e.IdEntregaDevolucion).HasColumnName("id_entrega_devolucion");
            entity.Property(e => e.Combustible)
                .HasMaxLength(10)
                .HasColumnName("combustible");
            entity.Property(e => e.FechaHora)
                .HasDefaultValueSql("(sysdatetime())", "DF_entdev_fecha")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Kilometraje).HasColumnName("kilometraje");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500)
                .HasColumnName("observaciones");
            entity.Property(e => e.Tipo)
                .HasMaxLength(10)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdContratoNavigation).WithMany(p => p.EntregaDevolucion)
                .HasForeignKey(d => d.IdContrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entdev_contrato");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.EntregaDevolucion)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entdev_sucursal");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.EntregaDevolucion)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_entdev_usuario");
        });

        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.HasKey(e => e.IdMantenimiento);

            entity.ToTable("mantenimiento");

            entity.HasIndex(e => new { e.IdVehiculo, e.Estado }, "IX_mant_vehiculo");

            entity.Property(e => e.IdMantenimiento).HasColumnName("id_mantenimiento");
            entity.Property(e => e.Costo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("costo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(12)
                .HasDefaultValue("en_curso", "DF_mant_estado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Kilometraje).HasColumnName("kilometraje");
            entity.Property(e => e.Taller)
                .HasMaxLength(100)
                .HasColumnName("taller");
            entity.Property(e => e.Tipo)
                .HasMaxLength(12)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Mantenimiento)
                .HasForeignKey(d => d.IdVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_mant_vehiculo");
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago);

            entity.ToTable("metodo_pago");

            entity.HasIndex(e => e.Nombre, "UQ_metodo_pago_nombre").IsUnique();

            entity.Property(e => e.IdMetodoPago).HasColumnName("id_metodo_pago");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true, "DF_metodo_pago_activo")
                .HasColumnName("activo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(40)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago);

            entity.ToTable("pago");

            entity.HasIndex(e => e.IdContrato, "IX_pago_contrato");

            entity.Property(e => e.IdPago).HasColumnName("id_pago");
            entity.Property(e => e.Concepto)
                .HasMaxLength(100)
                .HasColumnName("concepto");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(sysdatetime())", "DF_pago_fecha")
                .HasColumnName("fecha");
            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.IdMetodoPago).HasColumnName("id_metodo_pago");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("monto");

            entity.HasOne(d => d.IdContratoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdContrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_contrato");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_metodo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pago)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_pago_usuario");
        });

        modelBuilder.Entity<ParametroSistema>(entity =>
        {
            entity.HasKey(e => e.Clave).HasName("PK_parametro");

            entity.ToTable("parametro_sistema");

            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .HasColumnName("clave");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Valor)
                .HasMaxLength(100)
                .HasColumnName("valor");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva);

            entity.ToTable("reserva");

            entity.HasIndex(e => e.IdCliente, "IX_reserva_cliente");

            entity.HasIndex(e => new { e.IdCategoria, e.Estado, e.FechaInicio, e.FechaFin }, "IX_reserva_fechas");

            entity.Property(e => e.IdReserva).HasColumnName("id_reserva");
            entity.Property(e => e.Estado)
                .HasMaxLength(12)
                .HasDefaultValue("confirmada", "DF_reserva_estado")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(sysdatetime())", "DF_reserva_fecha")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(300)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reserva_categoria");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reserva_cliente");

            entity.HasOne(d => d.IdSucursalNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdSucursal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reserva_sucursal");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reserva_usuario");

            entity.HasOne(d => d.IdVehiculoNavigation).WithMany(p => p.Reserva)
                .HasForeignKey(d => d.IdVehiculo)
                .HasConstraintName("FK_reserva_vehiculo");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.ToTable("rol");

            entity.HasIndex(e => e.Nombre, "UQ_rol_nombre").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal);

            entity.ToTable("sucursal");

            entity.HasIndex(e => e.Nombre, "UQ_sucursal_nombre").IsUnique();

            entity.Property(e => e.IdSucursal).HasColumnName("id_sucursal");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true, "DF_sucursal_activa")
                .HasColumnName("activa");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .HasColumnName("direccion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.HasKey(e => e.IdTarifa);

            entity.ToTable("tarifa");

            entity.HasIndex(e => e.IdCategoria, "UX_tarifa_activa_por_categoria")
                .IsUnique()
                .HasFilter("([activa]=(1))");

            entity.Property(e => e.IdTarifa).HasColumnName("id_tarifa");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true, "DF_tarifa_activa")
                .HasColumnName("activa");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.PrecioDia)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_dia");

            entity.HasOne(d => d.IdCategoriaNavigation).WithOne(p => p.Tarifa)
                .HasForeignKey<Tarifa>(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tarifa_categoria");
        });

        modelBuilder.Entity<TipoCargo>(entity =>
        {
            entity.HasKey(e => e.IdTipoCargo);

            entity.ToTable("tipo_cargo");

            entity.HasIndex(e => e.Nombre, "UQ_tipo_cargo_nombre").IsUnique();

            entity.Property(e => e.IdTipoCargo).HasColumnName("id_tipo_cargo");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true, "DF_tipo_cargo_activo")
                .HasColumnName("activo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.MontoFijo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_fijo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("usuario");

            entity.HasIndex(e => e.NombreUsuario, "UQ_usuario_nombre").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true, "DF_usuario_activo")
                .HasColumnName("activo");
            entity.Property(e => e.ContrasenaHash)
                .HasMaxLength(255)
                .HasColumnName("contrasena_hash");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(sysdatetime())", "DF_usuario_fecha")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(100)
                .HasColumnName("nombre_completo");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .HasColumnName("nombre_usuario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_usuario_rol");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.IdVehiculo);

            entity.ToTable("vehiculo");

            entity.HasIndex(e => e.Matricula, "UQ_vehiculo_matricula").IsUnique();

            entity.Property(e => e.IdVehiculo).HasColumnName("id_vehiculo");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("activo", "DF_vehiculo_estado")
                .HasColumnName("estado");
            entity.Property(e => e.IdCategoria).HasColumnName("id_categoria");
            entity.Property(e => e.Kilometraje).HasColumnName("kilometraje");
            entity.Property(e => e.Marca)
                .HasMaxLength(40)
                .HasColumnName("marca");
            entity.Property(e => e.Matricula)
                .HasMaxLength(15)
                .HasColumnName("matricula");
            entity.Property(e => e.Modelo)
                .HasMaxLength(40)
                .HasColumnName("modelo");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Vehiculo)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_vehiculo_categoria");
        });

        modelBuilder.Entity<VwSaldoContrato>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_saldo_contrato");

            entity.Property(e => e.IdContrato).HasColumnName("id_contrato");
            entity.Property(e => e.Saldo)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("saldo");
            entity.Property(e => e.TotalAlquiler)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("total_alquiler");
            entity.Property(e => e.TotalCargos)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_cargos");
            entity.Property(e => e.TotalPagado)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("total_pagado");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
