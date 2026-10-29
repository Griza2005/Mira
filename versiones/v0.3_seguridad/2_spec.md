# Versión 3: Módulo de Seguridad, Autenticación, JWT y control de Roles

> **Versión 3: Módulo de Seguridad, Autenticación, JWT y control de Roles**  
> Rige la constitución permanente del proyecto: `docs/1_constitution.md`.  
> En esta versión el sistema incorpora el módulo transversal de seguridad con almacenamiento de contraseñas mediante hashing BCrypt, emisión y validación de tokens JWT (JSON Web Token) y Control de Acceso Basado en Roles (RBAC) tanto en la API REST (.NET 7) como en la interfaz web en Flask.

---

## 1. Propósito

Dotar al sistema **MIRA (Módulo de Investigación para la Gestión de Proyectos Académicos)** de la Universidad de San Buenaventura Medellín de una capa de seguridad robusta que proteja los recursos académicos e investigativos mediante autenticación basada en tokens firmados (`JWT Bearer`) y autorización diferenciada por roles (`Administrador`, `Editor` y `Consultor`).

---

## 2. Alcance

### 2.1. Capacidades de Seguridad Cubiertas
* **Autenticación Segura con BCrypt y JWT:** Validación de credenciales institucionales en `POST /api/Auth/login`, comparación criptográfica del hash `BCrypt` y generación de un token JWT firmado con `HmacSha256` que incluye los claims de identidad y roles del usuario.
* **Gestión de Usuarios y Roles (RBAC):** Administración de cuentas de usuario (`GET`, `POST`, `DELETE` lógico en `/api/Usuario`) restringida exclusivamente al rol `Administrador`, con asignación transaccional de roles en base de datos.
* **Protección de Endpoints por Rol:** Aplicación declarativa de `[Authorize]` y `[Authorize(Roles = "...")]` en todos los controladores de la API REST y decoradores `@login_required` / `@roles_required` en el Frontend Flask.

### 2.2. Entidades de Seguridad Incluidas
1. `rol` (`id` [PK SERIAL], `nombre` [UNIQUE], `descripcion`, `activo`, `fecha_creacion`).
2. `usuario` (`id` [PK SERIAL], `username` [UNIQUE], `password` [Hash BCrypt], `email` [UNIQUE], `nombre_completo`, `activo`, `fecha_creacion`, `fecha_actualizacion`).
3. `rol_usuario` (`usuario_id` [FK], `rol_id` [FK], `activo`, `fecha_asignacion`) — Tabla puente con PK compuesta `(usuario_id, rol_id)`.

### 2.3. Matriz de Control de Acceso Basado en Roles (RBAC)
1. **`Consultor`:** Acceso de solo lectura (`GET`) a los catálogos y módulos académicos.
2. **`Editor`:** Acceso de lectura (`GET`), creación (`POST`) y edición (`PUT`) en los módulos académicos y articulaciones.
3. **`Administrador`:** Control total del sistema, incluyendo borrado lógico (`DELETE`) en todos los módulos y administración exclusiva del módulo de usuarios (`/api/Usuario` y `/usuarios`).

---

## 3. Requisitos Funcionales

### RF1 — Autenticación y Emisión de Token JWT (`POST /api/Auth/login`)
* El sistema debe validar que el `username` exista en la tabla `usuario` con `activo = true` y verificar la contraseña mediante `BCrypt.Net.BCrypt.Verify`.
* En caso de credenciales inválidas o usuario inactivo, debe responder `401 Unauthorized`.
* En caso exitoso, debe retornar `200 OK` con el token JWT y los datos públicos del usuario (`username`, `nombreCompleto`, `roles`).

### RF2 — Encriptación de Contraseñas y Registro Transaccional (`POST /api/Usuario`)
* **Prohibición de texto plano:** Ninguna contraseña se almacenará sin antes procesarse con `BCrypt.Net.BCrypt.HashPassword`.
* **Transaccionalidad:** La creación del registro en `usuario` y la asignación de sus perfiles en `rol_usuario` deben ejecutarse dentro de una misma transacción de base de datos (`BeginTransaction`).

### RF3 — Autorización por Roles y Borrado Lógico de Usuarios (`DELETE /api/Usuario/{id}`)
* Todo endpoint protegido validará la firma, emisor (`MIRA_Api`), audiencia (`MIRA_Client`) y vigencia del token JWT.
* Si un usuario autenticado intenta ejecutar una operación para la cual no posee el rol requerido, la API debe responder `403 Forbidden`.
* La desactivación de un usuario ejecuta un borrado lógico (`UPDATE usuario SET activo = FALSE`).

### RF4 — Integración de Sesión y Permisos en el Frontend Flask
* El cliente web en Flask gestionará el inicio y cierre de sesión (`/login` y `/logout`), inyectando automáticamente el encabezado `Authorization: Bearer <token>` en cada petición hacia la API REST.
* La interfaz adaptará dinámicamente las acciones visibles (botones de creación, edición, eliminación y menú de usuarios) según los roles del usuario autenticado.

---

## 4. Requisitos No Funcionales

* **RNF1 — Separación de DTOs de Seguridad:** Las respuestas de la API (`UsuarioResponse`, `LoginResponse`) nunca expondrán el campo `password` (hash BCrypt) de la entidad `Usuario`.
* **RNF2 — Persistencia con Dapper y SQL Explícito:** Se mantiene el acceso a datos mediante `Npgsql` y `Dapper` con consultas parametrizadas y soporte transaccional.
* **RNF3 — Configuración de Seguridad en Swagger OpenAPI:** La interfaz de Swagger debe incluir la definición de seguridad `Bearer` para permitir autenticar y probar todos los endpoints protegidos.

---

## 5. Criterios de Aceptación (Rúbrica de la Versión 3)

1. **Rechazo de Peticiones No Autenticadas:** Cualquier solicitud a un controlador protegido sin token JWT válido recibe `401 Unauthorized`.
2. **Restricción Efectiva por Rol:** Un usuario con rol `Consultor` o `Editor` que intente eliminar un registro (`DELETE`) o acceder a `/api/Usuario` recibe `403 Forbidden`.
3. **Hashing BCrypt Verificado:** Las contraseñas en la tabla `usuario` se almacenan exclusivamente como hashes `$2a$11$...` generados con BCrypt.
4. **Flujo Completo en Frontend y Swagger:** Es posible iniciar sesión con el usuario administrador inicial (`admin`), navegar según los permisos del rol asignado y cerrar sesión limpiamente.
