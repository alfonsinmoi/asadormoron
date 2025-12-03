using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;
using System;

namespace AsadorMoron.Models
{
    public class ConfiguracionAdmin
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoDomingo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoJueves { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoLunes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMartes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool visibleRS { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMiercoles { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoSabado { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoViernes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoDomingoTarde { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoJuevesTarde { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoLunesTarde { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMartesTarde { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMiercolesTarde { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoSabadoTarde { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoViernesTarde { get; set; }

        public TimeSpan finDomingo { get; set; }
        public TimeSpan finJueves { get; set; }
        public TimeSpan finLunes { get; set; }
        public TimeSpan finMartes { get; set; }
        public TimeSpan finMiercoles { get; set; }
        public TimeSpan finSabado { get; set; }
        public TimeSpan finViernes { get; set; }

        public TimeSpan finDomingoTarde { get; set; }
        public TimeSpan finJuevesTarde { get; set; }
        public TimeSpan finLunesTarde { get; set; }
        public TimeSpan finMartesTarde { get; set; }
        public TimeSpan finMiercolesTarde { get; set; }
        public TimeSpan finSabadoTarde { get; set; }
        public TimeSpan finViernesTarde { get; set; }

        public TimeSpan inicioDomingoTarde { get; set; }
        public TimeSpan inicioJuevesTarde { get; set; }
        public TimeSpan inicioLunesTarde { get; set; }
        public TimeSpan inicioMartesTarde { get; set; }
        public TimeSpan inicioMiercolesTarde { get; set; }
        public TimeSpan inicioSabadoTarde { get; set; }
        public TimeSpan inicioViernesTarde { get; set; }

        public TimeSpan inicioDomingo { get; set; }
        public TimeSpan inicioJueves { get; set; }
        public TimeSpan inicioLunes { get; set; }
        public TimeSpan inicioMartes { get; set; }
        public TimeSpan inicioMiercoles { get; set; }
        public TimeSpan inicioSabado { get; set; }
        public TimeSpan inicioViernes { get; set; }

        public int extraLunes { get; set; }
        public int extraMartes { get; set; }
        public int extraMiercoles { get; set; }
        public int extraJueves { get; set; }
        public int extraViernes { get; set; }
        public int extraSabado { get; set; }
        public int extraDomingo { get; set; }

        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool servicioActivo { get; set; }
        public double gastosEnvio { get; set; }
        public int beneficios { get; set; }

        public int versionMinimaAndroid { get; set; }
        public int versionMinimaIOS { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }

        public TimeSpan inicioComercio { get; set; }
        public TimeSpan finComercio { get; set; }
        public TimeSpan inicioComercioTarde { get; set; }
        public TimeSpan finComercioTarde { get; set; }
        public double pedidoMinimo { get; set; }
        public double pedidoMinimoComercio { get; set; }

        public double distanciaMapas { get; set; }

        public string telefono { get; set; }
        public string whatsapp { get; set; }
        public string email { get; set; }
        public int idPueblo { get; set; }

        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool efectivo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool tarjeta { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool bizum { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool datafono { get; set; }

        public string nombreTicket { get; set; }
        public string cifTicket { get; set; }
        public string direccionTicket { get; set; }
        public string telefonoTicket { get; set; }
        public string nombreImpresora { get; set; }
        public string iban { get; set; }
        public int idGrupo { get; set; }
        public int ticketSize { get; set; }
        public double comision { get; set; }
        public double variableDatafono { get; set; }
        public double variableTarjeta { get; set; }
        public double fijoTarjeta { get; set; }
        public int tiempoEntreMenus { get; set; }
        public int categoriaPizzas { get; set; }
    }
}
