using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;

namespace AsadorMoron.Models
{
    public class MenuDiarioConfiguracionModel
    {
        public int id { get; set; }
        public int idEstablecimiento { get; set; }
        public int tiempoMaximo { get; set; }
        public int maxPedidos { get; set; }
        public TimeSpan horaInicioLunes { get; set; }
        public TimeSpan horaFinLunes { get; set; }
        public TimeSpan horaInicioMartes { get; set; }
        public TimeSpan horaFinMartes { get; set; }
        public TimeSpan horaInicioMiercoles { get; set; }
        public TimeSpan horaFinMiercoles { get; set; }
        public TimeSpan horaInicioJueves { get; set; }
        public TimeSpan horaFinJueves { get; set; }
        public TimeSpan horaInicioViernes { get; set; }
        public TimeSpan horaFinViernes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoLunes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMartes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMiercoles { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoJueves { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoViernes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool cartaYMenu { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool postreObligatorio { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool bebidaObligatoria { get; set; }
        public double extraPostre { get; set; }
        public double extraBebida { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool platoUnico { get; set; }
        public double precioPlatoUnico { get; set; }

    }
}
