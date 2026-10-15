# Investigación y Decisiones Técnicas (ADR) — MIRA v0.2 (Entrega 2)

> **Versión 0.2**: Registro de Decisiones de Arquitectura (**ADR**).  
> Rige la constitución permanente: `docs/1_constitution.md`.  
> Este documento registra el análisis técnico, las opciones evaluadas y las justificaciones de diseño para la gestión de relaciones e integridad referencial.

---

## D1 — Estrategia de Carga de Datos Relacionados: JOINs en Dapper

**Contexto:**  
Al consultar entidades relacionales como `docente`, `grupo_investigacion` o `semillero`, el cliente requiere visualizar nombres y descripciones legibles (por ejemplo, el nombre de la sede o de la línea matriz) en lugar de solo identificadores numéricos.

**Opciones evaluadas:**  
* (a) Consultar primero la tabla base y luego hacer una consulta adicional por cada registro para buscar el nombre de la FK (Problema N+1).  
* (b) **Ejecutar `LEFT JOIN` o `INNER JOIN` parametrizado en una única consulta con Dapper.**  
* (c) Traer toda la base de datos a memoria y filtrar con LINQ to Objects.

**Decisión:** **(b) Consultas SQL con JOINs optimizados en Dapper.**  
**Justificación:**  
La opción (a) colapsa la red y degrada severamente el tiempo de respuesta. La opción (c) agota la memoria del contenedor. Realizar un `LEFT JOIN` en PostgreSQL permite obtener la entidad base junto con las descripciones relacionadas en una sola operación atómica de base de datos, respetando el mandato constitucional de alto rendimiento y SQL parametrizado puro.

---

## D2 — Validación de Claves Foráneas en la Capa de Servicios

**Contexto:**  
Si un usuario o cliente HTTP envía un ID foráneo inexistente o inactivo (por ejemplo, asignar una sede `universidad_id = 9999` que no existe), el motor de base de datos arrojará una excepción de restricción foránea (`foreign_key_violation`), resultando en un error no controlado `500 Internal Server Error`.

**Opciones evaluadas:**  
* (a) Dejar que PostgreSQL falle y capturar la excepción genérica de base de datos en el controlador.  
* (b) **Validar explícitamente en la capa de servicios la existencia y estado activo de la clave foránea antes de ejecutar la persistencia.**  
* (c) Desactivar las restricciones de claves foráneas en la base de datos.

**Decisión:** **(b) Validación explícita en la capa de servicios.**  
**Justificación:**  
Las reglas de negocio deben residir en la capa de servicios. Validar previamente con el repositorio correspondiente (`ExisteActivoAsync` o `GetByIdAsync`) permite capturar el dato erróneo de forma controlada y devolver al cliente un código semántico `400 Bad Request` con un mensaje claro, evitando errores 500 y protegiendo la integridad transaccional.

---

## D3 — Borrado Lógico en Tablas Puente (Muchos a Muchos)

**Contexto:**  
Las relaciones entre docentes y grupos (`docente_grupo`), o semilleros y líneas (`semillero_linea`), manejan claves primarias compuestas y datos históricos. El Artículo 5 de la Constitución prohíbe el uso de sentencias físicas `DELETE`.

**Opciones evaluadas:**  
* (a) Eliminar físicamente los registros de la tabla puente usando `DELETE FROM`.  
* (b) **Mantener una columna `activo BOOLEAN NOT NULL DEFAULT TRUE` en cada tabla puente y ejecutar un `UPDATE` de desactivación.**  
* (c) Eliminar la tabla puente completa cuando un grupo se desactive.

**Decisión:** **(b) Desactivación lógica con columna `activo`.**  
**Justificación:**  
Permite mantener la trazabilidad histórica de participación de docentes y semilleros a lo largo del tiempo. Si un docente se desvincula de un grupo, la relación pasa a `activo = false`, preservando el histórico institucional para auditorías e informes de gestión (HU 5 y HU 10).

---

## D4 — Carga de Selectores en el Frontend (Flask + Jinja2)

**Contexto:**  
Para crear un nuevo docente o grupo, el usuario web necesita seleccionar la línea de investigación o sede universitaria desde un listado desplegable.

**Opciones evaluadas:**  
* (a) Quemar (hardcodear) las opciones en el código HTML de las plantillas.  
* (b) Conectar la aplicación web de Flask directamente a PostgreSQL para consultar los catálogos.  
* (c) **Hacer peticiones HTTP `GET` a la API REST de MIRA desde la vista de Flask para poblar dinámicamente el `<select>`.**

**Decisión:** **(c) Carga dinámica consumiendo la API REST.**  
**Justificación:**  
La arquitectura exige que el frontend esté totalmente desacoplado y tenga estrictamente prohibido conectarse de manera directa a la base de datos. Al consultar los endpoints `GET /api/{tabla}` de la Versión 0.1, el frontend siempre muestra los catálogos actualizados y únicamente las opciones activas.

---

## D5 — Formato de Parámetros en Desvinculación de Tablas Puente

**Contexto:**  
En tablas puente con claves compuestas (ej. `docente_cedula` y `grupo_id`), no existe un único `id` entero que pueda enviarse en la URL como `/api/docente_grupo/{id}`.

**Opciones evaluadas:**  
* (a) Enviar un cuerpo JSON en la petición `DELETE`.  
* (b) Concatenar las claves en la URL con guiones (ej. `/api/docente_grupo/1010-2`).  
* (c) **Utilizar Query Parameters estándar en la URL (ej. `DELETE /api/docente_grupo?docenteCedula=1010&grupoId=2`).**

**Decisión:** **(c) Query Parameters.**  
**Justificación:**  
El envío de body en solicitudes `DELETE` está desaconsejado por los estándares HTTP y clientes proxy. El uso de parámetros en la URL (`Query Parameters`) es estándar, transparente para Swagger y permite al controlador extraer ambos identificadores de forma tipada y segura.

---

## D6 — Mantenimiento del Entorno Contenerizado con Docker Compose

**Contexto:**  
La adición de nuevas tablas e interfaces no debe romper la simplicidad de ejecución del proyecto.

**Decisión:**  
Mantener la orquestación centralizada en `docker-compose.yml`. Las nuevas tablas se adicionarán mediante un script incremental en `database/init/` para que la base de datos se configure de manera automática y reproducible en cualquier máquina.