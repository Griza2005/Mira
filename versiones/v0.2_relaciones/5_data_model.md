# Modelo de Datos — MIRA v0.2 (Entrega 2)

> **Versión 0.2**: Tablas Relacionales y Articulación Académica.  
> Rige la constitución permanente: `docs/1_constitution.md`.  
> Este documento detalla las tablas, columnas, restricciones de claves foráneas y estructuras puente que se incorporan al esquema de PostgreSQL sobre la base de la versión 0.1.

---

## 1. Alcance del Modelo en la Versión 0.2

La base de datos conserva íntegras las 6 tablas maestras independientes de la versión 0.1 (`area_conocimiento`, `objetivo_desarrollo_sostenible`, `area_aplicacion`, `termino_clave`, `universidad`, `linea_investigacion`).

En esta versión se incorporan **10 tablas nuevas** organizadas en dos categorías:
1. **Entidades Fuertes Relacionales (3):** `docente`, `grupo_investigacion`, `semillero`.
2. **Tablas Puente Muchos a Muchos (7):** `docente_grupo`, `docente_semillero`, `grupo_linea`, `semillero_linea`, `linea_area`, `linea_ods`, `linea_area_aplicacion`.

Todas las tablas incluyen la columna `activo BOOLEAN NOT NULL DEFAULT TRUE` para garantizar el borrado lógico.

---

## 2. Diagrama de Relaciones de la Versión 0.2

```text
universidad (v0.1) ◄──────┐
                          │ (universidad_id)
linea_investigacion (v0.1)│
      ▲                   ▼
      │ (FK)     grupo_investigacion ◄─── docente (líder)
      │                   ▲                  ▲
   docente                │                  │
      ▲                   │ (grupo_id)       │
      │                   ▼                  │
      └──────────► docente_grupo ────────────┘
                          ▲
                          │
                      semillero ◄──── docente_semillero ◄── docente