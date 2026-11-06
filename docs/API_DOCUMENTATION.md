# Documentación Técnica de la API - VetSync

## 1. Información General y Estándares de Comunicación

- **Formato de Intercambio**: JSON (`application/json; charset=utf-8`).
- **Autenticación y Seguridad**: Sanitización HTML automática en contenido de notificaciones y codificación UTF-8.
- **Envoltura Estándar de Respuesta**: Todas las operaciones de la API utilizan el patrón `ServiceResponse<T>` para garantizar uniformidad en respuestas exitosas y fallidas.

### Esquema Base: `ServiceResponse<T>`
```json
{
  "exitoso": true,
  "datos": {},
  "mensaje": "Operacion completada exitosamente.",
  "errores": [],
  "codigoEstado": 200
}
```

### Esquema de Error de Excepción Global (`ManejadorExcepcionesMiddleware`)
```json
{
  "estatus": 409,
  "titulo": "Conflicto en horario de cita",
  "detalle": "El veterinario con ID 2 ya cuenta con una cita asignada en el intervalo solicitado.",
  "ruta": "/api/citas",
  "instanteUtc": "2026-11-05T17:00:00Z"
}
```

---

## 2. Códigos de Estado HTTP Utilizados

| Código HTTP | Significado | Escenario de Uso |
|---|---|---|
| `200 OK` | Operación exitosa | Consultas y actualizaciones completadas. |
| `201 Created` | Recurso creado | Creación de citas, mascotas, dueños e historiales. |
| `400 Bad Request` | Petición inválida | Violación de reglas FluentValidation o fechas en el pasado. |
| `404 Not Found` | Recurso no encontrado | Mascota, dueño o veterinario inexistente. |
| `409 Conflict` | Conflicto de negocio | Solapamiento de horarios de veterinarios o correo de dueño duplicado. |
| `500 Server Error` | Error interno | Excepciones no controladas capturadas por el middleware. |

---

## 3. Catálogo de Endpoints

### 3.1. Módulo de Citas (`/api/citas`)

#### A. Agendar Cita
- **Método**: `POST`
- **Ruta**: `/api/citas`
- **Descripción**: Agenda una nueva cita clínica para una mascota con un veterinario. Valida que la fecha sea futura, que la mascota y el veterinario existan (y esté activo), y que no exista solapamiento horario.
- **Cuerpo de la Petición (`CrearCitaDto`)**:
  ```json
  {
    "mascotaId": 1,
    "veterinarioId": 2,
    "fechaHoraInicio": "2026-11-10T10:00:00Z",
    "duracionMinutos": 30,
    "motivo": "Control anual de vacunas y desparasitacion"
  }
  ```
- **Respuesta Exitosa (`201 Created`)**:
  ```json
  {
    "exitoso": true,
    "datos": {
      "id": 105,
      "mascotaId": 1,
      "nombreMascota": "Rocky",
      "nombreDueno": "Laura Gomez",
      "correoDueno": "laura@mail.com",
      "veterinarioId": 2,
      "nombreVeterinario": "Dr. Carlos Perez",
      "fechaHoraInicio": "2026-11-10T10:00:00Z",
      "fechaHoraFin": "2026-11-10T10:30:00Z",
      "estado": 1,
      "motivo": "Control anual de vacunas y desparasitacion"
    },
    "mensaje": "Cita agendada exitosamente.",
    "errores": [],
    "codigoEstado": 201
  }
  ```
- **Respuestas de Error**:
  - `400 Bad Request`: Fecha en el pasado o duración fuera de rango (15-240 min).
  - `404 Not Found`: Mascota no registrada o veterinario inexistente/inactivo.
  - `409 Conflict`: El veterinario ya cuenta con una cita en el horario seleccionado (solapamiento detectado).

---

#### B. Cancelar Cita
- **Método**: `PUT`
- **Ruta**: `/api/citas/{id}/cancelar`
- **Descripción**: Cancela una cita previamente programada.
- **Respuesta Exitosa (`200 OK`)**:
  ```json
  {
    "exitoso": true,
    "datos": true,
    "mensaje": "Cita cancelada exitosamente.",
    "errores": [],
    "codigoEstado": 200
  }
  ```
- **Respuestas de Error**:
  - `404 Not Found`: Cita inexistente.
  - `400 Bad Request`: Cita ya cancelada o previamente completada.

---

#### C. Obtener Cita por ID
- **Método**: `GET`
- **Ruta**: `/api/citas/{id}`
- **Descripción**: Obtiene los detalles completos de una cita incluyendo paciente, dueño y profesional asignado.
- **Respuesta Exitosa (`200 OK`)**:
  ```json
  {
    "exitoso": true,
    "datos": {
      "id": 105,
      "mascotaId": 1,
      "nombreMascota": "Rocky",
      "nombreDueno": "Laura Gomez",
      "correoDueno": "laura@mail.com",
      "veterinarioId": 2,
      "nombreVeterinario": "Dr. Carlos Perez",
      "fechaHoraInicio": "2026-11-10T10:00:00Z",
      "fechaHoraFin": "2026-11-10T10:30:00Z",
      "estado": 1,
      "motivo": "Control anual"
    },
    "mensaje": "Operacion completada exitosamente.",
    "errores": [],
    "codigoEstado": 200
  }
  ```

---

#### D. Obtener Citas por Veterinario y Fecha
- **Método**: `GET`
- **Ruta**: `/api/citas/veterinario/{veterinarioId}?fecha={fecha}`
- **Parámetros**:
  - `veterinarioId` (ruta, entero): Identificador del profesional.
  - `fecha` (query string, formato `YYYY-MM-DD`): Día a consultar.
