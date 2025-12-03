using SQLite;

namespace AsadorMoron.Models
{
    public class IngredientesModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }
        public double precio { get; set; }
        public int idEstablecimiento { get; set; }
        public int puntos { get; set; }
    }
}
