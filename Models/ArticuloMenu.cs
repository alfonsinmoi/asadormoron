using System;
using System.ComponentModel;

namespace AsadorMoron.Models
{
    public class ArticuloMenu : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string imagen { get; set; }
        public string nombre { get; set; }
        public int tipo { get; set; }
        private int cantidad = 0;
        public int Cantidad
        {
            get { return cantidad; }
            set
            {
                cantidad = value;
                OnPropertyChanged(nameof(Cantidad));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
