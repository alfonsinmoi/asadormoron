namespace AsadorMoron.Models
{
    public class OpcionProductoModel
    {
        public int id { get; set; }
        public int idProducto { get; set; }
        public string opcion { get; set; }
        public string opcion_eng { get; set; }
        public string opcion_ger { get; set; }
        public string opcion_fr { get; set; }
        public double valorIncremento { get; set; }
        public int puntos { get; set; }
    }
}
