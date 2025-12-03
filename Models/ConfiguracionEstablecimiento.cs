using Newtonsoft.Json;
using AsadorMoron.Utils;
using SQLite;
using System;

namespace AsadorMoron.Models
{
    public class ConfiguracionEstablecimiento
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoDomingo { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoDomingoLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoJueves { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoJuevesLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoLunes { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMartes { get; set; }
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
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoLunesLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMartesLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMiercolesLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoSabadoLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoViernesLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoDomingoTardeLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoJuevesTardeLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoLunesTardeLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMartesTardeLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoMiercolesTardeLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoSabadoTardeLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activoViernesTardeLocal { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool repartoPropio { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool repartoPolloAndaluz { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool cualquierHoraOtroPueblo { get; set; }
        public string preferenciaReparto { get; set; }
        public TimeSpan finLunes { get; set; }
        public TimeSpan finMartes { get; set; }
        public TimeSpan finMiercoles { get; set; }
        public TimeSpan finJueves { get; set; }
        public TimeSpan finViernes { get; set; }
        public TimeSpan finSabado { get; set; }
        public TimeSpan finDomingo { get; set; }

        public TimeSpan inicioLunes { get; set; }
        public TimeSpan inicioMartes { get; set; }
        public TimeSpan inicioMiercoles { get; set; }
        public TimeSpan inicioJueves { get; set; }
        public TimeSpan inicioViernes { get; set; }
        public TimeSpan inicioSabado { get; set; }
        public TimeSpan inicioDomingo { get; set; }

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

        public TimeSpan finLunesLocal { get; set; }
        public TimeSpan finMartesLocal { get; set; }
        public TimeSpan finMiercolesLocal { get; set; }
        public TimeSpan finJuevesLocal { get; set; }
        public TimeSpan finViernesLocal { get; set; }
        public TimeSpan finSabadoLocal { get; set; }
        public TimeSpan finDomingoLocal { get; set; }

        public TimeSpan inicioLunesLocal { get; set; }
        public TimeSpan inicioMartesLocal { get; set; }
        public TimeSpan inicioMiercolesLocal { get; set; }
        public TimeSpan inicioJuevesLocal { get; set; }
        public TimeSpan inicioViernesLocal { get; set; }
        public TimeSpan inicioSabadoLocal { get; set; }
        public TimeSpan inicioDomingoLocal { get; set; }

        public TimeSpan finDomingoTardeLocal { get; set; }
        public TimeSpan finJuevesTardeLocal { get; set; }
        public TimeSpan finLunesTardeLocal{ get; set; }
        public TimeSpan finMartesTardeLocal { get; set; }
        public TimeSpan finMiercolesTardeLocal { get; set; }
        public TimeSpan finSabadoTardeLocal { get; set; }
        public TimeSpan finViernesTardeLocal { get; set; }

        public TimeSpan inicioDomingoTardeLocal { get; set; }
        public TimeSpan inicioJuevesTardeLocal { get; set; }
        public TimeSpan inicioLunesTardeLocal { get; set; }
        public TimeSpan inicioMartesTardeLocal { get; set; }
        public TimeSpan inicioMiercolesTardeLocal { get; set; }
        public TimeSpan inicioSabadoTardeLocal { get; set; }
        public TimeSpan inicioViernesTardeLocal { get; set; }

        public int idEstablecimiento { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool servicioActivo { get; set; }
        public int tiempoEntrega { get; set; }
        public double pedidoMinimo { get; set; }
        public int numeroPedidosSoportado { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool modoEscaparate { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool detalles { get; set; }
        public string textoDetalle { get; set; }
        public string textoIngredientes { get; set; }
        public string textoDetalle_eng { get; set; }
        public string textoIngredientes_eng { get; set; }
        public string textoDetalle_ger { get; set; }
        public string textoIngredientes_ger { get; set; }
        public string textoDetalle_fr { get; set; }
        public string textoIngredientes_fr { get; set; }
        public double comision { get; set; }
        public double comisionLocal { get; set; }
        public double comisionRecogida { get; set; }
        public double comisionReparto { get; set; }
        public double comisionAutoPedido { get; set; }
        public string nombreImpresora { get; set; }
        public string nombreImpresora2 { get; set; }
        public string nombreImpresora3 { get; set; }
        public string nombreImpresora4 { get; set; }
        public string nombreImpresora5 { get; set; }
        public string nombreImpresora6 { get; set; }
        public string nombreImpresora7 { get; set; }
        public string nombreImpresora8 { get; set; }
        public string nombreImpresora9 { get; set; }
        public string nombreImpresora10 { get; set; }
        public double cuotaFija { get; set; }
        public double otrasCuotas { get; set; }
        public int alturaLineaImpresora { get; set; }
        public double gastosEnvioPropio { get; set; }
        public int encargosPorHora { get; set; }
        public int encargosDiasDesde { get; set; }
        public int encargosDiasHasta { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool aceptaEncargos { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool sistemaPuntos { get; set; }
        public int puntosPorPedido { get; set; }
        public int puntosPorEuro { get; set; }
        public string textoPuntos { get; set; }
        public int visibilidadHoras { get; set; }
        public int numImpresion { get; set; }
        public int tipoAutoPedido { get; set; }
        public int estadoAutoPedido { get; set; }
        public int idZonaAutoPedido { get; set; }
        public int tiempoRepartoComercio { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool repartoComercioM { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool repartoComercioT { get; set; }
        public int ticketSize { get; set; }
        public TimeSpan horaSoloTarjetaDesde { get; set; }
        public TimeSpan horaSoloTarjetaHasta { get; set; }
        [JsonConverter(typeof(BoolToBitJsonConverter))]
        public bool activaSoloTarjeta { get; set; }
        [JsonProperty("tieneMediaPizza")]
        public double precioBolsa { get; set; }
        [JsonProperty("idCategoriaPizza")]
        public double rangoBolsas { get; set; }
        public double suplementoMediaPizza { get; set; }
        public string textoCerrado { get; set; }
    }
}
