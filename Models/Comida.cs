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

        private bool _noTieneOpciones;
        public bool noTieneOpciones
        {
            get => _noTieneOpciones;
            set
            {
                if (_noTieneOpciones != value)
                {
                    _noTieneOpciones = value;
                    OnPropertyChanged(nameof(noTieneOpciones));
                }
            }
        }
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

        /// <summary>
        /// Nombre localizado del artículo según el idioma actual
        /// </summary>
        [Ignore]
        public string NombreLocalizado
        {
            get
            {
                if (articulo == null) return "";
                return App.idioma switch
                {
                    "EN" => !string.IsNullOrEmpty(articulo.nombre_eng) ? articulo.nombre_eng : articulo.nombre,
                    "DE" => !string.IsNullOrEmpty(articulo.nombre_ger) ? articulo.nombre_ger : articulo.nombre,
                    "FR" => !string.IsNullOrEmpty(articulo.nombre_fr) ? articulo.nombre_fr : articulo.nombre,
                    _ => articulo.nombre ?? ""
                };
            }
        }

        /// <summary>
        /// Descripción localizada del artículo según el idioma actual
        /// </summary>
        [Ignore]
        public string DescripcionLocalizada
        {
            get
            {
                if (articulo == null) return "";
                return App.idioma switch
                {
                    "EN" => !string.IsNullOrEmpty(articulo.descripcion_eng) ? articulo.descripcion_eng : articulo.descripcion,
                    "DE" => !string.IsNullOrEmpty(articulo.descripcion_ger) ? articulo.descripcion_ger : articulo.descripcion,
                    "FR" => !string.IsNullOrEmpty(articulo.descripcion_fr) ? articulo.descripcion_fr : articulo.descripcion,
                    _ => articulo.descripcion ?? ""
                };
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
