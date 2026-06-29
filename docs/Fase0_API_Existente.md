# Fase 0 — Mapa de la API existente

> Inventario de endpoints actuales del backend `qoorder.com/pa_ws/` necesarios para el agente de voz.
> Última actualización: 2026-05-11

## Convenciones del backend

- **URL base:** `https://qoorder.com/pa_ws/`
- **Routing:** `archivo.php/GET` + query string (PATH_INFO en nginx)
- **Métodos:** GET (lectura), POST (crear), PUT (modificar), DELETE (borrar)
- **Auth:** sin token JWT, depende de `idUsuario` en parámetros (no apto para terceros)
- **Respuestas:** JSON sin envelope; campo `id` o `false` según éxito
- **DB:** MariaDB local en `82.223.139.121`, usuario `pollo_user`, base `pollo`

---

## Endpoints que necesita el agente de voz

### 1. `get_menu` — productos disponibles

**Endpoint actual:** `GET /productos.php/GET?idEstablecimientoProducto={id}`

Devuelve los productos con sus opciones, ingredientes y alérgenos por establecimiento.

**Cobertura:** ✅ existe, pero hay que filtrar por `estado=1` y `disponible_ahora` (no hay este último — habría que añadirlo si se quiere stock dinámico).

**Cambios sugeridos:**
- Añadir columna `qo_productos.disponible` (boolean) que el establecimiento pueda togglar
- O filtrar por `qo_productos_cat.estado=1` (ya existe)

---

### 2. `get_cliente_por_telefono` — identificar cliente

**Endpoint actual:** ❌ **NO EXISTE**

Lo más parecido es `usuarios.php/GET?email=...`. No hay búsqueda por teléfono.

**Acción:** crear nuevo endpoint
```
GET /usuarios.php/GET?telefono={phone}&idPueblo={id}
→ {idUsuario, nombre, apellidos, direccion, ultimoPedido}
```

---

### 3. `get_estado_local` — abierto/cerrado/saturado

**Endpoints actuales relacionados:**
- `GET /configuracion.php/GET?idEstablecimiento={id}` — devuelve config completa
- `GET /establecimientos.php/GET?idEstablecimientoFranja={id}` — franjas horarias
- `GET /contador_pollos.php?idEstablecimiento={id}` — pollos en horno ahora

**Cobertura:** ⚠️ parcial — hay que componer la respuesta desde tres llamadas.

**Acción:** crear endpoint agregador
```
GET /api/estado-local?idEstablecimiento={id}
→ {abierto: bool, motivo: string, saturado: bool, cargaCocina: int, proxima_apertura: datetime}
```

---

### 4. `get_slots_recogida` — huecos disponibles

**Endpoint actual:** ❌ **NO EXISTE**

`contador_pollos.php` da la carga actual pero no calcula slots futuros.

**Acción:** crear endpoint
```
GET /api/slots-recogida?idEstablecimiento={id}&desde={hora}&hasta={hora}
→ [{hora: "20:30", capacidad: 4, libre: 2}, ...]
```

Lógica: combinar `qo_pedidos` con hora estimada + `tiempo_preparacion_base_minutos` config.

---

### 5. `validar_zona_reparto` — comprueba dirección

**Endpoint actual:** `GET /zonas.php/GET?idPueblo={id}`

Devuelve las zonas con `gastos`, `pedidoMinimo`, `color`.

**Cobertura:** ⚠️ devuelve lista, no valida una dirección concreta.

**Acción:** crear validador
```
GET /api/validar-zona?direccion={texto}&idPueblo={id}
→ {valida: bool, idZona: int, nombre: string, gastos: float, pedidoMinimo: float}
```

Lógica: geocoding (Google Maps API o Nominatim) → comparar con polígonos de zonas o códigos postales.

---

### 6. `crear_pedido` — graba el pedido

**Endpoint actual:** `POST /pedidos.php` con body completo del pedido.

**Cobertura:** ✅ existe y funciona. Necesita ampliarse:

**Cambios requeridos** (ALTER TABLE):
```sql
ALTER TABLE qo_pedidos
  ADD COLUMN origen ENUM('app','voz','manual','web') DEFAULT 'manual',
  ADD COLUMN llamada_id BIGINT NULL,
  ADD COLUMN hora_estimada DATETIME NULL,
  ADD COLUMN repartidor_id BIGINT NULL,
  ADD INDEX idx_origen (origen);
```

Y aceptar los nuevos campos en `pedidos.php` POST.

---

### 7. `transferir_a_humano` — no requiere API

Lo gestiona Vapi/Twilio directamente con `<Dial>` al número del establecimiento (campo `qo_establecimientos.telefono`).

---

## Endpoints reutilizables (no necesitan cambios)

| Función voz | Endpoint actual | Estado |
|---|---|---|
| Listar repartidores activos | `repartidores.php/GET?idEstablecimientoRepartidor={id}` | ✅ |
| Posición de repartidor (para hora estimada) | `repartidores.php/GET?idPosicionRepartidor={id}` | ✅ |
| Email automático tras pedido | (lo dispara `pedidos.php` POST internamente) | ✅ |
| Push a la app del personal | `sendNotification.php` y `sendNotificationVarios.php` | ✅ (recién arreglados) |

---

## Endpoints completamente nuevos (Fase 1)

Estos no existen y hay que crearlos desde cero:

```
POST  /api/webhook/vapi              — receptor de eventos de Vapi (HMAC)
GET   /api/llamadas                  — listado de llamadas para dashboard
GET   /api/llamadas/{id}             — detalle + transcripción
GET   /api/dashboard/metricas        — KPIs agregados (cache Redis 5 min)
GET   /api/repartidores/asignar      — motor de reglas → repartidor óptimo
GET/POST/DELETE /api/blacklist       — números bloqueados
GET/POST  /api/reglas-asignacion     — gestor de reglas
GET   /api/config-agente             — parámetros del agente
PUT   /api/config-agente             — actualización
```

---

## Tablas nuevas necesarias (Fase 1)

Ya detalladas en el plan: `llamadas`, `transcripciones`, `reglas_asignacion`, `blacklist_telefonos`, `config_agente`.

---

## Bugs detectados durante el inventario (no críticos)

Cosas a anotar para una limpieza futura, sin bloquear el agente de voz:

- `zonas.php` DELETE elimina de `qo_users` (parece copia-pega)
- `valoraciones.php` DELETE elimina de `qo_establecimientos` (idem)
- `tokens.php` POST inserta en `qo_users` cuando debería insertar token
- `puntos.php` DELETE elimina de `qo_users` (idem)
- Varios endpoints sin validación de `idUsuario`/`token` — cualquiera con el ID puede leer datos ajenos

---

## Próximos pasos (Fase 0 → Fase 1)

1. **Validar inventario con el cliente:** confirmar horarios, productos disponibles, repartidores activos.
2. **Crear cuentas en proveedores:** Vapi.ai, Twilio (número español), OpenAI, ElevenLabs.
3. **Branch `feature/voice-agent`** en el repo + entorno de staging del backend.
4. **Empezar Fase 1:** migraciones SQL + endpoints nuevos.
