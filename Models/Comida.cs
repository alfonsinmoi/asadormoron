using System;
using System.ComponentModel;
using SQLite;

namespace AsadorMoron.Models
{
    public class Comida : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public ArticuloModel articulo { get; set; }
        public bool esMediaPizza { get; set; }
        public Boolean noTieneOpciones { get; set; }
        [Ignore]
        public Boolean botonesVisibles { get; set; }
        [Ignore]
        public int idEstablecimiento { get; set; }
        private int _cantidad;
        public string observaciones { get; set; }
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
        }
    }
}
