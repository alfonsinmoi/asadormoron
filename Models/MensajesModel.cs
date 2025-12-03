using SQLite;

namespace AsadorMoron.Models
{
    public class MensajesModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string clave { get; set; }
        public string valor { get; set; }
        public string valor_eng { get; set; }
        public string valor_ger { get; set; }
        public string valor_fr { get; set; }
    }
}
