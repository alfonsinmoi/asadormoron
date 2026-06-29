# Documentación — Agente de Voz IA

Carpeta de documentos del proyecto **Agente de voz IA para Asador Morón**.

## Índice

| Documento | Fase | Estado |
|---|---|---|
| [Plan_Implementacion_Agente_Voz.md](Plan_Implementacion_Agente_Voz.md) | Plan maestro | ✅ |
| [Fase0_API_Existente.md](Fase0_API_Existente.md) | Fase 0 | ✅ |
| [openapi-voice-agent.yaml](openapi-voice-agent.yaml) | Fase 0 | ✅ |
| [Fase0_Entrevista_Cliente.md](Fase0_Entrevista_Cliente.md) | Fase 0 | ✅ plantilla, pendiente rellenar |
| [Fase0_Setup_Tecnico.md](Fase0_Setup_Tecnico.md) | Fase 0 | ✅ |
| [.env.example](.env.example) | Fase 0 | ✅ |

## Cómo navegar

1. **Plan_Implementacion_Agente_Voz.md** — fuente única de verdad de qué se hace y cuándo.
2. **Fase0_API_Existente.md** — inventario de la API actual y gaps (markdown legible).
3. **openapi-voice-agent.yaml** — spec OpenAPI 3.0 del alcance del agente (importable a Postman / Swagger UI / Insomnia).
4. **Fase0_Entrevista_Cliente.md** — cuestionario para llevar a la reunión con el cliente.
5. **Fase0_Setup_Tecnico.md** — pasos de infraestructura y cuentas.

## Estado Fase 0

Trabajo completado por el desarrollador:
- [x] Documentación de la API actual
- [x] OpenAPI del alcance
- [x] Plantilla de entrevista
- [x] Setup técnico planificado
- [x] `.env.example`
- [x] Branch `feature/voice-agent`

Bloqueado a la espera de acción del cliente:
- [ ] Entrevista con el gestor del Asador
- [ ] Validación del inventario de productos/zonas/repartidores
- [ ] Alta de cuentas (Vapi, Twilio, OpenAI, ElevenLabs)
- [ ] Aprobación del presupuesto operativo mensual

Tras esos puntos arranca **Fase 1: API y modelo de datos**.

## Visualizar el OpenAPI

```bash
# Swagger UI vía Docker
docker run -p 8080:8080 -e SWAGGER_JSON=/spec/openapi-voice-agent.yaml \
  -v $(pwd)/docs:/spec swaggerapi/swagger-ui

# o subir el yaml a https://editor.swagger.io
```
