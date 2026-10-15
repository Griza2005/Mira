-- =============================================================================
-- MIRA - Tablas Relacionales y Tablas Puente (Versión 0.2 / Entrega 2)
-- =============================================================================

-- 1. docente (HU 3)
CREATE TABLE IF NOT EXISTS docente (
    cedula INTEGER PRIMARY KEY,
    nombres VARCHAR(150) NOT NULL,
    apellidos VARCHAR(150) NOT NULL,
    correo VARCHAR(150) UNIQUE NOT NULL,
    enlace_cvlac VARCHAR(250),
    escalafon VARCHAR(100),
    categoria_minciencias VARCHAR(100),
    linea_investigacion_principal INTEGER REFERENCES linea_investigacion(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- 2. grupo_investigacion (HU 5)
CREATE TABLE IF NOT EXISTS grupo_investigacion (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(50) UNIQUE NOT NULL,
    nombre VARCHAR(200) NOT NULL,
    enlace_gruplac VARCHAR(250),
    categoria_minciencias VARCHAR(50),
    universidad_id INTEGER NOT NULL REFERENCES universidad(id) ON DELETE RESTRICT,
    docente_lider_cedula INTEGER NOT NULL REFERENCES docente(cedula) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- 3. semillero (HU 6)
CREATE TABLE IF NOT EXISTS semillero (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    grupo_investigacion_id INTEGER NOT NULL REFERENCES grupo_investigacion(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE
);

-- =============================================================================
-- Tablas Puente (Muchos a Muchos)
-- =============================================================================

-- 4. docente_grupo (HU 5)
CREATE TABLE IF NOT EXISTS docente_grupo (
    docente_cedula INTEGER NOT NULL REFERENCES docente(cedula) ON DELETE RESTRICT,
    grupo_id INTEGER NOT NULL REFERENCES grupo_investigacion(id) ON DELETE RESTRICT,
    rol VARCHAR(100) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (docente_cedula, grupo_id, fecha_inicio)
);

-- 5. docente_semillero (HU 6)
CREATE TABLE IF NOT EXISTS docente_semillero (
    docente_cedula INTEGER NOT NULL REFERENCES docente(cedula) ON DELETE RESTRICT,
    semillero_id INTEGER NOT NULL REFERENCES semillero(id) ON DELETE RESTRICT,
    rol VARCHAR(100) NOT NULL,
    periodo_academico VARCHAR(20) NOT NULL,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (docente_cedula, semillero_id, periodo_academico)
);

-- 6. grupo_linea (HU 8)
CREATE TABLE IF NOT EXISTS grupo_linea (
    grupo_id INTEGER NOT NULL REFERENCES grupo_investigacion(id) ON DELETE RESTRICT,
    linea_id INTEGER NOT NULL REFERENCES linea_investigacion(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (grupo_id, linea_id)
);

-- 7. semillero_linea (HU 8)
CREATE TABLE IF NOT EXISTS semillero_linea (
    semillero_id INTEGER NOT NULL REFERENCES semillero(id) ON DELETE RESTRICT,
    linea_id INTEGER NOT NULL REFERENCES linea_investigacion(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (semillero_id, linea_id)
);

-- 8. linea_area (HU 9)
CREATE TABLE IF NOT EXISTS linea_area (
    linea_id INTEGER NOT NULL REFERENCES linea_investigacion(id) ON DELETE RESTRICT,
    area_conocimiento_id INTEGER NOT NULL REFERENCES area_conocimiento(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (linea_id, area_conocimiento_id)
);

-- 9. linea_ods (HU 9)
CREATE TABLE IF NOT EXISTS linea_ods (
    linea_id INTEGER NOT NULL REFERENCES linea_investigacion(id) ON DELETE RESTRICT,
    ods_id INTEGER NOT NULL REFERENCES objetivo_desarrollo_sostenible(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (linea_id, ods_id)
);

-- 10. linea_area_aplicacion (HU 9)
CREATE TABLE IF NOT EXISTS linea_area_aplicacion (
    linea_id INTEGER NOT NULL REFERENCES linea_investigacion(id) ON DELETE RESTRICT,
    area_aplicacion_id INTEGER NOT NULL REFERENCES area_aplicacion(id) ON DELETE RESTRICT,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (linea_id, area_aplicacion_id)
);

-- =============================================================================
-- Datos Semilla Iniciales (Versión 0.2)
-- =============================================================================

-- Semillas Docentes (HU 3)
INSERT INTO docente (cedula, nombres, apellidos, correo, enlace_cvlac, escalafon, categoria_minciencias, linea_investigacion_principal, activo)
VALUES (10102020, 'Carlos Arturo', 'Castro Castro', 'ccastro@usbmed.edu.co', 'https://scienti.minciencias.gov.co/cvlac/0001', 'Asociado', 'Senior', 1, TRUE)
ON CONFLICT (cedula) DO NOTHING;

INSERT INTO docente (cedula, nombres, apellidos, correo, enlace_cvlac, escalafon, categoria_minciencias, linea_investigacion_principal, activo)
VALUES (10203040, 'Francisco', 'Guisado López', 'fguisado@usbmed.edu.co', 'https://scienti.minciencias.gov.co/cvlac/0002', 'Asistente', 'Junior', 1, TRUE)
ON CONFLICT (cedula) DO NOTHING;

-- Semilla Grupo de Investigación (HU 5)
INSERT INTO grupo_investigacion (codigo, nombre, enlace_gruplac, categoria_minciencias, universidad_id, docente_lider_cedula, activo)
VALUES ('COL0012345', 'Grupo de Investigación en Tecnologías Emergentes (GITE)', 'https://scienti.minciencias.gov.co/gruplac/0001', 'A1', 1, 10102020, TRUE)
ON CONFLICT (codigo) DO NOTHING;

-- Semilla Semillero (HU 6)
INSERT INTO semillero (nombre, descripcion, grupo_investigacion_id, activo)
VALUES ('Semillero de Arquitectura de Software y Cloud Computing', 'Investigación formativa en microservicios, DevOps y plataformas distribuidas.', 1, TRUE)
ON CONFLICT DO NOTHING;

-- Semilla Articulaciones
INSERT INTO grupo_linea (grupo_id, linea_id, activo) VALUES (1, 1, TRUE) ON CONFLICT DO NOTHING;
INSERT INTO semillero_linea (semillero_id, linea_id, activo) VALUES (1, 1, TRUE) ON CONFLICT DO NOTHING;
INSERT INTO linea_area (linea_id, area_conocimiento_id, activo) VALUES (1, 1, TRUE) ON CONFLICT DO NOTHING;
INSERT INTO linea_ods (linea_id, ods_id, activo) VALUES (1, 2, TRUE) ON CONFLICT DO NOTHING;
INSERT INTO linea_area_aplicacion (linea_id, area_aplicacion_id, activo) VALUES (1, 2, TRUE) ON CONFLICT DO NOTHING;