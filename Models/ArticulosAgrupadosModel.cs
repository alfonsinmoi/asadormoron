using System.Collections.Generic;

namespace AsadorMoron.Models
{
    public class ArticulosAgrupadosModel : List<ArticuloModel>
    {
        public string Nombre { get; set; }
        public ArticulosAgrupadosModel(string name, List<ArticuloModel> articulos) : base(articulos)
        {
            Nombre = name;
        }
    }
}
