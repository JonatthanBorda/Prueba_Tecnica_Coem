--								Script de creación de tablas (DDL)

-- Se verifica si la base de datos ya existe y se elimina:
IF DB_ID('db_Portal_Coem') IS NOT NULL
DROP DATABASE db_Portal_Coem;
GO

-- Creación de la base de datos
CREATE DATABASE db_Portal_Coem;
GO

USE db_Portal_Coem;
GO

-- Creación de tabla TipoUsuario
CREATE TABLE TipoUsuario (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL
);
GO

-- Creación de tabla Usuarios
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Clave NVARCHAR(50) NULL,
    IdTipoUsuario INT NOT NULL,
    FOREIGN KEY (IdTipoUsuario) REFERENCES TipoUsuario(Id)
);
GO

-- Creación de tabla NivelEducativo
CREATE TABLE NivelEducativo (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Nivel NVARCHAR(255) NOT NULL
);
GO

-- Creación de tabla Demandantes
CREATE TABLE Demandantes (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    IdUsuario INT NOT NULL,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    FechaNacimiento DATETIME NOT NULL,
    Celular NVARCHAR(10) NOT NULL,
    IdNivelEducativo INT NOT NULL,
    Notas NVARCHAR(MAX),
    ExperienciaAnterior NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id),
    FOREIGN KEY (IdNivelEducativo) REFERENCES NivelEducativo(Id)
);
GO

-- Creación de la tabla Empleadores
CREATE TABLE Empleadores (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    IdUsuario INT NOT NULL,
    RazonSocial NVARCHAR(255) NOT NULL,
    Localizacion NVARCHAR(255) NOT NULL,
    Industria NVARCHAR(100) NOT NULL,
    NumeroEmpleados INT NOT NULL,
    FOREIGN KEY (IdUsuario) REFERENCES Usuarios(Id)
);
GO

-- Creación de tabla Vacantes
CREATE TABLE Vacantes (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    IdEmpleador INT NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    Requisitos NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (IdEmpleador) REFERENCES Empleadores(Id)
);
GO

-- Creación de tabla EstadoAplicacion
CREATE TABLE EstadoAplicacion (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    Estado NVARCHAR(255) NOT NULL
);
GO

-- Creación de la tabla Aplicaciones
CREATE TABLE Aplicaciones (
    Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    IdVacante INT NOT NULL,
    IdDemandante INT NOT NULL,
    FechaAplicacion DATETIME NOT NULL DEFAULT GETDATE(),
    IdEstado INT NOT NULL,
    FOREIGN KEY (IdVacante) REFERENCES Vacantes(Id),
    FOREIGN KEY (IdDemandante) REFERENCES Demandantes(Id),
    FOREIGN KEY (IdEstado) REFERENCES EstadoAplicacion(Id)
);
GO

--								Script de inserción de datos (DML)

-- Tabla TipoUsuario
INSERT INTO TipoUsuario (Tipo) VALUES ('Demandante'), ('Empleador');

-- Tabla NivelEducativo
INSERT INTO NivelEducativo (Nivel) VALUES ('Sin estudios formales'), ('Primaria/Bachillerato'), ('Profesional/Técnico/Tecnólogo'), ('Pregrado');

-- Tabla EstadoAplicacion
INSERT INTO EstadoAplicacion (Estado) VALUES ('Enviada'), ('Vista'), ('En proceso'), ('Finalizada');