# MIRA v0.1 Tasks — Entrega 1

## Phase 1 — Review source documents
- [x] Revisar documentación base y reglas de arquitectura.
- [x] Identificar las 6 tablas maestras independientes sin claves foráneas.

## Phase 2 — Complete specification
- [x] Definir alcance de la Entrega 1 (CRUD 6 tablas sin FK).
- [x] Establecer política obligatoria de borrado lógico (`activo = false`).

## Phase 3 — Complete data model
- [x] Crear scripts DDL en `scripts/01_create_tables.sql` y `database/init/01_create_tables.sql`.
- [x] Crear scripts de semillas en `scripts/02_seed_data.sql` y `database/init/02_seed_data.sql`.
- [x] Actualizar especificación del modelo de datos en `5_data_model.md`.

## Phase 4 — Define API contracts
- [x] Definir especificación REST para las 6 entidades en `6_contracts.md`.
- [x] Detallar payloads JSON de solicitud y respuesta para cada operación CRUD.

## Phase 5 — Implement Repository layer
- [x] Implementar factoría de conexiones `DbConnectionFactory` con `Npgsql`.
- [x] Crear interfaces e implementaciones de repositorio para las 6 tablas con Dapper y SQL parametrizado (`GetAllActivosAsync`, `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `SoftDeleteAsync`).

## Phase 6 — Implement Service layer
- [x] Crear interfaces e implementaciones de servicio para las 6 entidades con validaciones de negocio en `MIRA.Api/Servicios/`.

## Phase 7 — Implement Controller layer
- [x] Implementar controladores REST en `MIRA.Api/Peticiones/` (`AreaConocimientoController`, `ObjetivoDesarrolloSostenibleController`, `AreaAplicacionController`, `TerminoClaveController`, `UniversidadController`, `LineaInvestigacionController`).

## Phase 8 — Configure Dependency Injection
- [x] Registrar factoría de conexión, repositorios y servicios en `Program.cs`.
- [x] Configurar mapeo de nombres de columnas snake_case a PascalCase con Dapper.

## Phase 9 — Docker/PostgreSQL
- [x] Configurar archivo `.env` con variables de conexión a PostgreSQL.
- [x] Verificar orquestación de servicios en `docker-compose.yml` (`mira-postgres` y `mira-api`).

## Phase 10 — Swagger
- [x] Configurar Swagger/OpenAPI en `Program.cs`.
- [x] Validar que los endpoints de las 6 tablas aparezcan documentados en `/swagger`.

## Phase 11 — Smoke Tests
- [x] Ejecutar pruebas CRUD sobre las 6 tablas verificando respuestas 200, 201, y borrado lógico.
- [x] Documentar ejemplos cURL en `7_quickstart.md`.

## Phase 12 — Version validation
- [x] Entrega 1 completada y validada según criterios de aceptación.
