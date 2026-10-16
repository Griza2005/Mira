# MIRA - Módulo de Investigación para la Gestión de Proyectos Académicos

**Universidad de San Buenaventura Medellín**  
**Proyecto de Aula — Diseño de Software**  
**Estudiante:** JUAN JOSE GRISALES SAÑUDO — ID: 30000129372  

---

## Estado del Proyecto: Versión v0.3 (Completa y Funcional)

El sistema se encuentra en su fase **v0.3**, integrando la totalidad de los requisitos de las tres entregas del curso:
* **Versión 1 (v0.1):** Catálogos maestros institucionales.
* **Versión 2 (v0.2):** Modelo relacional complejo, tablas puente (N:M), borrado lógico (`activo = true/false`) y patrón Maestro-Detalle (**HU 3, 5, 6, 8 y 9**).
* **Versión 3 (v0.3):** Capa perimetral de seguridad stateless mediante **Tokens JWT**, almacenamiento criptográfico con **BCrypt** y **Control de Acceso Basado en Roles (RBAC)**.

---

## Credenciales de Acceso y Matriz de Roles (RBAC)

Para ingresar a la plataforma web (`http://localhost:5000`) y comprobar los niveles de autorización, se encuentran precargados los siguientes perfiles:

| Usuario | Contraseña | Rol Asignado | Alcance y Permisos en el Sistema |
| :--- | :--- | :--- | :--- |
| `superadmin` | `admin123` | **Administrador** | Acceso total al sistema, creación/edición de catálogos y gestión exclusiva del módulo administrativo de cuentas (`/usuarios`). |
| `editor1` | `editor123` | **Editor** | Gestión operativa: creación y edición de docentes, grupos, semilleros y articulaciones (HU 3, 5, 6, 8, 9). Sin acceso a gestión de usuarios. |
| `lector1` | `lector123` | **Consulta** | Modo solo lectura. La interfaz oculta contextual mente botones de acción y los decoradores bloquean cualquier intento de mutación de datos. |

---

## Despliegue Rápido con Docker Compose

El proyecto está completamente contenerizado para garantizar su reproducibilidad inmediata en cualquier entorno.

### Requisitos
* Docker Desktop activo.

### Comandos de ejecución
1. Clonar el repositorio y ubicarse en la raíz del proyecto:
   ```bash
   git clone [https://github.com/Griza2005/Mira.git](https://github.com/Griza2005/Mira.git)
   cd Mira