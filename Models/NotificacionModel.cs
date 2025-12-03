using SQLite;
using System;

namespace AsadorMoron.Models
{
    public class NotificacionModel
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public string Titulo { get; set; }
        public string Cuerpo { get; set; }
        public DateTimeOffset Fecha { get; set; }

        private string mostrarLeido;

        public string MostrarLeido
        {
            get { return mostrarLeido; }
            set
            {
                mostrarLeido = value;
                //OnPropertyChanged(nameof(MostrarLeido));
            }
        }

        public int leido { get; set; }
        [Ignore]
        public string Icono { get; set; }

        private string fondo;
        [Ignore]
        public string Fondo
        {
            get { return fondo; }
            set
            {
                fondo = value;
                //OnPropertyChanged(nameof(Fondo));
            }
        }
    }
}
