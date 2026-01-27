using System;
using System.Collections.Generic;
using AsadorMoron.Models;
using Mopups.Pages;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using AsadorMoron.Services;
using Microsoft.Maui.Controls.Xaml;
using Mopups.Services;
using System.Linq;

namespace AsadorMoron.Views.Repartidores
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPageInfoRepartidor : PopupPage
    {
        RepartidorModel rep;
        public PopupPageInfoRepartidor()
        {
            InitializeComponent();
        }

        public PopupPageInfoRepartidor(RepartidorModel repartidor)
        {
            InitializeComponent();
            rep = repartidor;
            txtNombre.Text = repartidor.nombre;
            txtTelefono.Text = repartidor.telefono;
            pbFoto.Source = repartidor.foto;
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await App.ResponseWS.EnviarMensajeRepartidor(txtMensaje.Text, rep.id);
            await MopupService.Instance.PopAsync(true);
        }

        void cbPredefinidos_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            Picker pick = (Picker)sender;
            if (pick.SelectedIndex > 0)
                txtMensaje.Text = ((PredefinidosModel)pick.SelectedItem).texto;
        }
    }
}