- **Respuesta Exitosa (`200 OK`)**:
  ```json
  {
    "exitoso": true,
    "datos": [
      {
        "id": 105,
        "mascotaId": 1,
        "nombreMascota": "Rocky",
        "veterinarioId": 2,
        "nombreVeterinario": "Dr. Carlos Perez",
        "fechaHoraInicio": "2026-11-10T10:00:00Z",
        "fechaHoraFin": "2026-11-10T10:30:00Z",
        "estado": 1,
        "motivo": "Control anual"
      }
    ],
    "mensaje": "Operacion completada exitosamente.",
    "errores": [],
    "codigoEstado": 200
  }
  ```

---

### 3.2. Módulo de Mascotas y Dueños (`/api/mascotas` y `/api/duenos`)

#### A. Registrar Dueño
- **Método**: `POST`
- **Ruta**: `/api/duenos`
- **Cuerpo de la Petición (`RegistrarDuenoDto`)**:
  ```json
  {
    "nombreCompleto": "Carlos Mendoza Ruiz",
    "correoElectronico": "carlos.mendoza@email.com",
    "telefono": "+525512345678",
    "direccion": "Av. Insurgentes Sur 1234, CDMX"
  }
  ```
- **Respuesta Exitosa (`201 Created`)**:
  ```json
  {
    "exitoso": true,
    "datos": {
      "id": 12,
      "nombreCompleto": "Carlos Mendoza Ruiz",
      "correoElectronico": "carlos.mendoza@email.com",
      "telefono": "+525512345678",
      "direccion": "Av. Insurgentes Sur 1234, CDMX"
    },
    "mensaje": "Dueno registrado exitosamente.",
    "errores": [],
    "codigoEstado": 201
  }
  ```
- **Respuestas de Error**:
  - `409 Conflict`: Ya existe un dueño registrado con dicho correo electrónico.

---

#### B. Registrar Mascota
- **Método**: `POST`
- **Ruta**: `/api/mascotas`
- **Cuerpo de la Petición (`RegistrarMascotaDto`)**:
  ```json
  {
    "duenoId": 12,
    "nombre": "Zeus",
    "especie": 1,
    "raza": "Pastor Aleman",
    "fechaNacimiento": "2022-04-15",
    "sexo": "Macho",
    "pesoKg": 32.50
  }
  ```
  *(Valores de `especie`: `1: Perro`, `2: Gato`, `3: Ave`, `4: Reptil`, `5: Otro`).*
- **Respuesta Exitosa (`201 Created`)**:
  ```json
  {
    "exitoso": true,
    "datos": {
      "id": 34,
      "duenoId": 12,
      "nombreDueno": "Carlos Mendoza Ruiz",
      "nombre": "Zeus",
      "especie": 1,
      "raza": "Pastor Aleman",
      "fechaNacimiento": "2022-04-15T00:00:00",
      "sexo": "Macho",
      "pesoKg": 32.50
    },
    "mensaje": "Mascota registrada exitosamente.",
    "errores": [],
    "codigoEstado": 201
  }
  ```
- **Respuestas de Error**:
  - `400 Bad Request`: Peso menor o igual a cero o fecha de nacimiento futura.
  - `404 Not Found`: Dueño no registrado en el sistema.

---

#### C. Obtener Mascotas por Dueño
- **Método**: `GET`
- **Ruta**: `/api/mascotas/dueno/{duenoId}`
- **Respuesta Exitosa (`200 OK`)**: Retorna arreglo de `MascotaDto`.

---

### 3.3. Módulo de Historial Médico (`/api/historiales-medicos`)

#### A. Registrar Entrada de Historial Médico
- **Método**: `POST`
- **Ruta**: `/api/historiales-medicos`
- **Cuerpo de la Petición (`RegistrarHistorialMedicoDto`)**:
  ```json
  {
    "mascotaId": 34,
    "veterinarioId": 2,
    "citaId": 105,
    "diagnostico": "Otitis externa bilateral bacteriana",
    "tratamiento": "Limpieza con solucion acida y gotas de ciprofloxacino c/12h por 10 dias",
    "observaciones": "Revision de control citologico agendada en 10 dias"
  }
  ```
- **Respuesta Exitosa (`201 Created`)**: Retorna `HistorialMedicoDto`.
- **Respuestas de Error**:
  - `404 Not Found`: Mascota o veterinario no encontrados.

---

#### B. Obtener Historial Médico por Mascota
- **Método**: `GET`
- **Ruta**: `/api/historiales-medicos/mascota/{mascotaId}`
- **Respuesta Exitosa (`200 OK`)**: Retorna lista ordenada cronológicamente de expedientes clínicos asociados al paciente.

---

### 3.4. Módulo de Notificaciones (`/api/notificaciones`)

#### A. Disparar Despacho de Recordatorios de Citas Próximas
- **Método**: `POST`
- **Ruta**: `/api/notificaciones/procesar-recordatorios`
- **Descripción**: Escanea todas las citas confirmadas o programadas que se encuentren dentro de la ventana de las próximas 24 horas y que aún no cuenten con notificación exitosa. Envía correos simulados (con sanitización contra XSS) y registra la auditoría.
- **Respuesta Exitosa (`200 OK`)**:
  ```json
  {
    "exitoso": true,
    "datos": 3,
    "mensaje": "Procesamiento de notificaciones completado. Notificaciones exitosas: 3 de 3.",
    "errores": [],
    "codigoEstado": 200
  }
  ```
