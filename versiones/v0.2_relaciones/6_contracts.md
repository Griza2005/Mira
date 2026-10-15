# Contratos de API REST — MIRA v0.2 (Entrega 2)

> **Versión 0.2**: Especificación de Contratos para Entidades Relacionales y Tablas Puente.  
> Rige la constitución permanente: `docs/1_constitution.md`.  
> Base local: `http://localhost:8081`. Formato global: `application/json; charset=utf-8`.

---

## 0. Convenciones Generales y Manejo de Errores

| Situación | Código HTTP | Formato de Respuesta |
| :--- | :--- | :--- |
| Consulta o actualización exitosa | **`200 OK`** | Entidad o lista de entidades serializadas en JSON. |
| Creación exitosa de entidad | **`201 Created`** | Cabecera `Location` hacia el recurso creado y cuerpo de confirmación. |
| Operación exitosa en tabla puente | **`200 OK`** | `{"mensaje": "Relación establecida/eliminada exitosamente."}` |
| Clave Foránea inválida o inactiva | **`400 Bad Request`** | `{"estado": 400, "mensaje": "Violación de integridad referencial...", "detalle": "..."}` |
| Datos de entrada incompletos/malformados | **`400 Bad Request`** | `{"mensaje": "Campos obligatorios faltantes o con formato inválido."}` |
| Recurso no encontrado o inactivo | **`404 Not Found`** | `{"mensaje": "El recurso solicitado no fue encontrado o se encuentra inactivo."}` |
| Conflicto de unicidad (código/cédula) | **`409 Conflict`** | `{"mensaje": "Ya existe un registro con el identificador ingresado."}` |

---

## 0.1. Flujo de Secuencia: Rechazo por Clave Foránea Inválida (400 Bad Request)

```mermaid
sequenceDiagram
autonumber
actor Flask as Frontend (Flask)
participant Ctl as DocenteController
participant Srv as DocenteService
participant RepLinea as LineaInvestigacionRepository
participant RepDoc as DocenteRepository

Flask->>Ctl: POST /api/docente { "linea_investigacion_principal": 999, ... }
Ctl->>Srv: CreateAsync(dto)
Srv->>RepLinea: GetByIdAsync(999)
RepLinea-->>Srv: null (No existe o inactiva)
Note over Srv: Regla de Negocio: FK inválida.<br/>Lanza ArgumentException
Srv--xCtl: ArgumentException("La línea de investigación seleccionada no existe o está inactiva.")
Note over Ctl: Captura y mapea a HTTP 400
Ctl-->>Flask: 400 Bad Request { estado: 400, mensaje: "La línea seleccionada no es válida." }