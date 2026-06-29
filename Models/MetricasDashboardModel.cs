using System.Collections.Generic;
using Newtonsoft.Json;

namespace AsadorMoron.Models
{
    /// <summary>
    /// KPIs agregados del agente para el dashboard.
    /// Corresponde a /api/dashboard-metricas.php
    /// </summary>
    public class MetricasDashboardModel
    {
        public string desde { get; set; }
        public string hasta { get; set; }
        public int? idEstablecimiento { get; set; }
        public int llamadas_totales { get; set; }
        public int convertidas_en_pedido { get; set; }
        public double tasa_conversion_pct { get; set; }

        [JsonProperty("por_estado")]
        public PorEstadoModel por_estado { get; set; } = new PorEstadoModel();

        public double coste_total_eur { get; set; }
        public int duracion_media_seg { get; set; }
        public int pico_concurrencia { get; set; }

        public List<HeatmapPuntoModel> heatmap { get; set; } = new List<HeatmapPuntoModel>();
        public string generado { get; set; }

        public string DuracionMediaFormateada
        {
            get
            {
                var m = duracion_media_seg / 60;
                var s = duracion_media_seg % 60;
                return $"{m}m {s}s";
            }
        }

        public string CosteFormateado => $"{coste_total_eur:0.00} €";

        /// <summary>
        /// Devuelve una lista plana de 7 × 24 = 168 celdas para renderizar el heatmap.
        /// El orden es por filas (día) y dentro de cada fila por hora (0..23).
        /// El día 1 es lunes (reordenado: SQL DAYOFWEEK devuelve 1=domingo, lo reubicamos
        /// para que aparezca al final de la semana).
        /// </summary>
        public List<HeatmapCeldaModel> CalcularCeldasHeatmap()
        {
            var celdas = new List<HeatmapCeldaModel>(168);
            var max = 0;
            foreach (var p in heatmap) if (p.n > max) max = p.n;
            if (max <= 0) max = 1;

            // Orden de filas: lunes, martes, ..., domingo (más natural)
            // DAYOFWEEK SQL: 1=domingo, 2=lunes, ..., 7=sábado
            int[] ordenDias = { 2, 3, 4, 5, 6, 7, 1 };
            string[] nombres = { "L", "M", "X", "J", "V", "S", "D" };

            for (int fila = 0; fila < 7; fila++)
            {
                int diaSql = ordenDias[fila];
                for (int h = 0; h < 24; h++)
                {
                    int n = 0;
                    foreach (var p in heatmap)
                    {
                        if (p.dia == diaSql && p.hora == h) { n = p.n; break; }
                    }
                    celdas.Add(new HeatmapCeldaModel
                    {
                        fila = fila,
                        columna = h,
                        valor = n,
                        intensidad = (double)n / max,
                        etiquetaDia = nombres[fila],
                        mostrarEtiquetaDia = (h == 0)
                    });
                }
            }
            return celdas;
        }
    }

    public class PorEstadoModel
    {
        public int en_curso { get; set; }
        public int completada { get; set; }
        public int transferida { get; set; }
        public int no_pedido { get; set; }
        public int fallida { get; set; }
    }

    public class HeatmapPuntoModel
    {
        public int dia { get; set; }   // 1=domingo, 7=sábado
        public int hora { get; set; }  // 0-23
        public int n { get; set; }
    }

    /// <summary>
    /// Celda visual del heatmap: posición en grid + intensidad para colorear.
    /// </summary>
    public class HeatmapCeldaModel
    {
        public int fila { get; set; }
        public int columna { get; set; }
        public int valor { get; set; }
        public double intensidad { get; set; }  // 0..1
        public string etiquetaDia { get; set; }
        public bool mostrarEtiquetaDia { get; set; }
        public bool TieneValor => valor > 0;
        public Microsoft.Maui.Graphics.Color ColorCelda
        {
            get
            {
                // Rojo de marca con alpha proporcional a intensidad (mínimo 0.06 para que se note el grid)
                var alpha = (float)(0.06 + intensidad * 0.94);
                return Microsoft.Maui.Graphics.Color.FromRgba(196f / 255f, 30f / 255f, 58f / 255f, alpha);
            }
        }
    }
}

