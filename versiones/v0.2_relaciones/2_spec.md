# Especificación de Requisitos — MIRA v0.2 (Entrega 2)

> **Versión 0.2**: Integridad Referencial, Estructura Académica y Articulación Investigativa.  
> Rige la constitución permanente del proyecto: `docs/1_constitution.md`.  
> En esta versión el sistema incorpora las entidades relacionales que articulan a docentes, grupos y semilleros con los catálogos base de la versión 0.1.

---

## 1. Propósito

Habilitar el núcleo relacional del sistema **MIRA (Módulo de Investigación para la Gestión de Proyectos Académicos)** para la Universidad de San Buenaventura Medellín. 

Esta versión garantiza la integridad referencial de los datos académicos mediante validación de claves foráneas en la capa de negocio, introduce las tablas puente para relaciones muchos a muchos y provee interfaces con selectores dinámicos en Flask que evitan el ingreso manual de identificadores foráneos.

---

## 2. Alcance

### 2.1. Historias de Usuario Cubiertas
* **HU 3 — Gestión de Perfiles Docentes e Investigadores:** Registro de hoja de vida institucional, CvLAC, categoría Minciencias y vinculación opcional a una línea de investigación matriz.
* **HU 5 — Administración de Grupos de Investigación:** Registro de grupos con GrupLAC, asignación de sede universitaria (`universidad`), designación de docente líder y vinculación histórica de investigadores con roles y fechas.
* **HU 6 — Creación de Semilleros y Asignación de Tutores:** Registro de semilleros adscritos a un grupo de investigación base y vinculación de docentes tutores con periodo académico y rol.
* **HU 8 — Articulación de Grupos y Semilleros con Líneas:** Vinculación de grupos y semilleros a una o varias líneas de investigación institucionales.
* **HU 9 — Articulación de Líneas de Investigación:** Mapeo de líneas de investigación con Áreas de Conocimiento, ODS y Áreas de Aplicación industrial.

### 2.2. Entidades Fuertes Relacionales Incluidas
1. `docente` (cedula [PK INT], nombres, apellidos, correo, enlace_cvlac, escalafon, categoria_minciencias, linea_investigacion_principal [FK], activo).
2. `grupo_investigacion` (id [PK SERIAL], codigo, nombre, enlace_gruplac, categoria_minciencias, universidad_id [FK], docente_lider_cedula [FK], activo).
3. `semillero` (id [PK SERIAL], nombre, descripcion, grupo_investigacion_id [FK], activo).

### 2.3. Tablas Puente Relacionales (Muchos a Muchos)
1. `docente_grupo` (docente_cedula [FK], grupo_id [FK], rol, fecha_inicio, fecha_fin, activo) — PK compuesta.
2. `docente_semillero` (docente_cedula [FK], semillero_id [FK], rol, periodo_academico, activo) — PK compuesta.
3. `grupo_linea` (grupo_id [FK], linea_id [FK], activo) — PK compuesta.
4. `semillero_linea` (semillero_id [FK], linea_id [FK], activo) — PK compuesta.
5. `linea_area` (linea_id [FK], area_conocimiento_id [FK], activo) — PK compuesta.
6. `linea_ods` (linea_id [FK], ods_id [FK], activo) — PK compuesta.
7. `linea_area_aplicacion` (linea_id [FK], area_aplicacion_id [FK], activo) — PK compuesta.

### 2.4. Explícitamente Excluido (YAGNI / Versiones Futuras)
* Autenticación con JWT, roles de usuario y permisos encriptados (Versión 0.3).
* Consultas analíticas complejas multitabla, dashboards con gráficos y reportes consolidados (Versión 0.4).

---

## 3. Requisitos Funcionales

### RF1 — Listado con Datos Relacionados (GET Enriquecido)
* Al consultar un `docente`, la API debe incluir el nombre de la línea de investigación principal asociada.
* Al consultar un `grupo_investigacion`, la respuesta debe incluir el nombre de la sede (`universidad`) y el nombre completo del docente líder.
* Al consultar un `semillero`, debe incluirse el nombre del grupo de investigación al que pertenece.

### RF2 — Validación de Claves Foráneas en Escritura (POST y PUT)
* **Regla estricta:** Antes de registrar o modificar una entidad o relación puente, la capa de servicios debe validar que todos los identificadores foráneos existan físicamente en la base de datos y tengan `activo = true`.
* **Respuesta de rechazo:** Si una clave foránea no existe o está inactiva, la API debe responder con código `400 Bad Request` indicando claramente cuál relación no es válida (evitando un fallo interno `500`).

### RF3 — Gestión de Relaciones en Tablas Puente (POST y DELETE)
* **Creación:** Permite asociar dos entidades existentes validando que ambos extremos estén activos.
* **Borrado Lógico Compuesto:** El endpoint de eliminación de relaciones puente recibe ambas claves por *Query Parameters* (ej: `DELETE /api/docente_grupo?docenteCedula=1010&grupoId=1`) y actualiza `activo = false`.

### RF4 — Integración en Frontend con Selectores Dinámicos
* En los formularios web de Flask, ningún campo foráneo se ingresará manualmente como un número.
* Las vistas cargarán dinámicamente las opciones activas desde los endpoints de la API (vía `GET`) para alimentar elementos `<select>`.

---

## 4. Requisitos No Funcionales

* **RNF1 — Consultas con JOINs en Dapper:** Queda prohibido el problema de consultas N+1. Las lecturas de entidades con datos relacionados deben realizarse mediante `INNER JOIN` o `LEFT JOIN` en SQL parametrizado puro.
* **RNF2 — Sin ORMs pesados:** Se mantiene la política constitucional: exclusión de Entity Framework Core; el acceso a datos se efectúa vía `Npgsql` y `Dapper`.
* **RNF3 — Integridad Transaccional:** La asignación de docentes con roles y fechas debe garantizar la coherencia de datos sin permitir duplicidades activas en el mismo periodo.

---

## 5. Criterios de Aceptación (Rúbrica de la Versión 0.2)

1. **Rechazo por Clave Foránea Inválida:** Un intento de crear un docente apuntando a una línea de investigación inexistente responde `400 Bad Request`.
2. **Consultas Enriquecidas Verificadas:** Los endpoints `GET` de docentes, grupos y semilleros retornan las descripciones textuales de sus entidades padres.
3. **Persistencia de Tablas Puente:** Es posible asociar y desvincular lógicamente un docente a un grupo y un semillero a una línea de investigación.
4. **Formularios Web Dinámicos:** La interfaz web en Flask permite crear un docente seleccionando su línea de investigación desde un menú desplegable poblado automáticamente por la API.