/* =====================================================================
   SISTEMA DE ALQUILER DE VEHICULOS - MVP
   Motor: SQL Server
   Notas de diseño:
   - Estados fijos mediante CHECK (la lógica del sistema depende de ellos).
   - Catalogos editables: tipo_cargo, metodo_pago, sucursal, categoria.
   - Tarifa: precio por dia por categoria, IMPUESTO INCLUIDO.
   - Disponibilidad NO se guarda: se calcula (fn_vehiculos_disponibles).
   - Multa por retraso = cargo de tipo 'Retraso' (se calcula en la aplicacion
     usando los parametros TOLERANCIA_RETRASO_MINUTOS y PORCENTAJE_MULTA_DIA).
   ===================================================================== */

IF DB_ID('AlquilerVehiculos') IS NULL
    CREATE DATABASE AlquilerVehiculos;
GO

USE AlquilerVehiculos;
GO

/* ========================= ACCESO ========================= */

CREATE TABLE rol (
    id_rol      INT IDENTITY(1,1) CONSTRAINT PK_rol PRIMARY KEY,
    nombre      NVARCHAR(30) NOT NULL CONSTRAINT UQ_rol_nombre UNIQUE
);

CREATE TABLE usuario (
    id_usuario       INT IDENTITY(1,1) CONSTRAINT PK_usuario PRIMARY KEY,
    id_rol           INT NOT NULL CONSTRAINT FK_usuario_rol REFERENCES rol(id_rol),
    nombre_usuario   NVARCHAR(50)  NOT NULL CONSTRAINT UQ_usuario_nombre UNIQUE,
    nombre_completo  NVARCHAR(100) NOT NULL,
    contrasena_hash  NVARCHAR(255) NOT NULL,   -- nunca guardar la contraseña en texto plano
    activo           BIT NOT NULL CONSTRAINT DF_usuario_activo DEFAULT 1,
    fecha_creacion   DATETIME2 NOT NULL CONSTRAINT DF_usuario_fecha DEFAULT SYSDATETIME()
);

/* ========================= CATALOGOS ========================= */

CREATE TABLE sucursal (
    id_sucursal  INT IDENTITY(1,1) CONSTRAINT PK_sucursal PRIMARY KEY,
    nombre       NVARCHAR(80)  NOT NULL CONSTRAINT UQ_sucursal_nombre UNIQUE,
    direccion    NVARCHAR(200) NULL,
    telefono     NVARCHAR(20)  NULL,
    activa       BIT NOT NULL CONSTRAINT DF_sucursal_activa DEFAULT 1
);

CREATE TABLE categoria (
    id_categoria  INT IDENTITY(1,1) CONSTRAINT PK_categoria PRIMARY KEY,
    nombre        NVARCHAR(50)  NOT NULL CONSTRAINT UQ_categoria_nombre UNIQUE,
    descripcion   NVARCHAR(200) NULL
);

-- Precio por dia por categoria, impuesto incluido. Una sola tarifa activa por categoria.
CREATE TABLE tarifa (
    id_tarifa     INT IDENTITY(1,1) CONSTRAINT PK_tarifa PRIMARY KEY,
    id_categoria  INT NOT NULL CONSTRAINT FK_tarifa_categoria REFERENCES categoria(id_categoria),
    precio_dia    DECIMAL(10,2) NOT NULL CONSTRAINT CK_tarifa_precio CHECK (precio_dia > 0),
    activa        BIT NOT NULL CONSTRAINT DF_tarifa_activa DEFAULT 1
);
CREATE UNIQUE INDEX UX_tarifa_activa_por_categoria
    ON tarifa(id_categoria) WHERE activa = 1;

