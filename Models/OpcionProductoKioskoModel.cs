using SQLite;

namespace AsadorMoron.Models
{
    /// <summary>
    /// Modelo para almacenar opciones de productos en SQLite para modo Kiosko.
    /// Incluye la relacion con el producto padre.
    /// </summary>
    public class OpcionProductoKioskoModel
    {
        [PrimaryKey, AutoIncrement]
        public int idLocal { get; set; }

        /// <summary>
        /// ID del producto al que pertenece esta opcion
        /// </summary>
        public int idProducto { get; set; }

        /// <summary>
        /// ID de la opcion en el servidor
        /// </summary>
        public int id { get; set; }

        public string opcion { get; set; }
        public string opcion_eng { get; set; }
        public string opcion_ger { get; set; }
        public string opcion_fr { get; set; }
        public string precio { get; set; }
        public int tipoIncremento { get; set; }
        public int puntos { get; set; }
        public string observaciones { get; set; }
    }
}
