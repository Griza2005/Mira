# Data Model — MIRA v0.1 (Entrega 1)

## 1. Fuente
Modelo de datos para las 6 tablas maestras independientes (sin claves foráneas) del módulo MIRA, alineado con las especificaciones de la Entrega 1.

## 2. Reglas Generales del Modelo
- **Llaves Primarias**: Enteros auto-incrementales (`SERIAL`) para entidades numéricas y cadena de texto (`VARCHAR(150)`) para `termino_clave`.
- **Borrado Lógico**: Ningún registro se elimina físicamente de la base de datos (`DELETE` ejecuta un `UPDATE` seteando `activo = FALSE`).
- **Filtrado por defecto**: Todas las operaciones de lectura (`GET`) deben devolver únicamente los registros con `activo = TRUE`.
- **Valores por defecto**: El campo `activo` se inicializa en `TRUE` en la creación.

---

## 3. Definición de Tablas

### 3.1. `area_conocimiento`
Representa la clasificación del conocimiento por grandes áreas, áreas específicas y disciplinas académicas.

| Columna | Tipo de Dato | Nulo | Descripción |
| :--- | :--- | :--- | :--- |
| `id` | `SERIAL` (INT) | NO | Llave primaria auto-incremental |
| `gran_area` | `VARCHAR(150)` | NO | Gran área de conocimiento (ej. Ingeniería y Tecnología) |
| `area` | `VARCHAR(150)` | NO | Área específica (ej. Ingeniería de Sistemas) |
| `disciplina` | `VARCHAR(150)` | NO | Disciplina académica (ej. Ingeniería de Software) |
| `activo` | `BOOLEAN` | NO | Estado de borrado lógico (DEFAULT TRUE) |

### 3.2. `objetivo_desarrollo_sostenible`
Objetivos de Desarrollo Sostenible (ODS) de la ONU a los cuales tributan los proyectos de investigación.

| Columna | Tipo de Dato | Nulo | Descripción |
| :--- | :--- | :--- | :--- |
| `id` | `SERIAL` (INT) | NO | Llave primaria auto-incremental |
| `nombre` | `VARCHAR(200)` | NO | Nombre del ODS (ej. Educación de Calidad) |
| `categoria` | `VARCHAR(100)` | NO | Categoría o dimensión (ej. Social, Económico, Ambiental) |
| `activo` | `BOOLEAN` | NO | Estado de borrado lógico (DEFAULT TRUE) |

### 3.3. `area_aplicacion`
Áreas y sectores prácticos donde se aplica la investigación generada.

| Columna | Tipo de Dato | Nulo | Descripción |
| :--- | :--- | :--- | :--- |
| `id` | `SERIAL` (INT) | NO | Llave primaria auto-incremental |
| `nombre` | `VARCHAR(200)` | NO | Nombre del sector o área de aplicación |
| `activo` | `BOOLEAN` | NO | Estado de borrado lógico (DEFAULT TRUE) |

### 3.4. `termino_clave`
Vocabulario controlado y palabras clave normalizadas para etiquetar y buscar proyectos.

| Columna | Tipo de Dato | Nulo | Descripción |
| :--- | :--- | :--- | :--- |
| `termino` | `VARCHAR(150)` | NO | Llave primaria (string) del término clave |
| `termino_ingles` | `VARCHAR(150)` | SÍ | Equivalente del término en idioma inglés |
| `activo` | `BOOLEAN` | NO | Estado de borrado lógico (DEFAULT TRUE) |

### 3.5. `universidad`
Instituciones de educación superior y universidades colaboradoras en investigación.

| Columna | Tipo de Dato | Nulo | Descripción |
| :--- | :--- | :--- | :--- |
| `id` | `SERIAL` (INT) | NO | Llave primaria auto-incremental |
| `nombre` | `VARCHAR(200)` | NO | Razón social o nombre institucional |
| `tipo` | `VARCHAR(100)` | NO | Naturaleza jurídica (Pública, Privada) |
| `ciudad` | `VARCHAR(100)` | NO | Ciudad sede principal |
| `activo` | `BOOLEAN` | NO | Estado de borrado lógico (DEFAULT TRUE) |

### 3.6. `linea_investigacion`
Líneas de investigación institucionales formalmente reconocidas.

| Columna | Tipo de Dato | Nulo | Descripción |
| :--- | :--- | :--- | :--- |
| `id` | `SERIAL` (INT) | NO | Llave primaria auto-incremental |
| `nombre` | `VARCHAR(200)` | NO | Nombre de la línea de investigación |
| `descripcion` | `TEXT` | SÍ | Descripción o alcance temático de la línea |
| `activo` | `BOOLEAN` | NO | Estado de borrado lógico (DEFAULT TRUE) |

---

## 4. Relaciones
En esta versión foundation (Entrega 1), estas 6 tablas actúan como tablas maestras independientes sin claves foráneas (sin FK) hacia otras tablas.

## 5. Responsabilidades
- **Base de datos (PostgreSQL)**: Integridad física, restricciones NOT NULL, llaves primarias y valores por defecto.
- **API (MIRA.Api)**: Validación de negocio de campos obligatorios, cadenas no vacías, mapeo objeto-relacional mediante Dapper y ejecución del borrado lógico.