CREATE TABLE tipo_cargo (
    id_tipo_cargo  INT IDENTITY(1,1) CONSTRAINT PK_tipo_cargo PRIMARY KEY,
    nombre         NVARCHAR(50)  NOT NULL CONSTRAINT UQ_tipo_cargo_nombre UNIQUE,
    descripcion    NVARCHAR(200) NULL,
    monto_fijo     DECIMAL(10,2) NULL CONSTRAINT CK_tipo_cargo_monto CHECK (monto_fijo IS NULL OR monto_fijo >= 0),
    activo         BIT NOT NULL CONSTRAINT DF_tipo_cargo_activo DEFAULT 1
);

CREATE TABLE metodo_pago (
    id_metodo_pago  INT IDENTITY(1,1) CONSTRAINT PK_metodo_pago PRIMARY KEY,
    nombre          NVARCHAR(40) NOT NULL CONSTRAINT UQ_metodo_pago_nombre UNIQUE,
    activo          BIT NOT NULL CONSTRAINT DF_metodo_pago_activo DEFAULT 1
);

-- Valores configurables del sistema (tolerancia, % de multa, moneda...)
CREATE TABLE parametro_sistema (
    clave        NVARCHAR(50)  NOT NULL CONSTRAINT PK_parametro PRIMARY KEY,
    valor        NVARCHAR(100) NOT NULL,
    descripcion  NVARCHAR(200) NULL
);

/* ========================= CLIENTES Y FLOTA ========================= */

CREATE TABLE cliente (
    id_cliente            INT IDENTITY(1,1) CONSTRAINT PK_cliente PRIMARY KEY,
    nombre                NVARCHAR(60) NOT NULL,
    apellido              NVARCHAR(60) NOT NULL,
    telefono              NVARCHAR(20) NOT NULL,
    documento             NVARCHAR(30) NOT NULL CONSTRAINT UQ_cliente_documento UNIQUE,  -- cedula
    licencia_numero       NVARCHAR(30) NOT NULL,
    licencia_vencimiento  DATE NOT NULL,
    estado                NVARCHAR(10) NOT NULL CONSTRAINT DF_cliente_estado DEFAULT 'activo'
                          CONSTRAINT CK_cliente_estado CHECK (estado IN ('activo','inactivo')),
    fecha_registro        DATETIME2 NOT NULL CONSTRAINT DF_cliente_fecha DEFAULT SYSDATETIME()
);

CREATE TABLE vehiculo (
    id_vehiculo   INT IDENTITY(1,1) CONSTRAINT PK_vehiculo PRIMARY KEY,
    id_categoria  INT NOT NULL CONSTRAINT FK_vehiculo_categoria REFERENCES categoria(id_categoria),
    matricula     NVARCHAR(15) NOT NULL CONSTRAINT UQ_vehiculo_matricula UNIQUE,
    marca         NVARCHAR(40) NOT NULL,
    modelo        NVARCHAR(40) NOT NULL,
    anio          SMALLINT NOT NULL CONSTRAINT CK_vehiculo_anio CHECK (anio BETWEEN 1990 AND 2100),
    kilometraje   INT NOT NULL CONSTRAINT DF_vehiculo_km DEFAULT 0
                  CONSTRAINT CK_vehiculo_km CHECK (kilometraje >= 0),
    estado        NVARCHAR(20) NOT NULL CONSTRAINT DF_vehiculo_estado DEFAULT 'activo'
                  CONSTRAINT CK_vehiculo_estado CHECK (estado IN ('activo','en_mantenimiento','fuera_de_servicio'))
);

CREATE TABLE mantenimiento (
    id_mantenimiento  INT IDENTITY(1,1) CONSTRAINT PK_mantenimiento PRIMARY KEY,
    id_vehiculo       INT NOT NULL CONSTRAINT FK_mant_vehiculo REFERENCES vehiculo(id_vehiculo),
    tipo              NVARCHAR(12) NOT NULL CONSTRAINT CK_mant_tipo CHECK (tipo IN ('preventivo','correctivo')),
    descripcion       NVARCHAR(300) NOT NULL,
    taller            NVARCHAR(100) NULL,
    fecha_inicio      DATE NOT NULL,
    fecha_fin         DATE NULL,
    costo             DECIMAL(10,2) NOT NULL CONSTRAINT DF_mant_costo DEFAULT 0
                      CONSTRAINT CK_mant_costo CHECK (costo >= 0),
    kilometraje       INT NULL CONSTRAINT CK_mant_km CHECK (kilometraje IS NULL OR kilometraje >= 0),
    estado            NVARCHAR(12) NOT NULL CONSTRAINT DF_mant_estado DEFAULT 'en_curso'
                      CONSTRAINT CK_mant_estado CHECK (estado IN ('en_curso','completado')),
    CONSTRAINT CK_mant_fechas CHECK (fecha_fin IS NULL OR fecha_fin >= fecha_inicio)
);

