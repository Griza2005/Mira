# Arquitectura de Software y Modelo Maestro-Detalle - Proyecto MIRA
# Juan jose grisales - 30000129372

## 1. Arquitectura Multicapa Contenerizada
El proyecto MIRA implementa una arquitectura multicapa desacoplada orquestada mediante Docker Compose:

1. **Capa de Presentación (Frontend Web):** Servidor Flask (Python 3) en el puerto 5000. Gestiona sesiones web (`session['jwt_token']`), renderiza vistas Jinja2 e interactúa exclusivamente vía HTTP REST inyectando la cabecera `Authorization: Bearer <token>`.
2. **Capa de Exposición (Controladores API):** ASP.NET Core 7 (.NET 7) en el puerto 8081 (`MIRA.Api/Controllers/`). Controladores delgados protegidos con `[Authorize]`, responsables de la negociación HTTP y captura de excepciones operativas (`try/catch`).
3. **Capa de Lógica de Negocio (Servicios):** `MIRA.Api/Servicios/`. Valida reglas de dominio e integridad referencial antes de invocar la persistencia.
4. **Capa de Acceso a Datos (Repositorios):** `MIRA.Api/Repositorios/`. Micro-ORM Dapper con sentencias SQL parametrizadas directas contra PostgreSQL 16 (puerto 5544) bajo el estándar de borrado lógico (`activo = true/false`).

---

## 2. Implementación del Patrón Maestro-Detalle (Master-Detail)

En el sistema MIRA, las entidades de investigación académica se articulan bajo el patrón Maestro-Detalle:

* **Caso 1: Grupo de Investigación (Maestro) ➔ Investigadores Vinculados (Detalle):**
  - **Maestro:** Tabla `grupo_investigacion` (código, nombre, universidad, docente líder).
  - **Detalle:** Tabla puente `docente_grupo` (docente, rol, fecha de inicio y fecha de fin).
  - **Flujo:** La interfaz captura los datos del grupo y su lista de investigadores en una sola vista, y el repositorio ejecuta las operaciones transaccionales para persistir el maestro y los renglones de detalle asociados.

* **Caso 2: Semillero (Maestro) ➔ Tutores Asignados (Detalle):**
  - **Maestro:** Tabla `semillero` (nombre, grupo de investigación base).
  - **Detalle:** Tabla puente `docente_semillero` (docente tutor, rol y periodo académico).

---

## 3. Inventario del Modelo Relacional (18 Tablas)
1. **Catálogos Maestros (6):** `area_conocimiento`, `objetivo_desarrollo_sostenible`, `area_aplicacion`, `termino_clave`, `universidad`, `linea_investigacion`.
2. **Entidades Principales (3):** `docente`, `grupo_investigacion`, `semillero`.
3. **Tablas Puente / Articulación N:M (7):** `docente_grupo`, `docente_semillero`, `grupo_linea`, `semillero_linea`, `linea_area`, `linea_ods`, `linea_area_aplicacion`.
4. **Seguridad y Acceso (2):** `usuario`, `rol` (y la intermedia `usuario_rol`).