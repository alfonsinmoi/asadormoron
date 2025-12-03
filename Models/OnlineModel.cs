using System;
namespace AsadorMoron.Models
{
    public class OnlineModel
    {
        public int id { get; set; }
        public int idUsuario { get; set; }
        public string tokenUsuario { get; set; }
        public int idPueblo { get; set; }
        public DateTime horaInicio { get; set; }
        public DateTime? horaCierre { get; set; }
        public DateTime? horaBackground { get; set; }
        public DateTime? horaResume { get; set; }
    }
}
