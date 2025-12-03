using SQLite;

namespace AsadorMoron.Models
{
    public class AlergenosModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string nombre { get; set; }
        public string imagen { get; set; }
    }
}
