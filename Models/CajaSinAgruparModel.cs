using System;
namespace AsadorMoron.Models
{
    public class CajaSinAgruparModel
    {
        public DateTime fecha { get; set; }
        public string ProductID { get; set; }

        public string productName { get; set; }

        public float Price { get; set; }

        public float Quantity { get; set; }

        public float Total { get; set; }
    }
}
