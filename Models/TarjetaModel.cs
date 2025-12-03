using SQLite;

namespace AsadorMoron.Models
{
    public class TarjetaModel
    {
        [PrimaryKey]
        public int id { get; set; }
        public string pan { get; set; }
        public string cardBrand { get; set; }
        public int errorCode { get; set; }
        public string cardType { get; set; }
        public string expiryDate { get; set; }
        public int idUser { get; set; }
        public string tokenUser { get; set; }
        public int idUsuario { get; set; }
    }
}
