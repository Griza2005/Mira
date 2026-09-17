# Guía de Inicio Rápido y Pruebas — MIRA v0.1 (Entrega 1)

Guía para levantar el entorno local con Docker y verificar los endpoints CRUD de las 6 tablas sin FK.

## 1. Levantamiento del Entorno

```bash
# 1. Asegurar la existencia del archivo de entorno .env
cp .env.example .env

# 2. Levantar la base de datos PostgreSQL y la API en Docker
docker compose up -d --build

# 3. Verificar estado de los contenedores
docker compose ps
```

- **Healthcheck de la API**: [http://localhost:8081/health](http://localhost:8081/health)
- **Interfaz Swagger UI**: [http://localhost:8081/swagger](http://localhost:8081/swagger)
- **Aplicación Frontend Web**: [http://localhost:5000](http://localhost:5000)

---

## 2. Comandos cURL de Prueba (Smoke Tests)

### 2.1. Área de Conocimiento (`area_conocimiento`)
```bash
# Listar activos
curl -X GET http://localhost:8081/api/area_conocimiento

# Obtener por ID
curl -X GET http://localhost:8081/api/area_conocimiento/1

# Crear nuevo
curl -X POST http://localhost:8081/api/area_conocimiento \
  -H "Content-Type: application/json" \
  -d '{"granArea": "Ingeniería", "area": "Telecomunicaciones", "disciplina": "Redes 5G"}'

# Actualizar
curl -X PUT http://localhost:8081/api/area_conocimiento/1 \
  -H "Content-Type: application/json" \
  -d '{"granArea": "Ingeniería y Tecnología", "area": "Ingeniería de Sistemas", "disciplina": "Arquitectura de Software"}'

# Borrado lógico
curl -X DELETE http://localhost:8081/api/area_conocimiento/1
```

### 2.2. Objetivo de Desarrollo Sostenible (`objetivo_desarrollo_sostenible`)
```bash
# Listar activos
curl -X GET http://localhost:8081/api/objetivo_desarrollo_sostenible

# Obtener por ID
curl -X GET http://localhost:8081/api/objetivo_desarrollo_sostenible/1

# Crear nuevo
curl -X POST http://localhost:8081/api/objetivo_desarrollo_sostenible \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Energía Asequible y No Contaminante", "categoria": "Ambiental"}'

# Actualizar
curl -X PUT http://localhost:8081/api/objetivo_desarrollo_sostenible/1 \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Educación Inclusiva y de Calidad", "categoria": "Social"}'

# Borrado lógico
curl -X DELETE http://localhost:8081/api/objetivo_desarrollo_sostenible/1
```

### 2.3. Área de Aplicación (`area_aplicacion`)
```bash
# Listar activos
curl -X GET http://localhost:8081/api/area_aplicacion

# Obtener por ID
curl -X GET http://localhost:8081/api/area_aplicacion/1

# Crear nuevo
curl -X POST http://localhost:8081/api/area_aplicacion \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Agricultura Inteligente y Agrotech"}'

# Actualizar
curl -X PUT http://localhost:8081/api/area_aplicacion/1 \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Salud Digital Avanzada"}'

# Borrado lógico
curl -X DELETE http://localhost:8081/api/area_aplicacion/1
```

### 2.4. Término Clave (`termino_clave`)
```bash
# Listar activos
curl -X GET http://localhost:8081/api/termino_clave

# Obtener por Término (string PK)
curl -X GET "http://localhost:8081/api/termino_clave/Inteligencia%20Artificial"

# Crear nuevo
curl -X POST http://localhost:8081/api/termino_clave \
  -H "Content-Type: application/json" \
  -d '{"termino": "Blockchain", "terminoIngles": "Blockchain"}'

# Actualizar equivalente en inglés
curl -X PUT "http://localhost:8081/api/termino_clave/Blockchain" \
  -H "Content-Type: application/json" \
  -d '{"terminoIngles": "Distributed Ledger Technology"}'

# Borrado lógico
curl -X DELETE "http://localhost:8081/api/termino_clave/Blockchain"
```

### 2.5. Universidad (`universidad`)
```bash
# Listar activos
curl -X GET http://localhost:8081/api/universidad

# Obtener por ID
curl -X GET http://localhost:8081/api/universidad/1

# Crear nueva
curl -X POST http://localhost:8081/api/universidad \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Universidad de los Andes", "tipo": "Privada", "ciudad": "Bogotá"}'

# Actualizar
curl -X PUT http://localhost:8081/api/universidad/1 \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Universidad de San Buenaventura - Sede Medellín", "tipo": "Privada", "ciudad": "Medellín"}'

# Borrado lógico
curl -X DELETE http://localhost:8081/api/universidad/1
```

### 2.6. Línea de Investigación (`linea_investigacion`)
```bash
# Listar activos
curl -X GET http://localhost:8081/api/linea_investigacion

# Obtener por ID
curl -X GET http://localhost:8081/api/linea_investigacion/1

# Crear nueva
curl -X POST http://localhost:8081/api/linea_investigacion \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Internet de las Cosas (IoT)", "descripcion": "Dispositivos embebidos y sensores para ciudades inteligentes."}'

# Actualizar
curl -X PUT http://localhost:8081/api/linea_investigacion/1 \
  -H "Content-Type: application/json" \
  -d '{"nombre": "Sistemas Distribuidos y Cloud", "descripcion": "Arquitecturas resilientes en entornos de nube."}'

# Borrado lógico
curl -X DELETE http://localhost:8081/api/linea_investigacion/1
```

---

## 3. Detener los Servicios

```bash
docker compose down
```
Para eliminar volúmenes y reiniciar datos:
```bash
docker compose down -v
```