/* ========================= OPERACION ========================= */

-- Reserva por CATEGORIA; el vehiculo es opcional (se puede asignar antes de la entrega)
CREATE TABLE reserva (
    id_reserva      INT IDENTITY(1,1) CONSTRAINT PK_reserva PRIMARY KEY,
    id_cliente      INT NOT NULL CONSTRAINT FK_reserva_cliente   REFERENCES cliente(id_cliente),
    id_categoria    INT NOT NULL CONSTRAINT FK_reserva_categoria REFERENCES categoria(id_categoria),
    id_vehiculo     INT NULL     CONSTRAINT FK_reserva_vehiculo  REFERENCES vehiculo(id_vehiculo),
    id_sucursal     INT NOT NULL CONSTRAINT FK_reserva_sucursal  REFERENCES sucursal(id_sucursal),
    id_usuario      INT NOT NULL CONSTRAINT FK_reserva_usuario   REFERENCES usuario(id_usuario),
    fecha_inicio    DATETIME2 NOT NULL,
    fecha_fin       DATETIME2 NOT NULL,
    estado          NVARCHAR(12) NOT NULL CONSTRAINT DF_reserva_estado DEFAULT 'confirmada'
                    CONSTRAINT CK_reserva_estado CHECK (estado IN ('confirmada','cancelada','convertida')),
    observaciones   NVARCHAR(300) NULL,
    fecha_creacion  DATETIME2 NOT NULL CONSTRAINT DF_reserva_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT CK_reserva_fechas CHECK (fecha_fin > fecha_inicio)
);

-- El contrato guarda cliente y vehiculo directamente; la reserva es OPCIONAL
CREATE TABLE contrato (
    id_contrato        INT IDENTITY(1,1) CONSTRAINT PK_contrato PRIMARY KEY,
    id_reserva         INT NULL     CONSTRAINT FK_contrato_reserva  REFERENCES reserva(id_reserva),
    id_cliente         INT NOT NULL CONSTRAINT FK_contrato_cliente  REFERENCES cliente(id_cliente),
    id_vehiculo        INT NOT NULL CONSTRAINT FK_contrato_vehiculo REFERENCES vehiculo(id_vehiculo),
    id_sucursal        INT NOT NULL CONSTRAINT FK_contrato_sucursal REFERENCES sucursal(id_sucursal),
    id_usuario         INT NOT NULL CONSTRAINT FK_contrato_usuario  REFERENCES usuario(id_usuario),
    fecha_inicio       DATETIME2 NOT NULL,
    fecha_fin_pactada  DATETIME2 NOT NULL,
    precio_dia_aplicado DECIMAL(10,2) NOT NULL   -- precio congelado al crear el contrato
                       CONSTRAINT CK_contrato_precio CHECK (precio_dia_aplicado > 0),
    dias_pactados      INT NOT NULL CONSTRAINT CK_contrato_dias CHECK (dias_pactados > 0),
    total_alquiler     DECIMAL(12,2) NOT NULL CONSTRAINT CK_contrato_total CHECK (total_alquiler >= 0),
    estado             NVARCHAR(10) NOT NULL CONSTRAINT DF_contrato_estado DEFAULT 'abierto'
                       CONSTRAINT CK_contrato_estado CHECK (estado IN ('abierto','cerrado')),
    fecha_creacion     DATETIME2 NOT NULL CONSTRAINT DF_contrato_fecha DEFAULT SYSDATETIME(),
    CONSTRAINT CK_contrato_fechas CHECK (fecha_fin_pactada > fecha_inicio)
);
-- Una reserva solo puede convertirse en un contrato
CREATE UNIQUE INDEX UX_contrato_reserva ON contrato(id_reserva) WHERE id_reserva IS NOT NULL;
-- Un vehiculo no puede tener dos contratos abiertos a la vez
CREATE UNIQUE INDEX UX_contrato_vehiculo_abierto ON contrato(id_vehiculo) WHERE estado = 'abierto';

