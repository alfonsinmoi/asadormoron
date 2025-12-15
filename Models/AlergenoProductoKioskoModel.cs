using SQLite;

namespace AsadorMoron.Models
{
    /// <summary>
    /// Modelo para almacenar alergenos de productos en SQLite para modo Kiosko.
    /// Incluye la relacion con el producto padre.
    /// </summary>
    public class AlergenoProductoKioskoModel
    {
        [PrimaryKey, AutoIncrement]
        public int idLocal { get; set; }

        /// <summary>
        /// ID del producto al que pertenece este alergeno
        /// </summary>
        public int idProducto { get; set; }

        /// <summary>
        /// ID del alergeno en el servidor
        /// </summary>
        public int id { get; set; }

        public string nombre { get; set; }
        public string imagen { get; set; }
    }
}
