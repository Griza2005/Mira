# Especificación de Requisitos — MIRA v0.1 (Entrega 1)

> **Versión 0.1**: Catálogos Maestros y Tablas Independientes.  
> Rige la constitución permanente del proyecto: `docs/1_constitution.md`.  
> En esta versión se construyen los cimientos del sistema: CRUD completo con borrado lógico para las 6 entidades sin dependencias externas.

---

## 1. Propósito

Construir la primera rebanada vertical del sistema **MIRA (Módulo de Investigación para la Gestión de Proyectos Académicos)** para la Universidad de San Buenaventura Medellín. 

Esta entrega establece la infraestructura base en Docker (PostgreSQL, Web API en ASP.NET Core y Frontend en Python Flask), garantizando la arquitectura en capas (`Controlador -> Servicio -> Repositorio -> Base de Datos`) y permitiendo la administración inicial de los catálogos y sedes institucionales.

---

## 2. Alcance

### 2.1. Historias de Usuario Cubiertas
* **HU 2 — Gestión de Catálogos de Clasificación:** Parametrización institucional de Áreas de Conocimiento, ODS, Áreas de Aplicación y Términos Clave.
* **HU 4 — Gestión de Sedes y Seccionales Universitarias:** Registro y consulta de sedes institucionales (`universidad`).
* **HU 7 — Gestión de Líneas de Investigación Institucionales:** Creación, edición y desactivación de líneas matrices de investigación.

### 2.2. Entidades Incluidas (6 Tablas Maestras)
1. `area_conocimiento` (gran_area, area, disciplina)
2. `objetivo_desarrollo_sostenible` (nombre, categoria)
3. `area_aplicacion` (nombre)
4. `termino_clave` (termino [PK string], termino_ingles)
5. `universidad` (nombre, ciudad, sede_principal)
6. `linea_investigacion` (nombre, descripcion)

### 2.3. Funcionalidades Incluidas
* Operaciones CRUD completas (Crear, Consultar todos, Consultar por ID, Actualizar y Eliminar Lógico).
* Política obligatoria de **Borrado Lógico**: Ninguna fila se elimina físicamente; se actualiza `activo = false`.
* Filtrado automático por defecto: Las consultas de lectura solo retornan registros con `activo = true`.
* Contenedores orquestados con Docker Compose para PostgreSQL, API y Frontend.
* Interfaz web responsiva en Flask para gestión visual de los 6 catálogos.

### 2.4. Explícitamente Excluido (YAGNI / Versiones Futuras)
* Tablas con claves foráneas, docentes, semilleros y relaciones complejas (Versión 0.2).
* Autenticación con JWT, roles y permisos de usuario (Versión 0.3).
* Consultas multitabla analíticas, dashboards estadísticos y reportes consolidados (Versión 0.4).

---

## 3. Requisitos Funcionales

*(Aplica de manera homogénea a las 6 tablas maestras; se utiliza `{tabla}` como convención de ruta).*

### RF1 — Listar Registros Activos (GET)
* **Endpoint:** `GET /api/{tabla}`
* **Comportamiento:** Retorna código `200 OK` con el listado JSON de los registros cuyo campo sea `activo = true`.
* **Regla:** Los registros inactivos (`activo = false`) quedan excluidos del listado principal.

### RF2 — Consultar por Identificador (GET por ID)
* **Endpoint:** `GET /api/{tabla}/{id}` *(o `GET /api/TerminoClave/{termino}` para PK string)*.
* **Comportamiento:** Retorna código `200 OK` con los datos de la entidad si existe y está activa.
* **Respuesta negativa:** Si el identificador no existe o tiene `activo = false`, responde `404 Not Found`.

### RF3 — Registro de Nueva Entidad (POST)
* **Endpoint:** `POST /api/{tabla}`
* **Comportamiento:** Valida la estructura del JSON entrante. Si los datos requeridos son correctos, inserta el registro con `activo = true` y responde `201 Created` con la cabecera `Location`.
* **Validación:** Si faltan campos obligatorios o los tipos son incorrectos, retorna `400 Bad Request`.
* **Conflicto:** En `termino_clave`, si la clave primaria ya existe, retorna `409 Conflict`.

### RF4 — Actualización de Datos (PUT)
* **Endpoint:** `PUT /api/{tabla}/{id}` *(o `PUT /api/TerminoClave/{termino}`)*.
* **Comportamiento:** Reemplaza los datos editables del registro existente. Responde `200 OK` con mensaje de confirmación.
* **Regla:** Si el registro no existe o está inactivo, retorna `404 Not Found`.

### RF5 — Borrado Lógico (DELETE)
* **Endpoint:** `DELETE /api/{tabla}/{id}` *(o `DELETE /api/TerminoClave/{termino}`)*.
* **Comportamiento:** Ejecuta un `UPDATE {tabla} SET activo = false WHERE id = @id`. Retorna `200 OK`.
* **Regla estricta:** Prohibido el uso de sentencias `DELETE` en SQL. Si el registro no existe, responde `404 Not Found`.

---

## 4. Requisitos No Funcionales

* **RNF1 — Arquitectura en Capas:** Desacoplamiento estricto: `Controlador` (HTTP/JSON) -> `Servicio` (Reglas de negocio) -> `Repositorio` (Acceso a BD) -> `PostgreSQL`.
* **RNF2 — Sin ORM / Dapper:** Prohibido Entity Framework. El acceso a datos debe ser directo mediante `Npgsql` y `Dapper`, usando exclusivamente SQL parametrizado (`@parametro`) para prevenir inyecciones SQL.
* **RNF3 — Operaciones Asíncronas:** Todo acceso a base de datos y llamadas de red en C# deben ser asíncronas (`async` / `await`).
* **RNF4 — Despliegue en un solo comando:** Todo el ecosistema debe compilar y levantarse localmente mediante `docker compose up -d --build`.
* **RNF5 — Frontend Desacoplado:** La interfaz gráfica en Flask consume la API exclusivamente mediante peticiones HTTP REST (librería `requests`), sin tocar la base de datos directamente.

---

## 5. Criterios de Aceptación (Cierre de Versión)

1. **Persistencia y Datos Semilla:** Al levantar los contenedores de Docker, la base de datos crea automáticamente las 6 tablas maestras e inserta los datos semilla de `02_seed_data.sql`.
2. **Operaciones CRUD Verificadas:** Los 6 controladores REST responden correctamente a las operaciones `GET`, `POST`, `PUT` y `DELETE` desde Swagger (`http://localhost:8081/swagger`).
3. **Borrado Lógico Funcional:** Al eliminar un registro vía `DELETE`, el registro desaparece del `GET /api/{tabla}`, pero permanece en PostgreSQL con `activo = false`.
4. **Validación de Frontend Web:** La aplicación web en Flask (`http://localhost:5000`) permite navegar, listar y registrar nuevos elementos en las 6 pantallas sin errores de conexión con la API.