-- Un contrato tiene como maximo una entrega y una devolucion
CREATE TABLE entrega_devolucion (
    id_entrega_devolucion INT IDENTITY(1,1) CONSTRAINT PK_entrega_dev PRIMARY KEY,
    id_contrato   INT NOT NULL CONSTRAINT FK_entdev_contrato REFERENCES contrato(id_contrato),
    id_sucursal   INT NOT NULL CONSTRAINT FK_entdev_sucursal REFERENCES sucursal(id_sucursal),
    id_usuario    INT NOT NULL CONSTRAINT FK_entdev_usuario  REFERENCES usuario(id_usuario),
    tipo          NVARCHAR(10) NOT NULL CONSTRAINT CK_entdev_tipo CHECK (tipo IN ('entrega','devolucion')),
    fecha_hora    DATETIME2 NOT NULL CONSTRAINT DF_entdev_fecha DEFAULT SYSDATETIME(),
    kilometraje   INT NOT NULL CONSTRAINT CK_entdev_km CHECK (kilometraje >= 0),
    combustible   NVARCHAR(10) NOT NULL CONSTRAINT CK_entdev_comb
                  CHECK (combustible IN ('vacio','1/4','1/2','3/4','lleno')),
    observaciones NVARCHAR(500) NULL,   -- aqui va el checklist / estado general en texto
    CONSTRAINT UQ_entdev_contrato_tipo UNIQUE (id_contrato, tipo)
);

/* ========================= DINERO E INCIDENTES ========================= */

-- Incluye: retraso, dano, combustible, limpieza, etc. (segun tipo_cargo)
CREATE TABLE cargo (
    id_cargo       INT IDENTITY(1,1) CONSTRAINT PK_cargo PRIMARY KEY,
    id_contrato    INT NOT NULL CONSTRAINT FK_cargo_contrato REFERENCES contrato(id_contrato),
    id_tipo_cargo  INT NOT NULL CONSTRAINT FK_cargo_tipo     REFERENCES tipo_cargo(id_tipo_cargo),
    descripcion    NVARCHAR(200) NULL,
    monto          DECIMAL(10,2) NOT NULL CONSTRAINT CK_cargo_monto CHECK (monto > 0),
    fecha          DATETIME2 NOT NULL CONSTRAINT DF_cargo_fecha DEFAULT SYSDATETIME()
);

CREATE TABLE dano (
    id_dano        INT IDENTITY(1,1) CONSTRAINT PK_dano PRIMARY KEY,
    id_contrato    INT NOT NULL CONSTRAINT FK_dano_contrato REFERENCES contrato(id_contrato),
    id_usuario     INT NOT NULL CONSTRAINT FK_dano_usuario  REFERENCES usuario(id_usuario),
    id_cargo       INT NULL     CONSTRAINT FK_dano_cargo    REFERENCES cargo(id_cargo),  -- cargo generado, si se cobra
    descripcion    NVARCHAR(300) NOT NULL,
    costo          DECIMAL(10,2) NOT NULL CONSTRAINT DF_dano_costo DEFAULT 0
                   CONSTRAINT CK_dano_costo CHECK (costo >= 0),
    fecha_registro DATETIME2 NOT NULL CONSTRAINT DF_dano_fecha DEFAULT SYSDATETIME()
);

