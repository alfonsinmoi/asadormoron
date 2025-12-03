using System;
using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;

namespace AsadorMoron.Models
{
    public class Establecimiento
    {
        [PrimaryKey, AutoIncrement]
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("idEstablecimiento")]
        public int idEstablecimiento { get; set; }
        [JsonProperty("nombre")]
        public string nombre { get; set; }
        [JsonProperty("direccion")]
        public string direccion { get; set; }
        [JsonProperty("poblacion")]
        public string poblacion { get; set; }
        [JsonProperty("provincia")]
        public string provincia { get; set; }
        [JsonProperty("codPostal")]
        public string codPostal { get; set; }
        [JsonProperty("latitud")]
        public double latitud { get; set; }
        [JsonProperty("longitud")]
        public double longitud { get; set; }
        [JsonProperty("tipo")]
        public string tipo { get; set; }
        [JsonProperty("valoraciones")]
        public int valoraciones { get; set; }
        [JsonProperty("idTipo")]
        public int idTipo { get; set; }
        [JsonProperty("imagen")]
        public string imagen { get; set; }
        [JsonProperty("telefono")]
        public string telefono { get; set; }
        public string telefono2 { get; set; }
        public string whatsapp { get; set; }
        public string emailContacto { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("estado")]
        public int estado { get; set; }
        [JsonProperty("numeroCategorias")]
        public int numeroCategorias { get; set; }
        [JsonProperty("numeroProductos")]
        public int numeroProductos { get; set; }
        [JsonProperty("zonas")]
        public int zonas { get; set; }
        [JsonProperty("ventas")]
        public double ventas { get; set; }
        [JsonProperty("local")]
        public int local { get; set; }
        [JsonProperty("envio")]
        public int envio { get; set; }
        [JsonProperty("recogida")]
        public int recogida { get; set; }
        [JsonProperty("logo")]
        public string logo { get; set; }
        [JsonProperty("idZona")]
        public int idZona { get; set; }
        [JsonProperty("distancia")]
        public double distancia { get; set; }
        public double valoracion { get; set; }
        public int visitas { get; set; }
        [JsonProperty("puntos")]
        public double puntos { get; set; }
        [JsonProperty("ipImpresora")]
        public string ipImpresora { get; set; }
        [JsonProperty("nombreImpresoraBarra")]
        public string nombreImpresoraBarra { get; set; }
        [JsonProperty("nombreImpresoraCocina")]
        public string nombreImpresoraCocina { get; set; }
        [JsonProperty("usuarioBarra")]
        public string usuarioBarra { get; set; }
        [JsonProperty("usuarioCocina")]
        public string usuarioCocina { get; set; }
        public int efectivo { get; set; }
        public int tarjeta { get; set; }
        public int puedeReservar { get; set; }
        public int llamadaCamarero { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool esComercio { get; set; }
        public bool activoLunes { get; set; }
        public bool activoMartes { get; set; }
        public bool activoMiercoles { get; set; }
        public bool activoJueves { get; set; }
        public bool activoViernes { get; set; }
        public bool activoSabado { get; set; }
        public bool activoDomingo { get; set; }

        public int numeroPedidosSoportado { get; set; }

        public TimeSpan? inicioMan { get; set; }
        public TimeSpan? inicioTarde { get; set; }
        public TimeSpan? finMan { get; set; }
        public TimeSpan? finTarde { get; set; }
        public int activoMan { get; set; }
        public int activoTarde { get; set; }


        public TimeSpan? finLunes { get; set; }
        public TimeSpan? finMartes { get; set; }
        public TimeSpan? finMiercoles { get; set; }
        public TimeSpan? finJueves { get; set; }
        public TimeSpan? finViernes { get; set; }
        public TimeSpan? finSabado { get; set; }
        public TimeSpan? finDomingo { get; set; }

        public TimeSpan? inicioLunes { get; set; }
        public TimeSpan? inicioMartes { get; set; }
        public TimeSpan? inicioMiercoles { get; set; }
        public TimeSpan? inicioJueves { get; set; }
        public TimeSpan? inicioViernes { get; set; }
        public TimeSpan? inicioSabado { get; set; }
        public TimeSpan? inicioDomingo { get; set; }

        public TimeSpan? finDomingoTarde { get; set; }
        public TimeSpan? finJuevesTarde { get; set; }
        public TimeSpan? finLunesTarde { get; set; }
        public TimeSpan? finMartesTarde { get; set; }
        public TimeSpan? finMiercolesTarde { get; set; }
        public TimeSpan? finSabadoTarde { get; set; }
        public TimeSpan? finViernesTarde { get; set; }

        public TimeSpan? inicioDomingoTarde { get; set; }
        public TimeSpan? inicioJuevesTarde { get; set; }
        public TimeSpan? inicioLunesTarde { get; set; }
        public TimeSpan? inicioMartesTarde { get; set; }
        public TimeSpan? inicioMiercolesTarde { get; set; }
        public TimeSpan? inicioSabadoTarde { get; set; }
        public TimeSpan? inicioViernesTarde { get; set; }
        public bool servicioActivo { get; set; }
        public int tiempoEntrega { get; set; }
        public string horario { get; set; }
        public bool activoHoy { get; set; }
        public TimeSpan? inicioHoy { get; set; }
        public TimeSpan? finHoy { get; set; }
        public bool activo { get; set; }
        public int orden { get; set; }
        public int favorito { get; set; }
        public double pedidoMinimo { get; set; }
        [JsonProperty("idCategoria")]
        public int idCategoria { get; set; }
        public int idPueblo { get; set; }

        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool recogeEnBarra { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool llevaAMesa { get; set; }
        public string textoMulti { get; set; }
        [Ignore]
        public ConfiguracionEstablecimiento configuracion { get; set; }
        public int idGrupo { get; set; }
        public string web { get; set; }
        public int tipoImpresora { get; set; }
        public string categorias { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool tieneMenuDiario { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool visibleFuera { get; set; }
    }
}
