using System.ComponentModel;

namespace AsadorMoron.Models
{
    public class OpcionesModel : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string opcion { get; set; }
        public string opcion_eng { get; set; }
        public string opcion_ger { get; set; }
        public string opcion_fr { get; set; }
        public string precio { get; set; }
        public int tipoIncremento { get; set; }
        public int puntos { get; set; }
        public string observaciones { get; set; }
        private bool _seleccionado;
        private int _cantidad;
        public bool visiblePuntos { get; set; }
        public virtual int cantidad
        {
            get { return _cantidad; }
            set
            {
                if (_cantidad != value)
                {

                    _cantidad = value;
                    OnPropertyChanged(nameof(cantidad));

                }

            }
        }
        public virtual bool seleccionado
        {
            get { return _seleccionado; }
            set
            {
                if (_seleccionado != value)
                {

                    _seleccionado = value;
                    OnPropertyChanged(nameof(seleccionado));

                }

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
        }
    }
}
