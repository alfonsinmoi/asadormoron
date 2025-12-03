using SQLite;

namespace AsadorMoron.Models
{
    public class TokenModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string nombre { get; set; }
        public string token { get; set; }
    }
}
