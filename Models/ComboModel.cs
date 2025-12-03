using System;
using System.Collections.ObjectModel;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace AsadorMoron.Models
{
    public class ComboModel:BindableObject
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int tipo { get; set; }
        public string nombreTipo { get; set; }
        private bool seleccionado;
        public bool Seleccionado
        {
            get
            {
                return seleccionado;
            }
            set
            {
                if (seleccionado != value)
                {
                    seleccionado = value;
                    OnPropertyChanged(nameof(Seleccionado));
                }
            }
        }
    }
}

