# AGENTS.md - Reglas y Convenciones del Proyecto VetSync

Este documento establece las directrices estrictas y estándares de arquitectura, diseño y codificación que todos los desarrolladores y agentes de IA deben cumplir en el proyecto **VetSync**.

---

## 1. Arquitectura del Proyecto (Clean Architecture)

El proyecto adopta **Clean Architecture** dividida en proyectos desacoplados con responsabilidades unidireccionales:

1. **VetSync.Domain**:
   - Entidades de negocio, Value Objects, Enumeraciones y Excepciones de Dominio.
   - **Regla estricta**: Cero dependencias externas o de frameworks (sin EF Core, sin ASP.NET Core).
2. **VetSync.Application**:
   - Interfaces de abstracción primarias (repositorios `ICitaRepository`, servicios `IEmailSenderService`, unidad de trabajo `IUnitOfWork`), casos de uso, DTOs, Mapeos y validaciones de negocio.
   - Depende únicamente de `VetSync.Domain`.
3. **VetSync.Infrastructure**:
   - Persistencia de datos (Entity Framework Core, SQL Server), implementaciones de repositorios, adaptadores externos y servicios simulados (ej. `EnvioCorreoSimuladoService`).
   - Depende de `VetSync.Application` y `VetSync.Domain`.
4. **VetSync.Web** (o API / Presentación):
   - Controladores MVC, ViewModels, Vistas Razor, Middlewares, Filtros y Configuración del Contenedor de Inyección de Dependencias (`Program.cs`).
   - Depende de `VetSync.Application` e `VetSync.Infrastructure`.
5. **VetSync.Tests**:
   - Pruebas unitarias y de integración siguiendo la metodología **TDD** (xUnit, Moq/NSubstitute, FluentAssertions).

---

## 2. Idioma y Nomenclatura Híbrida Dominio/Patrón

- **Entidades de Negocio y Conceptos de Dominio en Español**:
  - Las entidades, tablas y propiedades deben nombrarse en **español**.
  - Ejemplos de entidades: `Mascota`, `Dueno`, `Veterinario`, `Cita`, `HistorialMedico`, `Notificacion`.
  - Ejemplos de propiedades: `NombreCompleto`, `CorreoElectronico`, `Telefono`, `FechaHoraInicio`, `FechaHoraFin`, `EstadoCita`.
- **Sufijos Arquitectónicos y Patrones Técnicos en Inglés**:
  - Los sufijos y términos técnicos de Clean Architecture y C# se mantienen en **inglés**: `Repository`, `Service`, `Controller`, `Middleware`, `ViewModel`, `Dto`, `Command`, `Query`, `Handler`, `Exception`.
  - Ejemplos de interfaces y clases:
    - Repositorios: `ICitaRepository`, `CitaRepository`, `IMascotaRepository`, `MascotaRepository`.
    - Servicios: `ICitaService`, `CitaService`, `INotificacionService`, `NotificacionService`.
    - Controladores y Modelos: `CitaController`, `MascotaController`, `CitaViewModel`, `CrearCitaDto`.
    - Excepciones: `DominioException`, `SolapamientoCitaException`, `EntidadNoEncontradaException`.

---

## 3. Convenciones de Código y Nombres (Naming Conventions)

- **PascalCase Obligatorio para**:
  - Nombres de Clases, Records y Structs (`CitaService`, `Veterinario`, `MascotaRepository`).
  - Nombres de Métodos y Funciones (`ObtenerPorIdAsync`, `VerificarSolapamientoHorarioAsync`).
  - Propiedades Públicas y Protegidas (`Identificador`, `NombreMascota`, `FechaCreacion`).
  - Constantes (`TiempoMaximoCitaMinutos`).
- **camelCase con guion bajo (`_`) para campos privados**:
  - Variables de campo privadas: `_citaRepository`, `_notificacionService`, `_logger`.
