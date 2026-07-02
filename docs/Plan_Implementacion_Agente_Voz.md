# Plan de implementación — Agente de voz IA para Asador Morón

> Documento técnico de trabajo. Recoge tareas concretas, esquemas, decisiones de arquitectura y criterios de aceptación para ejecutar el proyecto de principio a fin.

**Cliente:** Asador Morón
**Versión:** 3.0
**Última actualización:** 2026-07-01
**Duración total estimada:** 5-6 semanas (+ bloque de personalización, ver §Auditoría)
**Coste de desarrollo:** 3.800 € + IVA (21%) = 4.598 €
**Forma de pago:** 50% al inicio + 50% a la entrega

## Estado actual (2026-05-13)

| Fase | Estado | Bloqueado por |
|---|---|---|
| Fase 0 — Descubrimiento y setup | 🟢 Cerrada (Hello World OK) | Entrevista cliente: sección 5 (Reparto) rellenada; resto pendiente |
| Fase 1 — API y modelo de datos | 🟢 12/13 endpoints + Redis + logs + load tests OK | Motor de asignación (cae en Fase 3) · staging · tests unitarios |
| Fase 2 — Agente de voz MVP | 🟡 5 tools conectadas + crear_pedido funcional | Validación E2E en llamada real con tools activas |
| Fase 3 — Reparto y asignación | 🟢 Motor + distancia hechos | `asignar_repartidor()` (turno+carga+round-robin) autoasigna en pedidos de voz Envío + endpoint `asignar-repartidor.php` + push. `validar_zona` valida por **distancia real** (geocoding OSM, radio 10+2 km, acepta si el geocoder falla). Pendiente (decisión de producto): autoasignar también los pedidos de la app (hoy la app usa coger manual) |
| Fase 4 — Dashboard en app | 🟡 Vistas Llamadas + Detalle + Dashboard listas en MAUI | Audio inline · notificaciones push tiempo real · gráfico heatmap |
| Fase 5 — Robustez y casos límite | ⚪ No iniciada | Fase 2 |
| Fase 6 — Piloto y producción | ⚪ No iniciada | Fases 2-5 |
| Fase 7 — Otras configuraciones | ⚪ No iniciada | Fase 4 |

Leyenda: 🟢 hecho · 🟡 en curso · 🔴 bloqueado · ⚪ no iniciada

---

## Auditoría de completitud (2026-07-01)

> Auditoría exhaustiva del agente de voz frente al requisito real del cliente: **recibir llamadas y tomar pedidos completos** (con opciones de producto, ingredientes de pizzas/personalizables, cargo de bolsa, acento andaluz y control total del proceso). Realizada con análisis multi-agente por dimensiones + verificación adversarial sobre el código en producción (`Backend/API/api/*.php`), el prompt desplegado en Vapi y el esquema real de BD.

### Veredicto

**Completitud global: 48 %.** El agente **toma pedidos SIMPLES de forma robusta**, pero **hoy no puede tomar pedidos correctos**: no maneja opciones (pollo entero/medio/cuarto), no maneja ingredientes (pizza sin cebolla / doble queso) y no cobra la bolsa. En pedidos personalizados **cobra de menos** y **manda a cocina información no estructurada**. No es apto para producción profesional hasta cerrar esa brecha de personalización y precio.

Lo que **sí** funciona y está verificado en producción: recepción de llamada (Twilio + Vapi gpt-4o), saludo y tono andaluz, búsqueda de menú fuzzy sobre productos reales, validación de zona, ETA determinista server-side según saturación, resumen con precio en palabras, creación transaccional del pedido (`qo_pedidos` + `qo_pedidos_detalle` + `qo_pedidos_estado`), código deletreado para TTS y push al staff. La base es sólida; la brecha es de **personalización y blindaje de precio**.

### Estado por dimensión

| # | Dimensión | Estado | % | Nota |
|---|-----------|--------|---|------|
| 1 | Control del flujo de pedido (end-to-end) | 🟡 parcial | 62 | Happy path completo; sin personalización ni validación de precio server-side en `crear_pedido` |
| 2 | **Opciones de producto** (entero/medio/cuarto) | 🟡 parcial | 20 | BD las tiene (`qo_productos_opc`), pero ninguna tool las expone ni el precio las suma |
| 3 | **Ingredientes** (añadir/quitar, pizzas) | 🔴 ausente | 3 | `qo_productos_ing`/`qo_ingredientes_producto` existen; el agente no tiene tool ni lógica |
| 4 | **La bolsa** (cargo por embalaje) | 🔴 ausente | 0 | Sin config, sin línea, sin prompt. No se puede cobrar |
| 5 | Reconocimiento de acento andaluz | 🟡 parcial | 28 | Solo heurística en el prompt + keywords Deepgram; sin normalización server-side ni métricas |
| 6 | Resolución de producto y exactitud de precio | 🟡 parcial | 62 | `resumen_pedido` valida precio BASE contra BD; `crear_pedido` confía en el precio del LLM |
| 7 | Robustez operativa y control | 🟡 parcial | 64 | Webhook/blacklist/rate-limit/RGPD/health OK; falta idempotencia real y transferencia en vivo |
| 8 | Telefonía e infraestructura | 🟡 parcial | 52 | Número Twilio **+1 US** puede ser rechazado por operadores ES; falta número español |

### Gaps críticos (bloquean tomar pedidos correctos hoy)

1. **Opciones inexistentes de extremo a extremo (impacto máximo).** `qo_productos_opc` tiene entero/medio/cuarto con `valorIncremento`, pero no hay tool `get_opciones`, `get_menu` no las expone, el prompt no las pregunta, `resumen_pedido` no suma el incremento y `crear_pedido` no las persiste. El cliente pide "pollo entero" → se cobra el precio base y cocina recibe un producto genérico. Es el requisito #1 y está roto.
2. **Persistencia imposible de opciones.** `qo_pedidos_detalle` **no tiene columna `opcion`/`idOpcion`** (solo `id, idPedido, idProducto, precio, estado, cantidad, tipo, concepto, comentario, tipoVenta, pagadoConPuntos`). Aunque el LLM capture la opción, no hay dónde guardarla estructurada. La app móvil (`CarritoModel.opcion`) sí la modela → brecha directa voz vs app.
3. **Ingredientes add/quita inexistentes.** `qo_productos_ing` (incremento) y `qo_ingredientes_producto` (precio) existen, pero no hay tool ni lógica. "Pizza sin cebolla con doble queso" se pierde: sin validación (el LLM puede alucinar ingredientes), sin cobro del suplemento, sin estructura para cocina y **sin registro de alérgenos (riesgo legal)**. El prompt incluso fuerza "X CON Y = DOS PRODUCTOS", partiendo mal la pizza personalizada.
4. **Exactitud de precio sin blindaje.** `resumen_pedido` hace lookup del precio BASE (bien contra alucinación de base) pero ignora opciones e ingredientes; `crear_pedido` usa el precio que llega en la línea sin recomputar. El total dicho al cliente y el guardado pueden divergir, sin reconciliación ni log. Erosión de margen en cada pedido personalizado.
5. **Cargo de bolsa no modelado.** Cero rastro de `gastos_bolsa_eur` en config ni lógica de línea. Requisito del cliente → pérdida directa de ingresos por pedido.
6. **ETA se rompe con personalización.** `tiempo_estimado_minutos` cuenta pollos por `strpos('pollo')` en el `concepto`. Con "pollo asado entero sin piel" el conteo puede fallar y la ETA queda mal calibrada.
7. **Acento andaluz sin capa de normalización ni métricas.** Los mapeos (bollo→pollo) viven solo como heurística del prompt; `webhook-vapi` guarda la transcripción cruda sin normalizar, sin score de confianza, sin fallback por baja confianza y sin corpus de test. Riesgo de pedidos perdidos **no medible**.

