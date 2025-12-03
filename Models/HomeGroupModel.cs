using System.Collections.Generic;

namespace AsadorMoron.Models
{
    public class HomeGroupModel
    {
        public int idTipo { get; set; }
        public string tipo { get; set; }
        public bool visibleEstablecimiento { get; set; }
        public List<Establecimiento> establecimiento { get; set; }

    }
}