- **camelCase para parámetros y variables locales**:
  - Parámetros de métodos: `mascotaId`, `fechaHoraInicio`.
  - Variables locales dentro de métodos: `citasExistentes`, `resultadoValidacion`.
- **Prefijo `I` para Interfaces**:
  - Toda interfaz debe comenzar con `I` en PascalCase: `ICitaRepository`, `IEmailSenderService`.

---

## 4. Uso Obligatorio de Interfaces e Inversión de Dependencias (DIP)

- **Prohibido el acoplamiento a clases concretas**:
  - Ningún controlador ni servicio debe instanciar ni recibir clases concretas en sus constructores.
  - Todo componente debe inyectarse a través de su interfaz correspondiente registrada en el contenedor de dependencias (`IServiceCollection`).
- **Segregación de Interfaces (ISP)**:
  - Definir interfaces pequeñas, cohesivas y enfocadas a una única responsabilidad. No crear interfaces monolíticas genéricas que acumulen métodos no relacionados.

---

## 5. Prohibición de `any`, `dynamic` y Tipado Débil

- **Prohibido terminantemente el uso de tipos no seguros**:
  - **No usar `dynamic`** bajo ninguna circunstancia.
  - **No usar `object`** como tipo comodín para evadir el sistema de tipos fuertemente tipado de C#.
  - **No usar variables débilmente tipadas en APIs o DTOs**: Todo payload, respuesta o modelo de vista debe tener una clase, record o struct explícitamente tipado.
  - El uso de `var` sólo está permitido cuando el tipo de la variable es evidente a la derecha de la asignación (ej. `var cita = new Cita();`), pero nunca para esconder tipos dinámicos o ambiguos.

---

## 6. Manejo Global de Errores con Middleware

- **Prohibido el uso indiscriminado de bloques `try-catch` vacíos o redundantes** en Controladores y Casos de Uso.
- Toda excepción no controlada debe ser capturada por un Middleware centralizado:
  - Nombre: `ManejadorExcepcionesMiddleware` (o `ExceptionMiddleware` en la capa Web).
  - El middleware debe interceptar excepciones de dominio (`DominioException`, `SolapamientoCitaException`, `EntidadNoEncontradaException`), registrar el incidente mediante `ILogger` con nivel de severidad adecuado, y transformar la respuesta en un formato consistente y seguro (evitando filtrar stack traces sensibles en producción).

---

## 7. Principios SOLID y Buenas Prácticas de Ingeniería

1. **S - Single Responsibility Principle (SRP)**:
   - Cada clase debe tener una única razón para cambiar. Los controladores no contienen lógica de negocio; los repositorios no envían correos; las entidades no ejecutan consultas SQL directas.
2. **O - Open/Closed Principle (OCP)**:
   - El código debe estar abierto a extensión y cerrado a modificación mediante polimorfismo, interfaces y patrones de diseño (ej. Strategy para el emisor de notificaciones).
3. **L - Liskov Substitution Principle (LSP)**:
   - Cualquier implementación de interfaz (ej. repositorios en memoria para pruebas o en base de datos) debe poder sustituirse sin romper la funcionalidad esperada.
4. **I - Interface Segregation Principle (ISP)**:
   - Clientes no deben ser forzados a depender de métodos que no utilizan.
5. **D - Dependency Inversion Principle (DIP)**:
   - Los módulos de alto nivel no deben depender de módulos de bajo nivel. Ambos deben depender de abstracciones.
6. **Programación Asíncrona (`async` / `await`)**:
   - Todo acceso a base de datos, I/O o llamadas a servicios externos debe ser completamente asíncrono utilizando `Task` y terminando en el sufijo `Async`.
   - No utilizar `.Result` ni `.Wait()` para evitar bloqueos del hilo principal (*deadlocks*).
7. **TDD (Test-Driven Development)**:
   - El código de negocio debe ser desarrollado o validado a través de pruebas automatizadas previas o concurrentes (Red -> Green -> Refactor), garantizando que las reglas clave (como evitar el choque de horarios en citas) cuenten con cobertura total de pruebas unitarias.