### Otros hallazgos (media/baja prioridad)

- **Inconsistencia de gastos de envío entre endpoints:** `tool_crear_pedido` añade la línea de envío automáticamente; `crear-pedido.php` (app) no. Comportamiento distinto según el endpoint.
- **`tool_get_slots_recogida` es código muerto:** el prompt nunca pregunta hora, nunca se invoca.
- **Blacklist se comprueba después del importe** en `crear-pedido.php` (debería ir antes).
- **Transferencia a humano sin transición en vivo:** el agente dice "llame al 626692828" y cuelga; Vapi no hace `<Dial>` real.
- **Auto-blacklist mal calibrado** para producción (umbral 50 fallidas/7d, era para desarrollo).
- **Prompt versionado en `/tmp`** en vez de en git.

---

## Bloque de cierre — Personalización y profesionalización (roadmap)

> Seis fases (P1–P6) para pasar del 48 % a un sistema **completo y profesional**. P1–P4 son **bloqueantes** para tomar pedidos correctos; P5–P6 profesionalizan. Nomenclatura P# para no confundir con las Fases 0–7 originales.

### Estado de ejecución (2026-07-01)

| Fase | Estado | Notas |
|---|---|---|
| P1 — Cimientos de datos | 🟢 hecho | Migración 002 aplicada (`idOpcion` + `ingredientes_json` + índice); contrato de línea; `tool_crear_pedido` write-ready; verificado transaccional |
| P2 — Tools de personalización | 🟢 hecho | `get_opciones`, `get_ingredientes`, `get_menu` con flags `opc/ing/nIng`; registradas en Vapi; keywords Deepgram |
| P3 — Precio server-side + bolsa | 🟢 hecho | `precio_linea_autoritativo()` compartido; opción absoluta + ingredientes; **bolsa calculada como la app** (`qo_configuracion_est`, nBolsas×precioBolsa, `tipo=4`); log de divergencia; ETA por idProducto |
| P4 — Prompt de personalización | 🟢 hecho | `vapi_prompt_v27.txt` versionado y desplegado; árbol opciones/ingredientes; deshecho "X con Y = 2 productos" |
| P5 — Acento andaluz | 🟢 hecho | `get_menu` tolerante (`normalizar_andaluz`) + **métricas en dashboard** (tasa de reconocimiento vía búsquedas, transcripción normalizada, migración 003). Corpus de regresión etiquetado queda como mejora futura opcional |
| P6 — Robustez e infraestructura | 🟡 parcial | Quick-wins técnicos hechos; quedan ítems de decisión del cliente |

**P6 — hecho (quick-wins técnicos, sin dependencia del cliente):**

- ✅ **Idempotencia del webhook**: dedupe de transcripción (los reintentos de Vapi ya no duplican filas).
- ✅ **Auto-blacklist configurable**: umbrales en `qo_config_agente` (`autoblacklist_fallidas=12`, `autoblacklist_ventana_h=24`); nunca bloquea si hubo un pedido en la ventana. Antes: 50/7d fijo (valor de desarrollo).
- ✅ **Test E2E del pedido personalizado**: `Backend/API/tests/e2e_pedido_personalizado.sh` (verificado contra producción: opción + ingredientes + precio autoritativo persistidos, con limpieza).

**Hardening pre-producción (revisión adversarial multi-agente, 2026-07-02):** el revisor marcó "no apto" con 2 críticos + 5 altos; **todos corregidos y verificados**:
- **Idempotencia de `crear_pedido` por callId** (no duplica pedidos) + índice UNIQUE en `qo_llamadas.pedido_id` (migración 004).
- **ETA unificada** (`eta_minutos()`) entre resumen/crear y slots (defaults y saturación coherentes).
- **Sin fuga de excepciones** en respuestas HTTP (crear_pedido, tool-call, dashboard) — solo en logs.
- **Métricas filtradas por establecimiento** (y arreglado un PDO error latente al pasar `idEst`).
- **Auditoría de ingredientes** (precio real + efecto), **cantidad/ingredientes validados**, header X-Vapi redactado, redondeo de bolsa coherente.
- Verificado E2E: doble tool-call del mismo callId → mismo pedido; cantidad 0 e ingredientes malformados → error controlado.
- Diferido (bajo, futuro): columna hash UNIQUE en transcripciones, `FOR UPDATE` en contador de pollos, política fail-closed del rate-limit sin Redis.

**P6 — pendiente (necesita decisión/gestión del cliente):**

- **Importe de la bolsa**: ✅ resuelto — el agente calcula la bolsa **igual que la app** leyendo de `qo_configuracion_est` (`precioBolsa`=col `tieneMediaPizza`, `rangoBolsas`=col `idCategoriaPizza`): `nBolsas=floor(total/rango)` (mín 1) × `precioBolsa`, línea `tipo=4`. Hoy `precioBolsa=0` para el est. 67 → no cobra; cuando el cliente fije el precio en la config del local, se aplicará automáticamente en app **y** voz.
- **Número de teléfono español (+34)**: el actual es Twilio **+1 US**, que operadores españoles pueden rechazar. Comprar/portar número ES con Regulatory Bundle (DNI + dirección, 1–3 días) es una gestión del cliente.
- **Transferencia a humano en vivo**: hoy el agente da el 626692828 y cuelga; conectar la llamada requiere configurar `<Dial>`/transferCall (técnico, pero conviene validar con el cliente el número destino y horario).

### Contrato de datos canónico (compartido voz ↔ app)

Toda línea de pedido —la genere la app o el agente— debe poder representarse igual:

```jsonc
{
  "idProducto": 123,
  "cantidad": 1,
  "idOpcion": 45,                 // null si el producto no tiene opciones
  "ingredientes": [               // [] si no aplica
    { "idIngrediente": 9,  "esAnadir": true,  "precio": 1.50 },   // doble queso
    { "idIngrediente": 12, "esAnadir": false, "precio": 0.00 }    // sin cebolla
  ],
  "precio": 12.90,                // precio_final REcalculado server-side (no del LLM)
  "concepto": "Pollo asado entero, sin piel",
  "comentario": ""
}
```

`precio_final = precio_base(qo_productos_est) + valorIncremento(qo_productos_opc) + Σ precio(ingredientes añadidos)`. **El servidor siempre recalcula**; el precio del LLM solo es fallback.

### P1 — Cimientos de datos (BLOQUEANTE)

**Objetivo:** que BD y escritura del pedido representen un pedido completo idéntico al de la app.

```sql
ALTER TABLE qo_pedidos_detalle
  ADD COLUMN idOpcion INT NULL AFTER idProducto,          -- alinea con CarritoModel.opcion
  ADD COLUMN ingredientes_json JSON NULL AFTER concepto;  -- detalle estructurado add/quita
```

- Definir el contrato canónico de línea (arriba) y documentarlo en `Backend/API/docs/`.
- Unificar la escritura de líneas en un helper común usado por `crear-pedido.php` (app) y `tool_crear_pedido` (voz) para que **no divergan**.
- Revisar triggers de `qo_pedidos_detalle` para que sigan reaccionando a `tipo=1` (envío) y no rompan con las columnas nuevas ni con la futura línea de bolsa.

**Entregable:** migración aplicada + contrato documentado.

### P2 — Tools de personalización y `get_menu` enriquecido

**Objetivo:** dar al LLM herramientas para descubrir y validar opciones/ingredientes reales (adiós alucinación).

