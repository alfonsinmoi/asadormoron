using SQLite;

namespace AsadorMoron.Models
{
    public class CarritoModel
    {


        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public int idArticulo { get; set; }
        public int idEstablecimiento { get; set; }
        public int idEvento { get; set; }
        public int cantidad { get; set; }
        public string observaciones { get; set; }
        public int opcion { get; set; }
        public string comida { get; set; }
        public string comida_eng { get; set; }
        public string comida_fr { get; set; }
        public string comida_ger { get; set; }
        public bool porEncargo { get; set; }
        public bool esMenu { get; set; }
        public double precio { get; set; }
        public int porPuntos { get; set; }
        public int puntos { get; set; }
        public string imagen { get; set; }

        public double precioTotal { get; set; }
        public string comentario { get; set; }
        public string nombreCantidad { get; set; }
        public string direccion { get; set; }
        public string poblacion { get; set; }
        public int idZona { get; set; }
        public int tipo { get; set; }
        public double gastos { get; set; }
    }
}
