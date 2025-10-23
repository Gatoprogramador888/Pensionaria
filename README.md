# Sistema de Pensiones y Andenes para Camiones

## Descripción General

Este proyecto consiste en un sistema integral de gestión de pensiones y andenes utilizado por camiones de carga y descarga.
El objetivo principal es ofrecer una solución que permita administrar citas de andenes y gestionar pensiones de forma automatizada, aplicando reglas de negocio reales, manejo de tiempos, tolerancias, descuentos, cobros extras y auditorías internas.

El sistema combina lógica de negocio sólida con bases de datos relacionales (MySQL) y en memoria (Redis), simulando un entorno empresarial completo.
Está diseñado como un proyecto de portafolio, enfocado en demostrar la capacidad de integrar distintos sistemas y tecnologías dentro de una arquitectura bien estructurada.

## Funcionalidad General

### Andenes (Carga, Descarga o Ambos)
- Los clientes pueden agendar citas para reservar un anden en una fecha y hora específicas, indicando el tiempo estimado de uso.
  - Ejemplo: 10:00 a.m. - 12:00 p.m. (2 horas normales).
- Cada cita cuenta con 30 minutos de tolerancia tras el tiempo contratado.
- Si el vehículo no se retira (por falta de registro de salida o por demoras), se inicia el cobro de horas extras al doble del costo normal, sin descuentos.
- Cada media hora adicional se acumula progresivamente:
  - 30m → primera hora extra
  - 90m → segunda hora extra
  - 30 + (60 × horas) → horas extras sucesivas
- Las empresas con convenio pueden tener descuentos almacenados que se aplican automáticamente.
- Los usuarios sin cita se encolan en Redis, esperando turno hasta que un anden quede libre.

### Pensiones (Estacionamientos Prolongados)
- El sistema permite administrar pensiones diarias, semanales o mensuales, con cálculo dinámico del costo según la duración del contrato:
  - (costo_día * días) + (costo_semana * semanas) + (costo_mes * meses)
- El sistema convierte automáticamente los días excedentes en semanas o meses:
  - 12 días → 1 semana y 5 días
  - 31 días → 1 mes y 3 días
- Control de sobretiempo:
  - Si un vehículo excede 15 días del tiempo contratado, se notifica a la administración.
  - A los 30 días y cada 15 días adicionales, se envían nuevas alertas.
  - Si el vehículo no se retira, se cobra el doble del costo diario por los días excedentes sin aplicar descuentos.

## Funcionalidades Principales
- Gestión de citas y horarios para andenes.
- Cálculo automático de costos, tolerancias, descuentos y horas extras.
- Administración de pensiones con alertas por sobretiempo.
- Colas de espera en Redis para usuarios sin cita.
- Auditorías automáticas en MySQL para cambios relevantes.
- Simulación de impuestos y balances empresariales.
- API REST para la administración y consulta de información en tiempo real.

## Arquitectura del Sistema

El sistema combina un enfoque monolítico modular con principios de microservicios, lo que permitirá una futura separación independiente por dominios (andenes, pensiones, auditoría, etc.).

## Tecnologías Utilizadas
| Componente           | Tecnología / Framework         |
|----------------------|-------------------------------|
| Lenguaje principal   | C# (.NET 8)                   |
| Framework web        | ASP.NET Core                  |
| ORM                  | Entity Framework Core         |
| Base de datos        | MySQL                         |
| Cache / Cola de espera | Redis (StackExchange.Redis) |
| Logs / Auditorías    | MySQL (triggers y bitácora)   |

## Diseño del Proyecto

La estructura general del proyecto está organizada de la siguiente forma:

<!--Modificar-->
```
/Database
├── API/
├── Business/
├── Database/
├── Models/
├── Utils/
├── Program.cs
├── appsettings.json
```

```
/frontend
└── (interfaz básica de prueba)
```
<!--Modificar-->
- **API/** → Contiene los controladores ASP.NET y las rutas de la API.
- **Business/** → Lógica de negocio, validaciones, cálculos y auditorías.
- **Database/** → Contextos de Entity Framework, configuración de Redis y migraciones.
- **Models/** → Entidades ORM y DTOs usados por MySQL y Redis.
- **Utils/** → Funciones auxiliares, control de excepciones, logs, etc.

## API

La API REST desarrollada con ASP.NET Core permite:
- Consultar andenes disponibles u ocupados.
- Ver pensiones activas y liberar espacios.
- Modificar precios, descuentos o convenios.
- Mover usuarios entre andenes o pensiones.
- Notificar vehículos con más de 15 días de exceso.
- Consultar el historial de auditorías y cambios.

## Base de Datos

### MySQL
- Maneja todos los datos persistentes:
  - Citas de andenes.
  - Pensiones contratadas.
  - Empresas con descuentos.
  - Auditorías y logs.
- Incluye triggers para registrar automáticamente las modificaciones y detectar errores o anomalías.

### Redis
- Utilizado como sistema de cola (FIFO) para los usuarios sin cita.
- No almacena datos permanentes, solo mantiene el orden de llegada.
- Permite priorizar a los usuarios con cita confirmada cuando un anden queda libre.
- El esquema SQL completo se encuentra dentro del repositorio, en la carpeta correspondiente a la base de datos (`/Database/Database/MySQL`).

## Principios y Buenas Prácticas
- Código estructurado y legible, aplicando principios SOLID.
- Uso de inyección de dependencias para desacoplar componentes.
- Refactorización constante para mantener coherencia y limpieza en la arquitectura.
- Preparado para futura división en microservicios independientes.
- Separación entre capa de datos, lógica de negocio y presentación (API).

## Despliegue Local

Este proyecto no es un SaaS, sino un servicio instalable en un equipo local.
Los usuarios dentro de la misma red pueden acceder al backend mediante un frontend simple destinado a pruebas funcionales.

## Licencia

Proyecto creado con fines educativos y de portafolio personal.
El uso o distribución comercial sin consentimiento previo del autor no está permitido.