-- Garantia retenida. Un deposito por contrato.
CREATE TABLE deposito (
    id_deposito       INT IDENTITY(1,1) CONSTRAINT PK_deposito PRIMARY KEY,
    id_contrato       INT NOT NULL CONSTRAINT FK_deposito_contrato REFERENCES contrato(id_contrato)
                      CONSTRAINT UQ_deposito_contrato UNIQUE,
    id_metodo_pago    INT NOT NULL CONSTRAINT FK_deposito_metodo REFERENCES metodo_pago(id_metodo_pago),
    monto             DECIMAL(10,2) NOT NULL CONSTRAINT CK_deposito_monto CHECK (monto > 0),
    monto_aplicado    DECIMAL(10,2) NOT NULL CONSTRAINT DF_deposito_aplicado DEFAULT 0,  -- usado para cargos
    estado            NVARCHAR(10) NOT NULL CONSTRAINT DF_deposito_estado DEFAULT 'retenido'
                      CONSTRAINT CK_deposito_estado CHECK (estado IN ('retenido','aplicado','devuelto')),
    fecha             DATETIME2 NOT NULL CONSTRAINT DF_deposito_fecha DEFAULT SYSDATETIME(),
    fecha_resolucion  DATETIME2 NULL,
    observaciones     NVARCHAR(200) NULL,
    CONSTRAINT CK_deposito_aplicado CHECK (monto_aplicado >= 0 AND monto_aplicado <= monto)
);

CREATE TABLE pago (
    id_pago         INT IDENTITY(1,1) CONSTRAINT PK_pago PRIMARY KEY,
    id_contrato     INT NOT NULL CONSTRAINT FK_pago_contrato REFERENCES contrato(id_contrato),
    id_metodo_pago  INT NOT NULL CONSTRAINT FK_pago_metodo   REFERENCES metodo_pago(id_metodo_pago),
    id_usuario      INT NOT NULL CONSTRAINT FK_pago_usuario  REFERENCES usuario(id_usuario),
    monto           DECIMAL(12,2) NOT NULL CONSTRAINT CK_pago_monto CHECK (monto > 0),
    concepto        NVARCHAR(100) NULL,
    fecha           DATETIME2 NOT NULL CONSTRAINT DF_pago_fecha DEFAULT SYSDATETIME()
);

/* ========================= AUDITORIA ========================= */

CREATE TABLE auditoria (
    id_auditoria    BIGINT IDENTITY(1,1) CONSTRAINT PK_auditoria PRIMARY KEY,
    id_usuario      INT NULL CONSTRAINT FK_auditoria_usuario REFERENCES usuario(id_usuario),
    accion          NVARCHAR(30)  NOT NULL,      -- ej. INSERT, UPDATE, DELETE, LOGIN
    tabla_afectada  NVARCHAR(50)  NOT NULL,
    id_registro     NVARCHAR(50)  NULL,
    valor_anterior  NVARCHAR(MAX) NULL,
    valor_nuevo     NVARCHAR(MAX) NULL,
    descripcion     NVARCHAR(300) NULL,
    fecha_hora      DATETIME2 NOT NULL CONSTRAINT DF_auditoria_fecha DEFAULT SYSDATETIME()
);
GO

-- La auditoria es de solo insercion: no se puede modificar ni borrar
CREATE TRIGGER trg_auditoria_inmutable
ON auditoria
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    RAISERROR('La tabla auditoria es de solo insercion.', 16, 1);
END;
GO

/* ========================= INDICES DE APOYO ========================= */

CREATE INDEX IX_reserva_fechas      ON reserva(id_categoria, estado, fecha_inicio, fecha_fin);
CREATE INDEX IX_reserva_cliente     ON reserva(id_cliente);
CREATE INDEX IX_contrato_cliente    ON contrato(id_cliente);
CREATE INDEX IX_cargo_contrato      ON cargo(id_contrato);
CREATE INDEX IX_pago_contrato       ON pago(id_contrato);
CREATE INDEX IX_mant_vehiculo       ON mantenimiento(id_vehiculo, estado);
CREATE INDEX IX_auditoria_tabla     ON auditoria(tabla_afectada, id_registro);
CREATE INDEX IX_auditoria_fecha     ON auditoria(fecha_hora);
GO

