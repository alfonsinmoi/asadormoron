using System;
namespace AsadorMoron.Models
{
    public class HomeModel
    {
        public double distancia { get; set; }
        public int idTipo { get; set; }
        public string imagen { get; set; }
        public string provincia { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string tipo { get; set; }
        public int numeroCategorias { get; set; }
        public int numeroProductos { get; set; }
        public double ventas { get; set; }
        public int zonas { get; set; }
        public int favorito { get; set; }
        public int local { get; set; }
        public int envio { get; set; }
        public int recogida { get; set; }
        public int valoraciones { get; set; }
        public double puntos { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public int estado { get; set; }
        public int id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public int esComercio { get; set; }
        public string poblacion { get; set; }
        public string codPostal { get; set; }
        public string logo { get; set; }
        public DateTime? fechaInicio { get; set; }
        public DateTime? fechaFin { get; set; }
        public string descripcion { get; set; }
        public string equipoLocal { get; set; }
        public string imagenEquipoLocal { get; set; }
        public string equipoVisitante { get; set; }
        public string imagenEquipoVisitante { get; set; }
        public string jornada { get; set; }
        public string temporada { get; set; }
        public string estadio { get; set; }
        public string ipImpresora { get; set; }
        public string nombreImpresoraBarra { get; set; }
        public string nombreImpresoraCocina { get; set; }
        public string usuarioBarra { get; set; }
        public string usuarioCocina { get; set; }
        public int llamadaCamarero { get; set; }
        public int puedeReservar { get; set; }

        public bool activoLunes { get; set; }
        public bool activoMartes { get; set; }
        public bool activoMiercoles { get; set; }
        public bool activoJueves { get; set; }
        public bool activoViernes { get; set; }
        public bool activoSabado { get; set; }
        public bool activoDomingo { get; set; }
        public bool activoLunesTarde { get; set; }
        public bool activoMartesTarde { get; set; }
        public bool activoMiercolesTarde { get; set; }
        public bool activoJuevesTarde { get; set; }
        public bool activoViernesTarde { get; set; }
        public bool activoSabadoTarde { get; set; }
        public bool activoDomingoTarde { get; set; }

        public bool abierto { get; set; }

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
        public double pedidoMinimo { get; set; }
        public int idCategoria { get; set; }
        public int orden { get; set; }
        public int idZona { get; set; }
    }
}