- `tool_get_opciones(idProducto)` → `SELECT id, opcion, tipoIncremento, valorIncremento FROM qo_productos_opc WHERE idProducto=?` (devuelve `[]` si no hay). Registrar en el dispatcher (`agent-tools.php`) y en el assistant de Vapi.
- `tool_get_ingredientes(idProducto)` → une `qo_productos_ing` + `qo_ingredientes_producto` → `[{id, nombre, precio_incremental, esConfigurable}]`. Registrar en dispatcher y Vapi.
- Ampliar `tool_get_menu`: añadir por producto `tieneOpciones`/`tieneIngredientes` (vía `EXISTS(...)`) y `numeroIngredientes`, para que el agente sepa **cuándo preguntar** sin llamadas extra.
- Keywords Deepgram para `entero/medio/cuarto/sin sal/sin cebolla/sin piel/doble`.

**Entregable:** dos tools nuevas + `get_menu` enriquecido + tools registradas en Vapi.

### P3 — Precio server-side blindado (opciones + ingredientes + bolsa)

**Objetivo:** que el total dicho == total guardado, validado y anti-fraude, con bolsa.

- Reescribir `tool_resumen_pedido` para recibir `idOpcion` e `ingredientes` por línea y calcular `precio_final` server-side (fórmula arriba). Ignorar el precio del LLM salvo fallback.
- Reescribir `tool_crear_pedido` para **revalidar `idOpcion` e ingredientes contra BD ANTES del INSERT**; si no existen, devolver `error + hint` para forzar reintento del LLM; recomputar total y persistir `idOpcion` + `ingredientes_json`.
- Añadir `gastos_bolsa_eur` a `qo_config_agente`; sumarla en `resumen_pedido` e insertarla en `crear_pedido` como línea `tipo=2` (idProducto=0), en paralelo a la de envío `tipo=1`. Documentar: `tipo=0` producto / `1` envío / `2` bolsa.
- **Log de divergencia de precio:** tras el INSERT, si `|precio_esperado − precio_guardado| > 0.01` → WARN con `pedido_id`, `idProducto`, `concepto`, diferencia.
- Corregir `tiempo_estimado_minutos`: detectar pollos por `idProducto`/categoría (no por `strpos` en `concepto`).

**Entregable:** precios exactos server-side, bolsa cobrada, ETA robusta, auditoría de divergencias.

### P4 — Prompt e interacción de personalización

**Objetivo:** que el agente pregunte opciones/ingredientes con naturalidad y comunique precio y bolsa sin ambigüedad.

- Sección **OPCIONES**: tras `get_menu`, si `tieneOpciones`, llamar `get_opciones` en silencio y preguntar "¿Entero, medio o cuarto?". Nunca asumir default ni inventar tamaños.
- Sección **INGREDIENTES**: si `tieneIngredientes`, ofrecer add/quita; si el cliente pide un ingrediente que `get_ingredientes` no devuelve, decir que no está disponible en vez de improvisar.
- **Retirar/matizar** la regla "X CON Y = DOS PRODUCTOS": la pizza personalizada es **UNA línea con ingredientes**, no varias.
- `resumen_pedido` incluye opción e ingredientes en el texto literal ("Un pollo asado entero, sin piel. Total con envío y bolsa: …"). Añadir mensaje cordial para importe > umbral.
- **Versionar el prompt en git** (`Backend/API/docs/vapi_prompt_vN.txt`) con procedimiento de despliegue a Vapi (no en `/tmp`).

**Entregable:** prompt nuevo versionado y desplegado con árbol de decisión de opciones/ingredientes/bolsa.

### P5 — Acento andaluz: normalización, métricas y test

**Objetivo:** reducir pedidos perdidos por transcripción y hacerlo medible.

- `normalize_transcript_andaluz()` en `_lib.php` (bollo/rollo→pollo, patada→patatas, choco→chocos, ganbarjillo→gambas al ajillo…) aplicada en `webhook-vapi` **antes de guardar**; columna `texto_normalizado` + auditoría original vs normalizado.
- Validar keywords Deepgram (boost pollo/patatas, penalty bollo/patada) y ajustar endpointing/VAD para elisiones andaluzas.
- Fuzzy match acento-aware en `tool_get_menu` (acepta término original y normalizado); re-pregunta si confianza baja o cero resultados.
- Corpus de 50–100 muestras andaluzas + job mensual que mida precisión (%) con alerta si baja de 90 %; métrica en el dashboard.

**Entregable:** capa de normalización + métricas de acento + corpus de regresión.

### P6 — Robustez operativa e infraestructura profesional

**Objetivo:** cerrar riesgos no funcionales antes de escalar volumen.

- **Idempotencia real:** `webhook_request_id UNIQUE` en `qo_llamadas` + tabla `qo_webhook_events`; reprocesar un evento no duplica datos.
- **Número Twilio español +34** con Regulatory Bundle (el +1 US actual puede ser rechazado por operadores ES); documentar el switchover de `phoneNumberId`.
- **Transferencia a humano real:** Twilio `<Dial timeout>` al 626692828 con fallback a buzón en la misma llamada; corregir URL de grabación en `voicemail.php`.
- Ajustar auto-blacklist (de 50/7d a ~15/24h), timeout global en `dispatch_tool`, reset diario documentado de `qo_contador_pollos`, whitelist VIP para bypass de importe.
- **Test E2E integral:** "pollo medio sin cebolla + pizza sin cebolla con doble queso, envío con bolsa" validando `get_opciones`, `get_ingredientes`, `resumen` (precio y texto), `crear_pedido` (`idOpcion` + `ingredientes_json` + línea bolsa) y coherencia con lo que ve cocina y la app.

**Entregable:** sistema idempotente, telefonía española, transferencia con fallback, controles calibrados y suite E2E del pedido personalizado. **Apto para producción profesional.**

---

## Decisiones operativas confirmadas por el cliente (2026-05-13)

Respuestas extraídas de [`Cuestionario para el cliente.md`](Cuestionario%20para%20el%20cliente.md):

**§5 Reparto:**

- **Cobertura: TODAS las zonas.** No hay zonas excluidas. → El agente aceptará cualquier dirección dentro del radio. El endpoint `validar-zona` se comporta como "siempre válida" mientras la dirección esté dentro del radio.
- **CP base: 41530** (Morón de la Frontera) — radio máximo de reparto: **10 km** desde el local.
- **Sin recargo en hora punta ni festivos** → `gastos` único por zona, sin lógica de multiplicador horario.

**Implicaciones técnicas inmediatas:**

- `tool_validar_zona` ya hace fallback aceptando cualquier dirección con la zona por defecto: comportamiento alineado.
- Cuando integremos geocoding (Fase 3), añadiremos un filtro por distancia ≤ 10 km al local en lugar de match por nombre de zona.
- Prompt del agente: NO mencionar restricciones de zona (ya aplicado en v4 y v5).

---

## Índice

