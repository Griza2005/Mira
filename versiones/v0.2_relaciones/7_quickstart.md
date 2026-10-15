# Guía de Inicio Rápido y Pruebas (Smoke Test) — MIRA v0.2 (Entrega 2)

> **Versión 0.2**: Validación de Integridad Referencial, Entidades Relacionales y Tablas Puente.  
> Rige la constitución permanente: `docs/1_constitution.md`.  
> Esta guía permite verificar el cumplimiento del 100% de los criterios de aceptación de `2_spec.md`.

---

## 1. Arranque del Entorno de Desarrollo

Todo el ecosistema se ejecuta centralizado en contenedores Docker:

```powershell
# 1. Asegurar que los contenedores estén actualizados y en ejecución
docker compose up -d --build

# 2. Verificar estado de salud de los servicios
docker compose ps