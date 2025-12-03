using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class FranjaHorariaModel : BindableObject
    {
        public string horaInicio { get; set; }
        public string horaFin { get; set; }
        public TimeSpan horaInicioReal { get; set; }
        private string colo;
        public string Color
        {
            get
            {
                return colo;
            }
            set
            {
                if (colo != value)
                {
                    colo = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }
    }
}
