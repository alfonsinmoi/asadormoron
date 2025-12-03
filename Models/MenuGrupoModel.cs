using System;
using System.Collections.Generic;

namespace AsadorMoron.Models
{
    public class MenuGrupoModel : List<ArticuloMenu>
    {
        public string Name { get; private set; }

        public MenuGrupoModel(string name, List<ArticuloMenu> productos) : base(productos)
        {
            Name = name;
        }
    }
    
}