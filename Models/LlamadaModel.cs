using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AsadorMoron.Models
{
    /// <summary>
    /// Llamada del agente de voz (corresponde a qo_llamadas).
    /// </summary>
    public class LlamadaModel
    {
        public int id { get; set; }
        public string vapi_call_id { get; set; }
        public string telefono_origen { get; set; }
        public int? cliente_id { get; set; }
        public int? idEstablecimiento { get; set; }
        public string estado { get; set; }
        public int? pedido_id { get; set; }
        public string pedido_codigo { get; set; }
        public int duracion_segundos { get; set; }
        public decimal coste_estimado { get; set; }
        public string audio_url { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }

        [JsonProperty("transcripcion")]
        public List<TranscripcionTurnoModel> transcripcion { get; set; }

        public string DuracionFormateada
        {
            get
            {
                if (duracion_segundos <= 0) return "—";
                var m = duracion_segundos / 60;
                var s = duracion_segundos % 60;
                return $"{m}:{s:D2}";
            }
        }

        public string CosteFormateado => $"{coste_estimado:0.00} €";

        public string FechaCorta
        {
            get
            {
                if (DateTime.TryParse(fecha_inicio, out var dt))
                    return dt.ToString("dd/MM HH:mm");
                return fecha_inicio ?? "";
            }
        }

        public string EstadoColor
        {
            get => estado switch
            {
                "completada"  => "#4CAF50",
                "transferida" => "#2196F3",
                "fallida"     => "#F44336",
                "no_pedido"   => "#FF9800",
                "en_curso"    => "#9E9E9E",
                _             => "#9E9E9E"
            };
        }

        public string EstadoTexto
        {
            get => estado switch
            {
                "completada"  => "Completada",
                "transferida" => "Transferida",
                "fallida"     => "Fallida",
                "no_pedido"   => "Sin pedido",
                "en_curso"    => "En curso",
                _             => estado ?? "—"
            };
        }

        public bool TienePedido => pedido_id.HasValue && pedido_id.Value > 0;
        public bool TieneAudio  => !string.IsNullOrEmpty(audio_url);
    }

    public class TranscripcionTurnoModel
    {
        public int id { get; set; }
        public string texto { get; set; }
        public string fecha { get; set; }

        [JsonProperty("turnos")]
        public List<TurnoStructModel> turnos { get; set; }

        public string FechaHora
        {
            get
            {
                if (DateTime.TryParse(fecha, out var dt))
                    return dt.ToString("HH:mm:ss");
                return "";
            }
        }
    }

    public class TurnoStructModel
    {
        public string rol { get; set; }
        public string texto { get; set; }
        public string timestamp { get; set; }
    }

    public class LlamadasPaginadasModel
    {
        public List<LlamadaModel> items { get; set; } = new List<LlamadaModel>();
        public int total { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public int pages { get; set; }
    }
}
