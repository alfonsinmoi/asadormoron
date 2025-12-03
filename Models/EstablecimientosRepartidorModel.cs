using SQLite;

namespace AsadorMoron.Models
{
    public class EstablecimientosRepartidorModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public int idRepartidor { get; set; }
        public int idEstablecimiento { get; set; }
    }
}
