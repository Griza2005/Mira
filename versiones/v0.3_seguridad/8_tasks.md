# Tareas de Implementación — MIRA v0.2 (Entrega 2)

> **Versión 0.2**: Plan de Construcción Secuencial para Entidades Relacionales e Integridad Referencial.  
> Rige la constitución permanente: `docs/1_constitution.md`.  
> Cada fase termina en una compuerta de verificación obligatoria antes de avanzar a la siguiente.

---

## Flujo Secuencial de Construcción

```mermaid
flowchart TB
    F0["Fase 0: Spec Kit Completo"] --> F1["Fase 1: Esquema DDL y Semillas BD"]
    F1 -->|"Scripts SQL listos"| F2["Fase 2: Modelos C# y DTOs"]
    F2 -->|"Compilación OK"| F3["Fase 3: Repositorios Dapper (JOINs)"]
    F3 -->|"Consultas verificadas"| F4["Fase 4: Servicios (Validación FKs)"]
    F4 -->|"Lógica de negocio lista"| F5["Fase 5: Controladores y Program.cs"]
    F5 -->|"Swagger funcional"| F6["Fase 6: Frontend Flask (Selectores)"]
    F6 -->|"UI dinámica validada"| F7["Fase 7: Smoke Test y Cierre (Tag v0.2)"]