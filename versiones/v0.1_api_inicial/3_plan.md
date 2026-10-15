# Plan de Implementación Técnica — MIRA v0.1 (Entrega 1)

> **Versión 0.1**: CÓMO se construye lo especificado en `2_spec.md`.  
> Rige la constitución permanente: `docs/1_constitution.md`.  
> Este documento rige las decisiones arquitectónicas para la API Backend, el acceso a datos con Dapper, el frontend en Flask y la orquestación en Docker.

---

## 1. Stack Tecnológico

| Capa / Componente | Tecnología | Justificación y Rol |
| :--- | :--- | :--- |
| **Lenguaje Backend** | **C# (.NET 7)** | Lenguaje tipado de alto rendimiento para APIs REST empresariales. |
| **Framework Backend** | **ASP.NET Core Web API** | Controladores ligeros con enrutamiento por atributos y documentación OpenAPI/Swagger. |
| **Acceso a Datos** | **Dapper (v2.1.35) + Npgsql (v7.0.7)** | Micro-ORM rápido que cumple con el mandato constitucional: SQL parametrizado puro, sin ORMs pesados. |
| **Base de Datos** | **PostgreSQL 16** | Motor relacional robusto ejecutado en contenedor Docker aislado. |
| **Frontend Web** | **Python (Flask + Jinja2)** | Interfaz de usuario desacoplada que consume la API exclusivamente mediante HTTP (`requests`). |
| **Orquestación** | **Docker & Docker Compose** | Infraestructura reproducible y portable ejecutable con un único comando. |

---

## 2. Arquitectura del Sistema

El sistema implementa una arquitectura multicapa desacoplada con Inyección de Dependencias (DI):

```text
[ Cliente Web / Swagger ]
          │ (HTTP / JSON)
          ▼
┌──────────────────────────────────────────────┐
│                  MIRA.Api                    │
│  Controllers (Peticiones)                    │
│      │                                       │
│      ▼                                       │
│  Services (Reglas de Negocio)                │
│      │                                       │
│      ▼                                       │
│  Repositories (Consultas Dapper)             │
└──────────────────────────────────────────────┘
          │ (Conexión TCP / SQL Parametrizado)
          ▼
┌──────────────────────────────────────────────┐
│         mira-postgres (Docker)               │
│  Tablas maestras con borrado lógico          │
└──────────────────────────────────────────────┘