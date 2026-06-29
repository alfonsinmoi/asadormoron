# Fase 0 — Setup técnico

> Cómo se monta el entorno de trabajo para el agente de voz.
> Última actualización: 2026-05-11

---

## 1. Repositorio y branching

- **Repositorio principal:** monorepo `AsadorMoron`
- **Branch base:** `main`
- **Branch del agente:** `feature/voice-agent` (creado en Fase 0)
- **Estrategia:** PRs pequeños hacia `feature/voice-agent`; al cerrar la Fase 6, merge a `main`.

```bash
git checkout main
git pull
git checkout -b feature/voice-agent
git push -u origin feature/voice-agent
```

---

## 2. Infraestructura

### Servidor único reutilizado
Se utiliza el mismo VPS que ya sirve `qoorder.com`:

- **Host:** `82.223.139.121` (Ubuntu, PHP 8.3-FPM + nginx + MariaDB local)
- **Web root:** `/var/www/qoorder/pa_ws/` (API actual)
- **Web root staging:** `/var/www/qoorder-staging/pa_ws/` (a crear)
- **Base de datos producción:** `pollo` (MariaDB local)
- **Base de datos staging:** `pollo_staging` (a crear, copia semanal de prod)

### Componentes nuevos a instalar en el servidor
- [ ] **Redis 7+** — cache de menú, rate-limiting, métricas
- [ ] **php8.3-redis** — driver
- [ ] **php8.3-curl** (ya instalado)
- [ ] **php8.3-soap** — no necesario (ya migramos paycomet a REST)
- [ ] **certbot** para `agente.qoorder.com` (subdominio del webhook)
- [ ] **bandwidth de salida** suficiente para audio (Twilio + Vapi)

Comandos instalación Redis:
```bash
apt update
apt install -y redis-server php8.3-redis
systemctl enable --now redis-server
systemctl restart php8.3-fpm
```

---

## 3. Subdominios y rutas

| Subdominio | Uso | Tipo |
|---|---|---|
| `qoorder.com/pa_ws/` | API actual (sin cambios) | Existente |
| `qoorder.com/api/` | Endpoints nuevos del agente | Nuevo |
| `staging.qoorder.com/pa_ws/` | Staging del backend | Nuevo |
| `agente.qoorder.com/webhook` | Webhook público de Vapi (HTTPS obligatorio) | Nuevo |

---

## 4. Variables de entorno

Las claves NO se hardcodean en PHP. Se guardan en `/etc/qoorder/.env` (fuera del web root, modo 600) y se leen en `config.php` con `parse_ini_file`.

Ejemplo del fichero (ver `docs/.env.example`):

```env
# Backend
APP_URL=https://qoorder.com
WEBHOOK_BASE_URL=https://agente.qoorder.com/webhook

# OpenAI
OPENAI_API_KEY=
OPENAI_MODEL=gpt-4o-mini

# Vapi
VAPI_API_KEY=
VAPI_WEBHOOK_SECRET=
VAPI_ASSISTANT_ID=

# Twilio
TWILIO_ACCOUNT_SID=
TWILIO_AUTH_TOKEN=
TWILIO_PHONE_NUMBER=

# ElevenLabs
ELEVENLABS_API_KEY=
ELEVENLABS_VOICE_ID=

# Deepgram (opcional, normalmente ya viene con Vapi)
DEEPGRAM_API_KEY=

# Redis
REDIS_HOST=127.0.0.1
REDIS_PORT=6379

# RGPD
RETENTION_AUDIO_DIAS=90
RETENTION_TRANSCRIPCION_DIAS=365

# Límites operativos
PRESUPUESTO_DIARIO_EUR=50
MAX_LLAMADAS_HORA_POR_NUMERO=3
MAX_IMPORTE_SIN_CONFIRMACION_HUMANA=100

# Concurrencia
PHP_FPM_WORKERS=30
DB_POOL_MIN=20
DB_POOL_MAX=50
```

---

## 5. Cuentas a crear (acción del cliente)

| Servicio | Plan inicial | Coste arranque | Quién lo da de alta |
|---|---|---|---|
| OpenAI | Pay-as-you-go | 0 € + uso | Pendiente |
| Vapi.ai | Pay-as-you-go (10 líneas) | 60 min gratis | Pendiente |
| Twilio | Programmable Voice | ~1 €/mes + uso | Pendiente |
| ElevenLabs | Starter | 5 €/mes | Pendiente |
| Deepgram | (incluido en Vapi) | — | — |

Documento con costes y dueño de cada cuenta: `docs/Fase0_Cuentas_Servicios.md` (a crear cuando estén dadas de alta).

---

## 6. Pruebas de carga (criterio de aceptación Fase 1)

Antes de pasar a producción se valida que la API soporta la concurrencia esperada:

```bash
# Ejemplo con k6 (instalar localmente)
k6 run --vus 20 --duration 30s tests/loadtest_crear_pedido.js
```

Objetivo:
- 20 peticiones concurrentes a `POST /api/pedido`
- p95 < 500 ms
- 0% errores

---

## 7. Plan de despliegue (resumen del plan, sección 6)

1. Fase piloto: 20-30 clientes habituales redirigidos al agente.
2. Despliegue gradual: 10% → 50% → 100% del tráfico.
3. Rollback < 5 min: cambiar webhook de Twilio al sistema actual.

---

## 8. Checklist de cierre de Fase 0

- [x] Inventario de la API actual (`Fase0_API_Existente.md`)
- [x] OpenAPI 3.0 del alcance del agente (`openapi-voice-agent.yaml`)
- [x] Cuestionario para el cliente (`Fase0_Entrevista_Cliente.md`)
- [x] Plan técnico de setup (este documento)
- [x] Plantilla `.env.example`
- [x] Branch `feature/voice-agent`
- [ ] Entrevista realizada con el cliente
- [ ] Cuentas creadas y API keys obtenidas
- [ ] Staging creado y poblado con copia de producción
- [ ] Primer "Hello World" Vapi → OpenAI
- [ ] Número Twilio activo y recibiendo llamadas a un webhook de prueba
