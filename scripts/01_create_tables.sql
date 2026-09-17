-- =============================================================================
-- MIRA - Creación de tablas base (Sin llaves foráneas / Entrega 1)
-- =============================================================================

-- 1. area_conocimiento
CREATE TABLE IF NOT EXISTS area_conocimiento (
    id SERIAL PRIMARY KEY,
    gran_area VARCHAR(150) NOT NULL,
    area VARCHAR(150) NOT NULL,
    disciplina VARCHAR(150) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- 2. objetivo_desarrollo_sostenible
CREATE TABLE IF NOT EXISTS objetivo_desarrollo_sostenible (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    categoria VARCHAR(100) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- 3. area_aplicacion
CREATE TABLE IF NOT EXISTS area_aplicacion (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- 4. termino_clave (PK string)
CREATE TABLE IF NOT EXISTS termino_clave (
    termino VARCHAR(150) PRIMARY KEY,
    termino_ingles VARCHAR(150),
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- 5. universidad
CREATE TABLE IF NOT EXISTS universidad (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    tipo VARCHAR(100) NOT NULL,
    ciudad VARCHAR(100) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- 6. linea_investigacion
CREATE TABLE IF NOT EXISTS linea_investigacion (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

