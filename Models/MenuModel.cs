using System;
namespace AsadorMoron.Models
{
    public class MenuModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string nombre_ingles { get; set; }
        public string nombre_frances { get; set; }
        public string nombre_aleman { get; set; }
        public string viewmodel { get; set; }
        public string rol { get; set; }
        public string imagen { get; set; }
        public int idParent { get; set; }
    }
}
