# Plan de Implementación Técnica — MIRA v0.2 (Entrega 2)

> **Versión 0.2**: CÓMO se construye lo especificado en `2_spec.md`.  
> Rige la constitución permanente: `docs/1_constitution.md`.  
> Este documento rige las decisiones arquitectónicas para manejar relaciones, claves foráneas, Dapper con JOINs parametrizados y selectores dinámicos en Flask.

---

## 1. Stack Tecnológico (Se mantiene el de la Versión 0.1)

| Componente | Tecnología | Rol en la Versión 0.2 |
| :--- | :--- | :--- |
| **Backend API** | **C# sobre ASP.NET Core (.NET 7)** | Controladores REST con inyección de dependencias y validación de frontera. |
| **Acceso a Datos** | **Dapper + Npgsql** | Ejecución de `JOINs` y `Multi-Mapping` para traer entidades con sus datos relacionados en una sola consulta. Prohibido Entity Framework. |
| **Base de Datos** | **PostgreSQL 16 en Docker** | Tablas relacionales con restricciones `FOREIGN KEY`, índices y columna `activo`. |
| **Frontend Web** | **Python (Flask + Jinja2)** | Formularios enriquecidos que consumen la API para poblar listas desplegables dinámicas. |
| **Orquestación** | **Docker Compose** | Despliegue y persistencia unificada mediante contenedores. |

---

## 2. Arquitectura y Flujo de Integridad Referencial

El reto principal de esta versión es asegurar que **no existan registros huérfanos** y que cualquier intento de asociar un ID inexistente sea interceptado antes de llegar a fallar en la base de datos:

```text
[ Frontend Flask ]
      │ (Envía JSON con IDs foráneos, ej: linea_investigacion_id)
      ▼
[ Controlador C# ]
      │ (Valida formato y delega al servicio)
      ▼
[ Servicio C# ]
      ├─► Consulta ILineaInvestigacionRepository.GetByIdAsync(id)
      │     └─► ¿Existe y activo == true?
      │           ├─► NO ──► Lanza ArgumentException / ValidationException
      │           │          (El Controlador lo traduce a 400 Bad Request)
      │           └─► SÍ ──► Continúa flujo
      ▼
[ Repositorio Dapper ]
      │ (Ejecuta INSERT con SQL parametrizado)
      ▼
[ PostgreSQL en Docker ]