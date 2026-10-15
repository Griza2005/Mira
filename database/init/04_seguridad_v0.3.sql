-- =============================================================================
-- MIRA - Versión 0.3: Seguridad, Autenticación y Roles (RBAC)
-- =============================================================================

-- 1. Tabla de Roles (Permisos del Sistema)
CREATE TABLE IF NOT EXISTS rol (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL UNIQUE,
    descripcion VARCHAR(200),
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_creacion TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 2. Tabla de Usuarios (Credenciales con Hash BCrypt)
CREATE TABLE IF NOT EXISTS usuario (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL, -- Almacena Hash BCrypt (60 caracteres), NUNCA texto plano
    email VARCHAR(100) NOT NULL UNIQUE,
    nombre_completo VARCHAR(150),
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    fecha_creacion TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 3. Tabla Puente Rol - Usuario (Muchos a Muchos)
CREATE TABLE IF NOT EXISTS rol_usuario (
    usuario_id INT NOT NULL REFERENCES usuario(id) ON DELETE RESTRICT,
    rol_id INT NOT NULL REFERENCES rol(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (usuario_id, rol_id)
);

-- =============================================================================
-- SEEDING INICIAL DE SEGURIDAD
-- =============================================================================

-- Roles predeterminados del sistema
INSERT INTO rol (nombre, descripcion, activo)
VALUES 
    ('Administrador', 'Acceso total a la gestión del sistema y administración de usuarios.', TRUE),
    ('Editor', 'Capacidad de formulación, edición y vinculación académica.', TRUE),
    ('Consulta', 'Acceso de solo lectura para reportes e indicadores.', TRUE)
ON CONFLICT (nombre) DO NOTHING;

-- Usuario Inicial: admin (Contraseña en texto plano: admin123)
-- El hash generado con BCrypt compatible: $2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy
INSERT INTO usuario (username, password, email, nombre_completo, activo)
VALUES (
    'admin',
    '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy',
    'admin@usbmed.edu.co',
    'Administrador del Sistema',
    TRUE
)
ON CONFLICT (username) DO NOTHING;

-- Asignación del rol Administrador al usuario admin
INSERT INTO rol_usuario (usuario_id, rol_id, activo)
VALUES (
    (SELECT id FROM usuario WHERE username = 'admin'),
    (SELECT id FROM rol WHERE nombre = 'Administrador'),
    TRUE
)
ON CONFLICT (usuario_id, rol_id) DO NOTHING;