/* ========================= DATOS INICIALES ========================= */

INSERT INTO rol (nombre) VALUES ('Administrador'), ('Empleado');

INSERT INTO tipo_cargo (nombre, descripcion, monto_fijo) VALUES
    ('Retraso',      'Multa por devolucion tardia (se calcula por dia extra)', NULL),
    ('Dano',         'Cobro por danos al vehiculo',                            NULL),
    ('Combustible',  'Reposicion de combustible faltante',                     NULL),
    ('Limpieza',     'Limpieza extraordinaria del vehiculo',                   NULL),
    ('Otro',         'Otros cargos',                                           NULL);

INSERT INTO metodo_pago (nombre) VALUES ('Efectivo'), ('Tarjeta'), ('Transferencia');

INSERT INTO parametro_sistema (clave, valor, descripcion) VALUES
    ('TOLERANCIA_RETRASO_MINUTOS', '60',  'Minutos de gracia despues de la hora pactada'),
    ('PORCENTAJE_MULTA_DIA',       '100', '% de la tarifa diaria del contrato por cada dia o fraccion extra'),
    ('MONEDA',                     'DOP', 'Moneda del sistema'),
    ('IMPUESTO_INCLUIDO',          'SI',  'Las tarifas ya incluyen impuesto');
GO

/* ========================= VISTA Y FUNCION ========================= */

-- Saldo pendiente por contrato = alquiler + cargos - pagos
CREATE VIEW vw_saldo_contrato
AS
SELECT  c.id_contrato,
        c.total_alquiler,
        ISNULL(ca.total, 0) AS total_cargos,
        ISNULL(p.total, 0)  AS total_pagado,
        c.total_alquiler + ISNULL(ca.total, 0) - ISNULL(p.total, 0) AS saldo
FROM contrato c
LEFT JOIN (SELECT id_contrato, SUM(monto) AS total FROM cargo GROUP BY id_contrato) ca
       ON ca.id_contrato = c.id_contrato
LEFT JOIN (SELECT id_contrato, SUM(monto) AS total FROM pago  GROUP BY id_contrato) p
       ON p.id_contrato  = c.id_contrato;
GO

-- Vehiculos libres de una categoria para un rango de fechas.
-- Excluye: vehiculos no activos, con contrato abierto, con reserva confirmada
-- ya asignada, o con mantenimiento en curso que se solape.
-- Nota: las reservas por categoria SIN vehiculo asignado se deben descontar aparte
-- (cantidad de vehiculos libres - reservas confirmadas de esa categoria en el rango).
CREATE FUNCTION dbo.fn_vehiculos_disponibles
(
    @inicio       DATETIME2,
    @fin          DATETIME2,
    @id_categoria INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT v.*
    FROM vehiculo v
    WHERE v.id_categoria = @id_categoria
      AND v.estado = 'activo'
      AND NOT EXISTS (SELECT 1 FROM contrato c
                      WHERE c.id_vehiculo = v.id_vehiculo AND c.estado = 'abierto'
                        AND c.fecha_inicio < @fin AND c.fecha_fin_pactada > @inicio)
      AND NOT EXISTS (SELECT 1 FROM reserva r
                      WHERE r.id_vehiculo = v.id_vehiculo AND r.estado = 'confirmada'
                        AND r.fecha_inicio < @fin AND r.fecha_fin > @inicio)
      AND NOT EXISTS (SELECT 1 FROM mantenimiento m
                      WHERE m.id_vehiculo = v.id_vehiculo AND m.estado = 'en_curso'
                        AND m.fecha_inicio < @fin AND (m.fecha_fin IS NULL OR m.fecha_fin > @inicio))
);
GO
