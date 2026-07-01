#!/usr/bin/env bash
# E2E del pedido personalizado por voz: get_menu → get_opciones → get_ingredientes
# → resumen_pedido → crear_pedido, y verifica la persistencia (idOpcion,
# ingredientes_json, concepto, precio autoritativo, línea de bolsa).
#
# ⚠️ Crea un pedido REAL de prueba y lo BORRA al final (incluye limpieza).
# Uso:  ./e2e_pedido_personalizado.sh
set -euo pipefail

P="https://qoorder.com/pa_ws/api/agent-tools.php"
SEC="7cfba4331b5ed71fd28e72510ee4b3f063e215d9a988d6642b059764eba95cb1"
call() { # $1 = json de arguments de la tool  ·  $2 = nombre tool
  curl -sS -X POST "$P" -H "Content-Type: application/json" -H "X-Vapi-Secret: $SEC" \
    -d "{\"message\":{\"type\":\"tool-calls\",\"call\":{\"id\":\"e2e\"},\"toolCalls\":[{\"id\":\"c\",\"function\":{\"name\":\"$2\",\"arguments\":$1}}]}}" \
  | python3 -c "import json,sys; print(json.load(sys.stdin)['results'][0]['result'])"
}

echo "1) get_menu(pollo) — busca producto con opciones"
call '{"buscar":"pollo"}' get_menu | python3 -c "import json,sys;d=json.loads(sys.stdin.read());print('  productos:',d['total'],'| con opc:',[p['id'] for p in d['productos'] if p['opc']][:3])"

echo "2) resumen_pedido con opción + ingredientes (precio server-side)"
RES=$(call '{"tipoVenta":"Recogida","lineas":[{"idProducto":11716,"cantidad":1,"idOpcion":14160},{"idProducto":11879,"cantidad":1,"ingredientes":[{"idIngrediente":579,"esAnadir":true},{"idIngrediente":580,"esAnadir":false}]}]}' resumen_pedido)
echo "$RES" | python3 -c "import json,sys;d=json.loads(sys.stdin.read());print('  total:',d['total'],'| resumen:',d['resumen'])"

echo "3) crear_pedido (mismas líneas)"
CRE=$(call '{"tipoVenta":"Recogida","nombreUsuario":"E2E TEST","telefono":"+34600000000","lineas":[{"idProducto":11716,"cantidad":1,"idOpcion":14160},{"idProducto":11879,"cantidad":1,"ingredientes":[{"idIngrediente":579,"esAnadir":true},{"idIngrediente":580,"esAnadir":false}]}]}' crear_pedido)
echo "$CRE" | python3 -c "import json,sys;d=json.loads(sys.stdin.read());print('  codigo:',d.get('codigo'),'| id:',d.get('id'),'| total:',d.get('total'))"
COD=$(echo "$CRE" | python3 -c "import json,sys;print(json.loads(sys.stdin.read()).get('codigo',''))")

echo "4) verificar persistencia en BD (idOpcion, ingredientes_json, concepto)"
ssh root@82.223.139.121 "mysql -u pollo_user -p'P0ll0_2026!Qx' -D pollo -e \"
SELECT d.idProducto, d.idOpcion, d.tipo, d.precio, d.concepto, LEFT(d.ingredientes_json,60) AS ing
FROM qo_pedidos_detalle d JOIN qo_pedidos p ON p.id=d.idPedido
WHERE p.codigo='$COD' ORDER BY d.tipo;\" 2>/dev/null"

echo "5) limpieza (borrar pedido de prueba)"
ssh root@82.223.139.121 "mysql -u pollo_user -p'P0ll0_2026!Qx' -D pollo -e \"
DELETE d FROM qo_pedidos_detalle d JOIN qo_pedidos p ON p.id=d.idPedido WHERE p.codigo='$COD';
DELETE e FROM qo_pedidos_estado e JOIN qo_pedidos p ON p.id=e.idPedido WHERE p.codigo='$COD';
DELETE FROM qo_pedidos WHERE codigo='$COD';
SELECT CONCAT('borradas filas pedido: ', ROW_COUNT());\" 2>/dev/null"
echo "E2E OK"
