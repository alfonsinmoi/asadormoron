using System.Collections.Generic;

namespace AsadorMoron.Models
{
    public class ItemMenuLateralModel
    {
        public string Title { get; set; }

        public string IconSource { get; set; }

        public System.Type TargetType { get; set; }
        public bool TieneHijos { get; set; }
        public List<ItemMenuLateralModel> Hijos { get; set; }
    }
}