1. [Visión general](#1-visión-general)
2. [Stack y servicios](#2-stack-y-servicios)
3. [Atención simultánea de llamadas](#3-atención-simultánea-de-llamadas)
4. [Fase 0 — Descubrimiento y setup](#fase-0--descubrimiento-y-setup-semana-1)
5. [Fase 1 — API y modelo de datos](#fase-1--api-y-modelo-de-datos-semana-1-2)
6. [Fase 2 — Agente de voz MVP](#fase-2--agente-de-voz-mvp-semanas-2-3)
7. [Fase 3 — Reparto y asignación de repartidores](#fase-3--reparto-y-asignación-de-repartidores-semanas-3-4)
8. [Fase 4 — Dashboard y transcripciones en la app](#fase-4--dashboard-y-transcripciones-en-la-app-semana-4)
9. [Fase 5 — Robustez y casos límite](#fase-5--robustez-y-casos-límite-semana-5)
10. [Fase 6 — Piloto y puesta en producción](#fase-6--piloto-y-puesta-en-producción-semanas-5-6)
11. [Fase 7 — Otras configuraciones de la app](#fase-7--otras-configuraciones-de-la-app-semana-6)
12. [Anexos](#anexos)

---

## 1. Visión general

### Objetivo

Desplegar un agente conversacional que atienda llamadas entrantes al Asador Morón, recoja pedidos, los introduzca en el sistema existente, asigne repartidores automáticamente y permita gestionar todo desde la app .NET MAUI ya en producción.

### Alcance

**Incluido:**
- Atención de llamadas en español con voz natural.
- Atención simultánea de varias llamadas en paralelo.
- Toma de pedidos de recogida y reparto.
- Asignación automática de repartidor según reglas configurables.
- Cálculo automático de hora estimada de entrega.
- Almacenamiento de transcripciones y audios.
- Dashboard de métricas integrado en la app.
- Configuración del agente desde la app del gestor.
- Confirmación al cliente mediante el email que ya envía la app actual.

**Fuera de alcance (futuras iteraciones):**
- Pagos online por voz.
- Llamadas salientes (campañas).
- Multi-idioma más allá de español.
- Integración con sistemas de fidelización.

### Hitos principales

| Hito | Semana | Entregable |
|---|---|---|
| M1 — Setup completo | 1 | API documentada, cuentas creadas, número Twilio reservado |
| M2 — API lista | 1-2 | Backend PHP con todos los endpoints y webhooks |
| M3 — MVP de voz | 3 | Agente atendiendo llamadas y creando pedidos |
| M4 — Reparto operativo | 4 | Asignación automática de repartidores funcionando |
| M5 — App actualizada | 4-5 | Dashboard, configuración y visor de transcripciones |
| M6 — Producción | 5-6 | Sistema en uso real con clientes |

### Coste por fase

| Fase | Importe |
|---|---|
| Fase 0 — Descubrimiento y setup | Sin coste |
| Fase 1 — API y modelo de datos | 500 € |
| Fase 2 — Agente de voz (MVP) | 1.600 € |
| Fase 3 — Reparto y asignación de repartidores | 300 € |
| Fase 4 — Dashboard y transcripciones en la app | 300 € |
| Fase 5 — Robustez y casos límite | 700 € |
| Fase 6 — Piloto y puesta en producción | 200 € |
| Fase 7 — Otras configuraciones de la app | 200 € |
| **TOTAL** | **3.800 €** |

---

## 2. Stack y servicios

| Capa | Servicio | Modelo / versión |
|---|---|---|
| Telefonía | Twilio | Programmable Voice (número español geográfico) |
| Orquestación de voz | Vapi.ai | Pay-as-you-go (10 líneas simultáneas incluidas) |
| LLM | OpenAI | `gpt-4o-mini` |
| Speech-to-Text | Deepgram | Nova-2 (vía Vapi) |
| Text-to-Speech | ElevenLabs | Voz española multilingüe |
| Backend | PHP existente | + nuevos módulos |
| Cache / rate-limit | Redis | 7+ |
| Almacenamiento audio | S3 o compatible | Retención 90 días (RGPD) |
| App | .NET MAUI existente | + nuevas vistas |
| Tiempo real | SignalR o SSE | Notificaciones a la app |

### Coste operativo por llamada (3 min media)

| Componente | Coste |
|---|---|
| Twilio | ~0,03 € |
| Vapi.ai | ~0,15 € |
| OpenAI gpt-4o-mini | ~0,01 € |
| Deepgram | ~0,03 € |
| ElevenLabs | ~0,12 € |
| **Total por pedido completado** | **~0,35 €** |

---

## 3. Atención simultánea de llamadas

### Comportamiento por defecto

El sistema **atiende varias llamadas en paralelo sin colas ni buzón**. Si el cliente A está pidiendo mientras llama el cliente B, la llamada de B se contesta inmediatamente por otra instancia del agente.

Cada llamada es una sesión completamente aislada: tiene su propia transcripción, su propio contexto conversacional y su propio identificador único. No hay riesgo de "mezclar" pedidos entre clientes simultáneos.

### Límites

- **Twilio**: sin límite práctico de llamadas concurrentes en un número estándar.
- **Vapi**: **10 líneas simultáneas incluidas** en el plan pay-as-you-go. Cada línea adicional cuesta 10 $/mes.
- **API PHP**: cuello de botella real. Debe soportar 10-20 peticiones concurrentes (pruebas de carga en fase 1).
- **MySQL/PostgreSQL**: pool de conexiones dimensionado para concurrencia (mínimo 20 conexiones).

### Recomendaciones operativas

- Arrancar con las 10 líneas incluidas.
- Monitorizar el primer mes el pico de llamadas concurrentes (métrica en el dashboard).
- Si en hora pico (domingos al mediodía) se llega al límite con frecuencia, ampliar a 15-20 líneas.
- Configurar alerta automática si se rechaza alguna llamada por límite de concurrencia.

### Dimensionamiento del backend PHP

Para soportar la concurrencia:

- PHP-FPM con al menos 30 workers.
- Pool de conexiones a BD con mínimo 20 simultáneas.
- Redis para rate-limiting y caché de menú/configuración (evita golpear la BD en cada llamada).
- Endpoints idempotentes (especialmente el webhook receiver).
- Timeouts ajustados: respuesta del backend < 500 ms para no degradar la conversación.

---

## Fase 0 — Descubrimiento y setup (semana 1)

**Coste:** Sin coste
**Objetivo:** dejar todo preparado para empezar a desarrollar sin bloqueos.

### Tareas

#### Documentación de la API existente
- [x] Capturar tráfico real de la app .NET MAUI con mitmproxy o Charles. _(hecho extrayendo URLs del código C# directamente, más rápido que mitmproxy)_
- [x] Generar especificación OpenAPI/Swagger de los endpoints actuales. _([docs/openapi-voice-agent.yaml](openapi-voice-agent.yaml))_
- [x] Identificar qué endpoints faltan o necesitan ajustes. _([docs/Fase0_API_Existente.md](Fase0_API_Existente.md))_
- [ ] Validar el inventario con el cliente.

#### Entrevistas con el cliente y el personal
- [ ] Preguntas frecuentes que hacen los clientes al teléfono.
- [ ] Errores más comunes en la toma manual de pedidos.
- [ ] Horas pico (días, franjas horarias).
- [ ] Capacidad real de la cocina por franjas (pollos cada 15 minutos).
- [ ] Lista de productos con sus modificadores reales.
- [ ] Zonas de reparto y tiempos típicos por zona.
- [ ] Lista de repartidores, vehículos y horarios.
- [ ] Confirmación de que el email automático actual cubre las necesidades de notificación.

#### Cuentas y accesos
- [x] Crear cuenta Vapi.ai (60 minutos gratuitos iniciales). _(private + public keys en `/etc/qoorder/agente.env`)_
- [x] Crear cuenta Twilio y reservar número español geográfico (idealmente de Morón). _(cuenta `Qoorder` Full, número `+14156495668` US para dev; nº español requiere Regulatory Bundle, lo dejamos para Fase 6)_
- [x] Crear cuenta OpenAI y obtener API key. _(key dedicada al agente, gpt-4o-mini)_
- [x] Crear cuenta ElevenLabs y elegir voz española de prueba. _(Free tier con Bella como voz activa; Cristina B andaluz lista para usar tras upgrade a Starter)_
- [x] Configurar acceso al repositorio del backend PHP. _(acceso SSH a `82.223.139.121` ya operativo)_

#### Entorno de desarrollo
- [x] Branch `feature/voice-agent` en el repositorio.
- [x] Variables de entorno para claves API. _([docs/.env.example](.env.example))_
- [ ] Entorno de staging del backend PHP separado de producción.
- [ ] Base de datos de staging poblada con datos realistas.

### Criterio de aceptación
- [x] OpenAPI completo de la API existente en el repositorio. _([openapi-voice-agent.yaml](openapi-voice-agent.yaml))_
- [ ] Documento con respuestas a todas las preguntas al cliente. _(plantilla lista en [Cuestionario para el cliente.md/docx](Cuestionario%20para%20el%20cliente.md), falta la reunión)_
- [x] Número Twilio activo y recibiendo llamadas (a un test webhook). _(`+14156495668` con voice_url apuntando a Vapi; webhook propio en `/api/webhook-vapi.php` con HMAC validado)_
- [x] Primer "Hello World" desde Vapi → OpenAI respondido en consola. _(2026-05-12: llamada outbound completa con pedido conversacional. Stack OpenAI gpt-4o-mini + ElevenLabs Bella + Deepgram nova-2 + Vapi + Twilio funcionando E2E)_

---

## Fase 1 — API y modelo de datos (semana 1-2)

**Coste:** 500 €
**Objetivo:** dejar el backend listo para que el agente pueda leer y escribir todo lo que necesita.

### Modelo de datos — tablas nuevas

```sql
-- Llamadas entrantes
CREATE TABLE llamadas (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    vapi_call_id VARCHAR(100) UNIQUE NOT NULL,
    telefono_origen VARCHAR(20),
    cliente_id BIGINT NULL,
    estado ENUM('en_curso','completada','transferida','no_pedido','fallida') NOT NULL,
    pedido_id BIGINT NULL,
    duracion_segundos INT,
    coste_estimado DECIMAL(8,4),
    audio_url VARCHAR(500),
    fecha_inicio DATETIME NOT NULL,
    fecha_fin DATETIME NULL,
    metadatos JSON,
    INDEX idx_fecha (fecha_inicio),
    INDEX idx_estado (estado),
    INDEX idx_telefono (telefono_origen)
);

-- Transcripciones
CREATE TABLE transcripciones (
    id BIGINT PRIMARY KEY AUTO_INCREMENT,
    llamada_id BIGINT NOT NULL,
    texto LONGTEXT NOT NULL,
    texto_estructurado JSON,  -- turnos: [{rol, texto, timestamp}]
    fecha DATETIME NOT NULL,
    FOREIGN KEY (llamada_id) REFERENCES llamadas(id) ON DELETE CASCADE
);

-- Reglas de asignación de repartidores
CREATE TABLE reglas_asignacion (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100) NOT NULL,
    prioridad INT NOT NULL,
    tipo ENUM('zona','carga','vehiculo','turno','round_robin') NOT NULL,
    parametros JSON NOT NULL,
    activa BOOLEAN DEFAULT TRUE,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- Números bloqueados
CREATE TABLE blacklist_telefonos (
    telefono VARCHAR(20) PRIMARY KEY,
    motivo VARCHAR(255),
    fecha DATETIME DEFAULT CURRENT_TIMESTAMP,
    bloqueado_por VARCHAR(100)
);

-- Configuración global del agente
CREATE TABLE config_agente (
    clave VARCHAR(100) PRIMARY KEY,
    valor JSON NOT NULL,
    fecha_actualizacion DATETIME DEFAULT CURRENT_TIMESTAMP
);
```

### Cambios en tablas existentes

```sql
ALTER TABLE pedidos
    ADD COLUMN origen ENUM('app','voz','manual','web') DEFAULT 'manual',
    ADD COLUMN llamada_id BIGINT NULL,
    ADD COLUMN hora_estimada DATETIME NULL,
    ADD COLUMN repartidor_id BIGINT NULL,
    ADD INDEX idx_origen (origen);
```

### Endpoints nuevos

| Estado | Método | Ruta | Función |
|---|---|---|---|
| ✅ | `POST` | `/api/webhook-vapi.php` | Recibe eventos de Vapi (firmados con HMAC) |
| ⚪ | `GET` | `/api/menu` | Productos disponibles ahora con stock real (existe `productos.php`) |
| ✅ | `GET` | `/api/cliente.php?telefono=` | Identifica al cliente por número |
| ✅ | `POST` | `/api/crear-pedido.php` | Crea pedido (ampliado con `origen=voz` y `llamada_id`) |
| ✅ | `GET` | `/api/estado-local.php` | Abierto/cerrado/saturado + horario |
| ✅ | `GET` | `/api/slots-recogida.php` | Slots disponibles por carga real |
| ✅ | `GET` | `/api/validar-zona.php` | Valida si una dirección está en zona |
| ⚪ | `GET` | `/api/repartidores/asignar` | Devuelve repartidor óptimo (motor de reglas — Fase 3) |
| ✅ | `GET/POST/PUT/DELETE` | `/api/reglas-asignacion.php` | CRUD completo de reglas |
| ✅ | `GET` | `/api/llamadas.php` | Listado paginado + detalle por id |
| ✅ | `GET` | `/api/dashboard-metricas.php` | KPIs agregados (cache APCu 5 min) |
| ✅ | `GET/POST/DELETE` | `/api/blacklist.php` | Gestión de números bloqueados |
| ✅ | `GET/PUT` | `/api/config-agente.php` | Lectura/actualización de la configuración |

### Tareas

#### Migraciones y modelos
- [x] Crear migraciones SQL con las tablas y alteraciones. _([Backend/BD/migrations/001_voice_agent_tables.sql](../Backend/BD/migrations/001_voice_agent_tables.sql) — aplicada en producción)_
- [ ] ~~Modelos / clases del ORM PHP correspondientes.~~ _(no aplica: el backend usa PDO directo, no ORM)_
- [ ] Seeds con datos de ejemplo para staging. _(staging pendiente)_

#### Endpoints CRUD básicos
- [ ] `GET /api/menu` con stock real desde almacén. _(reutilizamos `productos.php` actual; queda añadir flag `disponible`)_
- [x] `GET /api/cliente` por teléfono. _([Backend/API/api/cliente.php](../Backend/API/api/cliente.php))_
- [x] `POST /api/pedido` con campos nuevos. _([Backend/API/api/crear-pedido.php](../Backend/API/api/crear-pedido.php) — endpoint dedicado, evita tocar pedidos.php existente)_
- [x] `GET /api/estado-local`. _([Backend/API/api/estado-local.php](../Backend/API/api/estado-local.php))_
- [x] `GET /api/slots-recogida` con lógica de carga de cocina. _([Backend/API/api/slots-recogida.php](../Backend/API/api/slots-recogida.php))_
- [x] `GET /api/zonas-reparto`. _(reutilizamos `zonas.php` actual + [validar-zona.php](../Backend/API/api/validar-zona.php) nuevo)_

#### Webhook receiver
- [x] Endpoint `POST /api/webhook/vapi` con validación HMAC. _([Backend/API/api/webhook-vapi.php](../Backend/API/api/webhook-vapi.php))_
- [x] Parser de eventos: `call-start`, `call-end`, `function-call`, `transcript-final`.
- [x] Persistencia idempotente (mismo evento procesado dos veces no duplica). _(usa `ON DUPLICATE KEY UPDATE` por `vapi_call_id` único)_
- [ ] Reintentos con backoff exponencial. _(Vapi reintenta del lado emisor; queda añadir cola interna por si falla la BD)_

#### Seguridad y operación
- [x] Rate limiting básico por IP (Redis). _(Redis 7 instalado en `82.223.139.121` + `php8.3-redis`; helper en [_lib.php](../Backend/API/api/_lib.php); 20 pedidos/min IP en crear-pedido, 120 eventos/min IP en webhook-vapi)_
- [x] Logs estructurados (JSON) con identificador de llamada. _(`agente_log()` en [_lib.php](../Backend/API/api/_lib.php); usado en crear-pedido y webhook-vapi)_
- [x] Variables de entorno para todas las claves. _([docs/.env.example](.env.example))_
- [x] Pruebas de carga: simular 20 peticiones concurrentes a `crear_pedido`. _(50 req / 20 conc → p95 195 ms, p99 232 ms, 0 errores 5xx — bien por debajo del objetivo <500 ms)_

### Criterio de aceptación
- [x] Postman / Insomnia con todos los endpoints documentados y probados. _(OpenAPI yaml importable; probado con curl E2E)_
- [ ] Pruebas unitarias de endpoints críticos (>70% cobertura).
- [x] Webhook recibe eventos firmados de Vapi correctamente. _(probado en modo HMAC y modo dev)_
- [ ] Crear un pedido vía API y verlo en la app .NET MAUI con etiqueta `origen=voz`. _(el INSERT funciona; falta exponer el flag en la app)_
- [ ] El email automático actual de la app se dispara correctamente para pedidos `origen=voz`. _(pendiente verificar — el envío es interno de la app, no del PHP)_

---

## Fase 2 — Agente de voz MVP (semanas 2-3)

**Coste:** 1.600 €
**Objetivo:** primer agente capaz de coger una llamada, conversar y crear un pedido.

### Configuración del agente en Vapi

#### Modelo
- Provider: OpenAI
- Modelo: `gpt-4o-mini`
- Temperatura: 0.5
- Max tokens por respuesta: 250

#### Voz
- Provider: ElevenLabs
- Voz española (probar varias y elegir con el cliente).
- Stability: 0.6, Similarity: 0.75.

#### Transcriptor
- Provider: Deepgram
- Modelo: Nova-2
- Idioma: `es`
- Endpointing: 300ms.

### Prompt del agente — esqueleto

```text
Eres el asistente telefónico del Asador Morón. Tu función es atender
llamadas y tomar pedidos de los clientes.

REGLAS:
- Habla siempre en español natural y cercano, frases cortas.
- Confirma SIEMPRE el pedido completo antes de cerrarlo.
- Si el cliente pide algo no disponible, ofrece alternativa del menú actual.
- Si el cliente quiere hablar con una persona, transfiere sin discutir.
- Nunca inventes productos: solo ofrece lo que devuelva get_menu.
- Si el cliente da una dirección, valida que esté en zona de reparto.

FLUJO TÍPICO:
1. Saluda. Si reconoces al cliente por su número, salúdalo por su nombre.
2. Pregunta qué quiere pedir.
3. Toma el pedido producto a producto.
4. Pregunta si recogida o reparto.
5. Si reparto: confirma dirección.
6. Pregunta hora preferida o ofrece la estimada del sistema.
7. Lee el pedido completo y pide confirmación.
8. Crea el pedido y da el número y la hora.
9. Indícale que recibirá un email con el resumen.
10. Despídete amablemente.

HERRAMIENTAS DISPONIBLES:
- get_menu: lista de productos disponibles ahora.
- get_cliente_por_telefono: identifica al cliente.
- get_estado_local: si está abierto, saturado, etc.
- get_slots_recogida: huecos disponibles.
- validar_zona_reparto: comprueba si una dirección es válida.
- crear_pedido: crea el pedido en el sistema.
- transferir_a_humano: transfiere la llamada.
```

### Tareas

#### Configuración base
- [x] Crear assistant en Vapi con el prompt anterior. _(id `7a2ebbd1-e03d-4387-a842-dcdb5fcd0158`, prompt v5)_
- [x] Conectar las 7 herramientas (function calling) a los endpoints PHP. _(5 tools activas: `get_cliente`, `get_estado_local`, `get_slots_recogida`, `validar_zona`, `crear_pedido`, todas vía dispatcher `agent-tools.php` con HMAC)_
- [x] Configurar webhook de eventos hacia el backend. _(`/api/webhook-vapi.php` recibe events; tool-calls van a `/api/agent-tools.php`)_
- [x] Asociar número Twilio al assistant. _(`+14156495668` → assistant; voice_url Twilio gestionado por Vapi)_
- [x] Habilitar las 10 líneas simultáneas del plan pay-as-you-go. _(plan PAYG por defecto incluye 10)_

#### Pruebas conversacionales (mínimo 8 escenarios)
- [ ] Pedido simple de un producto.
- [ ] Pedido con varios productos y modificadores.
- [ ] Cliente reconocido por teléfono.
- [ ] Producto no disponible → alternativa.
- [ ] Cambio de opinión a mitad del pedido.
- [ ] Cliente pide hablar con persona.
- [ ] Cliente cuelga sin confirmar (no se debe crear pedido fantasma).
- [ ] Dos llamadas simultáneas (validar aislamiento de sesiones).

#### Confirmación al cliente
- [ ] Tras crear pedido, el email automático de la app se dispara correctamente.
- [ ] El agente menciona al cliente que recibirá el email.
- [ ] Validar que el email contiene: número de pedido, productos, hora estimada.

### Criterio de aceptación
- [ ] 20 llamadas de prueba consecutivas sin caídas.
- [ ] 80%+ de pedidos correctamente creados sin intervención.
- [ ] Latencia media de respuesta < 1.5s.
- [ ] El email automático llega dentro de los 10 segundos posteriores al pedido.
- [ ] Dos llamadas simultáneas se atienden sin mezclar contextos.

---

## Fase 3 — Reparto y asignación de repartidores (semanas 3-4)

**Coste:** 300 €
**Objetivo:** que el agente pueda gestionar pedidos de reparto y asignar repartidor automáticamente.

### Motor de reglas de asignación

Las reglas se evalúan en orden de prioridad. Cada regla filtra el conjunto de repartidores disponibles; la siguiente regla filtra sobre el subconjunto resultante. El primer repartidor del subconjunto final es el asignado.

#### Tipos de regla

```json
// Filtro por zona
{
  "tipo": "zona",
  "parametros": {
    "modo": "codigo_postal",
    "zonas_repartidores": {
      "41530": [12, 15],
      "41530B": [12, 18, 22]
    }
  }
}

// Filtro por carga máxima
{
  "tipo": "carga",
  "parametros": {
    "max_pedidos_simultaneos": 3
  }
}

// Filtro por tipo de vehículo
{
  "tipo": "vehiculo",
  "parametros": {
    "regla": [
      { "si_distancia_mayor_que_km": 5, "vehiculo": "coche" },
      { "si_pedido_mayor_que_eur": 50, "vehiculo": "coche" }
    ]
  }
}

// Filtro por turno activo
{
  "tipo": "turno",
  "parametros": {
    "solo_jornada_activa": true
  }
}

// Round-robin como desempate
{
  "tipo": "round_robin",
  "parametros": {}
}
```

### Cálculo de hora estimada

```
hora_estimada = ahora()
              + tiempo_preparacion_cocina(carga_actual, productos)
              + tiempo_reparto(distancia_a_cliente, trafico_estimado)
              + buffer_seguridad
```

Configurable desde `config_agente`:
- `tiempo_preparacion_base_minutos`: 20
- `tiempo_extra_por_pollo`: 5
- `velocidad_reparto_kmh`: 25
- `buffer_seguridad_minutos`: 5

### Tareas

#### Backend
- [ ] Servicio `MotorAsignacionRepartidores` con tests unitarios completos.
- [ ] Endpoint `GET /api/repartidores/asignar?pedido_id=X`.
- [ ] Servicio `CalculadorHoraEstimada` con tests.
- [ ] Endpoint `POST /api/reglas-asignacion` con validación.
- [ ] Endpoint `GET /api/zonas-reparto` con polígonos o códigos postales.
- [ ] Validación de dirección (geocoding o códigos postales).

#### Agente
- [ ] Añadir herramienta `validar_zona_reparto` al assistant.
- [ ] Añadir lógica al prompt para gestionar reparto.
- [ ] Tras crear pedido, llamar a `asignar_repartidor` y comunicar al cliente la hora.

### Criterio de aceptación
- [ ] Suite de tests cubre todos los tipos de regla.
- [ ] Caso de borde: ningún repartidor disponible → el agente lo gestiona honestamente.
- [ ] La hora estimada se ajusta dinámicamente con la carga real.

---

## Fase 4 — Dashboard y transcripciones en la app (semana 4)

**Coste:** 300 €
**Objetivo:** que el gestor pueda ver, configurar y revisar todo desde la app .NET MAUI.

### Vistas nuevas en la app

#### Vista "Llamadas"
- Listado paginado: fecha/hora, teléfono, duración, estado, pedido asociado.
- Filtros: rango de fechas, estado, búsqueda por teléfono.
- Detalle: transcripción completa, audio (si existe), pedido vinculado.

#### Vista "Dashboard"
- Tarjetas con KPIs del periodo seleccionado:
  - Llamadas totales recibidas.
  - Convertidas en pedidos (con %).
  - Transferidas a humano (con %).
  - No relacionadas con pedidos.
  - Fallidas.
  - Coste total estimado.
- Gráfico de llamadas por hora (heatmap día x hora).
- Top productos pedidos por voz vs por app.
- Tiempo medio de llamada.
- **Pico de llamadas concurrentes** (para dimensionar líneas).

#### Vista "Configuración del agente"
- Editor visual de reglas de asignación de repartidores.
- Configuración de horarios y modo saturación.
- Gestión de blacklist.
- Parámetros del cálculo de hora estimada.
- Personalización del saludo y tono del agente.

### Comunicación en tiempo real

Cuando entra una llamada o se completa un pedido por voz, la app debe enterarse sin sondear:

- **Opción A — SignalR**: si la app ya usa autenticación con el backend.
- **Opción B — Push notifications (FCM)**: si ya está configurado en la app.
- **Opción C — Long polling / SSE**: fallback.

### Tareas

#### Backend
- [x] Endpoint `GET /api/dashboard/metricas` con caché Redis. _([dashboard-metricas.php](../Backend/API/api/dashboard-metricas.php) con `agente_cache` Redis 5 min)_
- [ ] Job programado que precalcula métricas cada 5 minutos. _(cron a añadir cuando el volumen lo justifique; la cache lazy ya da p95 OK)_
- [ ] Hub SignalR (o equivalente) emitiendo eventos: `llamada-iniciada`, `pedido-creado`, `llamada-finalizada`. _(pendiente — alternativa: usar OneSignal que ya está integrado)_

#### App MAUI
- [x] Vista "Llamadas" con listado y filtros. _([LlamadasViewAdmin.xaml](../Views/Administrador/LlamadasViewAdmin.xaml) + [VM](../ViewModels/Administrador/LlamadasViewModelAdmin.cs): filtros por fechas, estado, teléfono; paginación)_
- [x] Vista detalle de llamada con transcripción. _([DetalleLlamadaViewAdmin.xaml](../Views/Administrador/DetalleLlamadaViewAdmin.xaml))_
- [ ] Reproductor de audio integrado. _(de momento abre la URL del audio en navegador; falta `MediaElement` inline)_
- [x] Vista "Dashboard" con KPIs y gráficos. _([DashboardAgenteView.xaml](../Views/Administrador/DashboardAgenteView.xaml): KPIs tarjetas; gráficos heatmap pendientes)_
- [ ] Suscripción al hub SignalR. _(pendiente)_
- [ ] Notificación visual cuando entra un pedido por voz. _(pendiente — se puede hacer con OneSignal en cuanto haya hub)_

### Criterio de aceptación
- [ ] Dashboard se carga en < 2 segundos. _(falta medir en dispositivo real)_
- [ ] Notificación de nueva llamada llega a la app en < 3 segundos. _(pendiente — hub no implementado)_
- [ ] Audio reproducible directamente desde la app. _(parcial — abre en navegador, no inline)_
- [x] KPI de "Pico de llamadas concurrentes" visible en el dashboard. _(card "Pico llamadas simultáneas" en `DashboardAgenteView`)_

---

## Fase 5 — Robustez y casos límite (semana 5)

**Coste:** 700 €
**Objetivo:** preparar el sistema para escenarios reales.

### Modos especiales

#### Modo fuera de horario
- Si `hora actual ∉ horario` → el agente informa y opcionalmente acepta una reserva para mañana.
- Configurable desde la app.

#### Modo saturación
- Si la cocina está al límite (carga > umbral configurable) → el agente extiende tiempos o rechaza nuevos pedidos.
- Estado mostrado claramente en la app del personal.

### Anti-abuso

- [ ] Rate limit por número: máx. 3 pedidos/hora desde el mismo número.
- [ ] Importe máximo automático: pedidos > 100€ requieren confirmación humana (transferencia).
- [ ] Detección de patrones de trolleo: añadir automáticamente a blacklist tras N llamadas no relacionadas con pedidos.
- [ ] Endpoint para que el gestor desbloquee números desde la app.

### Resiliencia

- [ ] Health check de Vapi y Twilio cada minuto.
- [ ] Si Vapi cae → Twilio redirige a buzón de respaldo con mensaje grabado.
- [ ] Si la API PHP cae → el agente informa al cliente de problema técnico.
- [ ] Alertas (email/Slack) si:
  - Tasa de error > 10% en una hora.
  - Coste supera presupuesto diario configurado.
  - Más de 5 llamadas fallidas seguidas.
  - **Se alcanza el límite de líneas simultáneas** (señal para ampliar plan).

### Casos límite conversacionales

Probar y refinar con grabaciones reales:

- [ ] Cliente con voz infantil o muy mayor.
- [ ] Mucho ruido de fondo (cocina, tráfico, niños).
- [ ] Cliente que no termina las frases.
- [ ] Cliente que dicta el pedido todo seguido sin pausas.
- [ ] Cliente que cambia de opinión muchas veces.
- [ ] Acentos regionales fuertes (andaluz, gallego, catalán hablando español).
- [ ] Cliente extranjero con español limitado.
- [ ] Pedido con muchas modificaciones simultáneas.

### Cumplimiento legal (RGPD)

- [ ] Mensaje al inicio: "esta llamada será grabada para mejorar el servicio".
- [ ] Política de retención: audios borrados a los 90 días, transcripciones a los 365.
- [ ] Endpoint para que un cliente solicite borrado de sus datos.
- [ ] Documento de privacidad actualizado y aprobado por el cliente.

### Criterio de aceptación
- [ ] Pasar checklist de los 8 casos límite con tasa de éxito > 80%.
- [ ] Simulacro de caída de Vapi: el cliente final no se queda sin servicio.
- [ ] Auditoría RGPD completada.

---

## Fase 6 — Piloto y puesta en producción (semanas 5-6)

**Coste:** 200 €
**Objetivo:** despliegue progresivo con seguridad.

### Fase piloto

- [ ] Selección de 20-30 clientes habituales para piloto (con consentimiento).
- [ ] Su número se enruta al agente; el resto al sistema actual.
- [ ] Llamadas internas del personal para validar tono.
- [ ] Recopilar feedback durante 1 semana.
- [ ] Iterar prompt y reglas según resultados reales.

### Despliegue gradual

- [ ] Día 1-3: 10% del tráfico al agente (muestreo aleatorio).
- [ ] Día 4-7: 50% si KPIs > umbral.
- [ ] Día 8 en adelante: 100% si todo va bien.
- [ ] Plan de rollback documentado: revertir a sistema anterior en < 5 min.

### Monitoreo continuo

Métricas a vigilar diariamente la primera semana, luego semanalmente:

| Métrica | Umbral aceptable |
|---|---|
| Tasa de finalización sin transferencia | > 70% |
| Pedidos correctos validados | > 90% |
| Duración media de llamada | < 120s |
| Latencia de respuesta | < 1.5s |
| Coste medio por llamada | < 0.40 € |
| Pico de llamadas concurrentes | < 10 (umbral de ampliación) |

### Comunicación al cliente final

- [ ] Cartel en el local explicando el cambio.
- [ ] Mensaje en redes sociales del asador.
- [ ] Mensaje en el saludo del agente: "soy el asistente del Asador Morón, te ayudo con tu pedido".

### Criterio de aceptación final
- [ ] 1 semana en producción al 100% sin incidencias críticas.
- [ ] Cliente conforme con el funcionamiento.
- [ ] Documentación de operación entregada.
- [ ] Procedimiento de soporte y escalado definido.

---

## Fase 7 — Otras configuraciones de la app (semana 6)

**Coste:** 200 €
**Objetivo:** afinar la app con configuraciones y herramientas adicionales que el gestor necesite tras ver el sistema funcionando.

### Funcionalidades

#### Editor visual de reglas de asignación
- [ ] Interfaz drag & drop para reordenar prioridades de reglas.
- [ ] Formulario por tipo de regla con campos validados.
- [ ] Vista previa: "con esta configuración, este pedido iría a este repartidor".

#### Configurador de horarios y modos
- [ ] Horarios de apertura por día de la semana.
- [ ] Días festivos y horarios especiales.
- [ ] Umbrales del modo saturación (pedidos en cola, tiempos máximos).
- [ ] Activación/desactivación rápida del agente.

#### Parámetros del cálculo de hora estimada
- [ ] Tiempo base de preparación.
- [ ] Tiempo extra por producto (por categoría).
- [ ] Velocidad media de reparto.
- [ ] Buffer de seguridad.

#### Gestión de blacklist
- [ ] Listado de números bloqueados con motivo.
- [ ] Añadir/quitar números desde la app.
- [ ] Historial de bloqueos automáticos.

#### Personalización del agente
- [ ] Editor del saludo personalizado.
- [ ] Selección de voz (entre opciones probadas).
- [ ] Ajustes de tono (más formal / más cercano).

#### Buffer para ajustes operativos
Esta fase incluye explícitamente un margen para implementar pequeños ajustes que surjan durante el desarrollo y que no encajen en otras fases (sin coste adicional, dentro del precio de la fase).

### Criterio de aceptación
- [ ] El gestor puede modificar todas las reglas y parámetros sin tocar código.
- [ ] Los cambios se aplican en tiempo real (sin redeploy del agente).
- [ ] Todas las pantallas testeadas con el cliente real.

---

## Anexos

### A. Variables de entorno

```env
# OpenAI
OPENAI_API_KEY=...

# Vapi
VAPI_API_KEY=...
VAPI_WEBHOOK_SECRET=...
VAPI_ASSISTANT_ID=...

# Twilio
TWILIO_ACCOUNT_SID=...
TWILIO_AUTH_TOKEN=...
TWILIO_PHONE_NUMBER=+34...

# ElevenLabs
ELEVENLABS_API_KEY=...
ELEVENLABS_VOICE_ID=...

# Deepgram
DEEPGRAM_API_KEY=...

# Backend
APP_URL=https://api.asadormoron.com
WEBHOOK_BASE_URL=https://api.asadormoron.com/api/webhook
REDIS_URL=redis://...

# Cumplimiento RGPD
RETENTION_AUDIO_DIAS=90
RETENTION_TRANSCRIPCION_DIAS=365

# Costes y límites
PRESUPUESTO_DIARIO_EUR=50
MAX_LLAMADAS_HORA_POR_NUMERO=3
MAX_IMPORTE_SIN_CONFIRMACION_HUMANA=100

# Concurrencia
PHP_FPM_WORKERS=30
DB_POOL_MIN=20
DB_POOL_MAX=50
```

### B. Flujo de eventos del webhook

| Evento Vapi | Acción en backend |
|---|---|
| `call-start` | Insertar registro en `llamadas` con estado `en_curso` |
| `function-call` | Ejecutar función correspondiente y devolver resultado |
| `transcript-final` | Guardar turno en `transcripciones` |
| `call-end` (status=completed) | Cerrar llamada, calcular coste, vincular pedido |
| `call-end` (status=failed) | Marcar como `fallida`, registrar motivo |
| `call-transfer` | Marcar como `transferida`, registrar destino |

### C. Estimación de coste por volumen

| Llamadas/mes | Coste mensual estimado |
|---|---|
| 900 (30/día) | 270 - 360 € |
| 3.000 (100/día) | 900 - 1.200 € |
| 6.000 (200/día) | 1.800 - 2.400 € |

Coste por llamada típica: **~0,35 €**.

### D. Riesgos y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
|---|---|---|---|
| API PHP inestable bajo carga | Media | Alto | Pruebas de carga en fase 1, caché agresiva |
| Cliente final rechaza voz IA | Baja | Alto | Voz premium, transferencia fácil, comunicación previa |
| Costes mayores de lo previsto | Media | Medio | Alertas de presupuesto, monitoreo diario |
| Vapi cambia precios | Baja | Medio | Capa middleware permite cambiar de proveedor |
| Caídas de proveedores | Baja | Alto | Buzón de respaldo, alertas, plan de continuidad |
| Saturación de líneas en pico | Media | Medio | Monitoreo de pico concurrente, ampliación con 10€/mes por línea |
| Fugas de datos personales | Baja | Crítico | Cifrado en tránsito y reposo, retención corta, auditoría RGPD |

### E. Calendario por semana

```
Semana 1: Fase 0 (Descubrimiento) ── Fase 1 inicio
Semana 2: Fase 1 cierre ── Fase 2 inicio (MVP)
Semana 3: Fase 2 cierre (MVP listo) ── Fase 3 inicio
Semana 4: Fase 3 cierre ── Fase 4 (Dashboard)
Semana 5: Fase 5 (Robustez) ── Fase 6 inicio (Piloto)
Semana 6: Fase 6 cierre (Producción) ── Fase 7 (Configuraciones)
```

### F. Forma de pago

| Hito | Importe (sin IVA) | Cuándo |
|---|---|---|
| Anticipo de inicio | 1.900 € | A la firma del presupuesto |
| Entrega final | 1.900 € | A la entrega del sistema completo |
| **Total** | **3.800 €** | |

### G. Próximos pasos inmediatos

1. Firma del presupuesto por parte del Asador Morón.
2. Cobro del anticipo (1.900 €).
3. Arranque de la fase 0: documentación de la API y entrevistas.
4. Creación de cuentas en proveedores.
5. Reserva del número Twilio.

---

*Documento vivo. Actualizar tras cada fase con lecciones aprendidas y ajustes al plan.*
