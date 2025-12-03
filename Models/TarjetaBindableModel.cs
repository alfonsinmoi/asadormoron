using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class TarjetaBindableModel:BindableObject
    {
        public TarjetaModel tarjeta { get; set; }
        private string Fondo;
        public string fondo
        {
            get { return Fondo; }
            set
            {
                if (Fondo != value)
                {
                    Fondo = value;
                    OnPropertyChanged(nameof(fondo));
                }
            }
        }

        private bool seleccionada;

        public bool Seleccionada
        {
            get { return seleccionada; }
            set 
            {
                seleccionada = value;
                OnPropertyChanged();
            }
        }

    }
}
