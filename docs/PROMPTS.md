# Registro de Prompts y Desarrollo Asistido por IA (Spec-Driven Development)

En cumplimiento de la metodología SDD descrita en `0_METODOLOGIA.md`, la inteligencia artificial se utilizó bajo restricciones formales derivadas del Spec Kit (`2_spec.md` y `6_contracts.md`), asegurando que la especificación siempre gobernara sobre el código generado.

---

## 1. Prompt para Modelado Relacional y Maestro-Detalle (Versión 2)
> **Contexto suministrado:** `versiones/v2.../2_spec.md` (Historias de Usuario HU 3, 5, 6, 8, 9) y `6_contracts.md`.  
> **Prompt técnico:**  
> *"Genera la capa de controladores REST en .NET 7 (`MIRA.Api/Controllers/`), servicios de dominio (`MIRA.Api/Servicios/`) y repositorios con Dapper para PostgreSQL (`MIRA.Api/Repositorios/`) cumpliendo con los contratos de `6_contracts.md`. Aplica borrado lógico (`activo = true/false`) para las entidades maestras (`docente`, `grupo_investigacion`, `semillero`) y modela las tablas puente de articulación (`docente_grupo`, `docente_semillero`, `grupo_linea`, `semillero_linea`, `linea_area`, `linea_ods`, `linea_area_aplicacion`) con sus respectivos metadatos de fechas y roles bajo el patrón maestro-detalle transaccional."*

---

## 2. Prompt para Seguridad JWT y Control de Acceso RBAC (Versión 3)
> **Contexto suministrado:** Requisitos de autenticación sin estado y matriz de roles de la v3.  
> **Prompt técnico:**  
> *"Implementa la seguridad perimetral en ASP.NET Core 7 configurando autenticación stateless mediante JWT (`JwtBearer`) y hashing unidireccional de contraseñas con `BCrypt.Net-Next`. Decora los controladores con el atributo `[Authorize]` y restringe los endpoints administrativos exclusivamente al rol 'Administrador' (`[Authorize(Roles = 'Administrador')]`). En el frontend Flask, gestiona la sesión de usuario inyectando la cabecera `Authorization: Bearer <token>` en cada llamada HTTP mediante `api_call` y define los decoradores `@login_required` y `@roles_required`."*

---

## 3. Prompt de Validación de Negocio y Diagrama de Secuencia (Rechazo FK 999)
> **Contexto suministrado:** Diagrama de secuencia de validación de claves foráneas.  
> **Prompt técnico:**  
> *"Implementa la defensa en capas en `DocenteService.cs`: valida previamente si la clave foránea de `linea_investigacion_principal` existe mediante `LineaInvestigacionRepository.GetByIdAsync`. Si retorna null, arroja un `ArgumentException` impidiendo que `DocenteRepository` sea ejecutado. Captura la excepción en `DocenteController.cs` traduciéndola a un código HTTP 400 Bad Request estructurado con JSON `{ estado: 400, mensaje: ... }`."*