using SQLite;

namespace AsadorMoron.Models
{
    public class ZonasRepartidorModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idRepartidor { get; set; }
        public int idZona { get; set; }
    }
}
