# Contratos de API REST — MIRA v0.1 (Entrega 1)

Documentación de los contratos REST para las 6 tablas maestras sin claves foráneas implementadas en `MIRA.Api`.

## 1. Convenciones Generales
- **Ruta Base**: `/api/{tabla}`
- **Formato**: JSON (`application/json; charset=utf-8`)
- **Códigos de Estado**:
  - `200 OK`: Operación de consulta, actualización o eliminación exitosa.
  - `201 Created`: Creación exitosa de un nuevo registro (incluye cabecera `Location`).
  - `400 Bad Request`: Error de validación en los campos enviados.
  - `404 Not Found`: El recurso solicitado no existe o se encuentra inactivo (borrado lógico).
  - `409 Conflict`: Conflicto al intentar crear una llave primaria existente (ej. en `termino_clave`).

---

## 2. Endpoints por Entidad

### 2.1. Área de Conocimiento (`/api/area_conocimiento`)

#### `GET /api/area_conocimiento`
- **Respuesta `200 OK`**:
```json
[
  {
    "id": 1,
    "granArea": "Ingeniería y Tecnología",
    "area": "Ingeniería de Sistemas",
    "disciplina": "Ingeniería de Software",
    "activo": true
  }
]
```

#### `GET /api/area_conocimiento/{id}`
- **Respuesta `200 OK`**:
```json
{
  "id": 1,
  "granArea": "Ingeniería y Tecnología",
  "area": "Ingeniería de Sistemas",
  "disciplina": "Ingeniería de Software",
  "activo": true
}
```
- **Respuesta `404 Not Found`**:
```json
{
  "mensaje": "Área de conocimiento con ID 999 no encontrada o inactiva."
}
```

#### `POST /api/area_conocimiento`
- **Cuerpo de la Petición**:
```json
{
  "granArea": "Ciencias Médicas",
  "area": "Medicina Básica",
  "disciplina": "Inmunología"
}
```
- **Respuesta `201 Created`**: Objeto creado con su `id` asignado y `activo: true`.

#### `PUT /api/area_conocimiento/{id}`
- **Cuerpo de la Petición**:
```json
{
  "granArea": "Ciencias Médicas",
  "area": "Medicina Básica",
  "disciplina": "Inmunología Clínica"
}
```
- **Respuesta `200 OK`**: `{"mensaje": "Área de conocimiento actualizada exitosamente."}`

#### `DELETE /api/area_conocimiento/{id}`
- **Respuesta `200 OK`**: `{"mensaje": "Área de conocimiento eliminada lógicamente."}`

---

### 2.2. Objetivo de Desarrollo Sostenible (`/api/objetivo_desarrollo_sostenible`)

- `GET /api/objetivo_desarrollo_sostenible` -> `200 OK` (listado activo)
- `GET /api/objetivo_desarrollo_sostenible/{id}` -> `200 OK` o `404 Not Found`
- `POST /api/objetivo_desarrollo_sostenible` -> `201 Created`
  - Payload: `{"nombre": "Fin de la Pobreza", "categoria": "Social"}`
- `PUT /api/objetivo_desarrollo_sostenible/{id}` -> `200 OK` o `404 Not Found`
- `DELETE /api/objetivo_desarrollo_sostenible/{id}` -> `200 OK` (borrado lógico)

---

### 2.3. Área de Aplicación (`/api/area_aplicacion`)

- `GET /api/area_aplicacion` -> `200 OK` (listado activo)
- `GET /api/area_aplicacion/{id}` -> `200 OK` o `404 Not Found`
- `POST /api/area_aplicacion` -> `201 Created`
  - Payload: `{"nombre": "Energías Renovables"}`
- `PUT /api/area_aplicacion/{id}` -> `200 OK` o `404 Not Found`
- `DELETE /api/area_aplicacion/{id}` -> `200 OK` (borrado lógico)

---

### 2.4. Término Clave (`/api/termino_clave`)

*Nota: Su identificador primario es la cadena `{termino}`.*

- `GET /api/termino_clave` -> `200 OK` (listado activo)
- `GET /api/termino_clave/{termino}` -> `200 OK` o `404 Not Found`
- `POST /api/termino_clave` -> `201 Created`
  - Payload: `{"termino": "Deep Learning", "terminoIngles": "Deep Learning"}`
- `PUT /api/termino_clave/{termino}` -> `200 OK` o `404 Not Found`
  - Payload: `{"terminoIngles": "Deep Learning Models"}`
- `DELETE /api/termino_clave/{termino}` -> `200 OK` (borrado lógico)

---

### 2.5. Universidad (`/api/universidad`)

- `GET /api/universidad` -> `200 OK` (listado activo)
- `GET /api/universidad/{id}` -> `200 OK` o `404 Not Found`
- `POST /api/universidad` -> `201 Created`
  - Payload: `{"nombre": "Universidad Pontificia Bolivariana", "tipo": "Privada", "ciudad": "Medellín"}`
- `PUT /api/universidad/{id}` -> `200 OK` o `404 Not Found`
- `DELETE /api/universidad/{id}` -> `200 OK` (borrado lógico)

---

### 2.6. Línea de Investigación (`/api/linea_investigacion`)

- `GET /api/linea_investigacion` -> `200 OK` (listado activo)
- `GET /api/linea_investigacion/{id}` -> `200 OK` o `404 Not Found`
- `POST /api/linea_investigacion` -> `201 Created`
  - Payload: `{"nombre": "Bioinformática", "descripcion": "Aplicación de ciencias computacionales a la biología molecular"}`
- `PUT /api/linea_investigacion/{id}` -> `200 OK` o `404 Not Found`
- `DELETE /api/linea_investigacion/{id}` -> `200 OK` (borrado lógico)
